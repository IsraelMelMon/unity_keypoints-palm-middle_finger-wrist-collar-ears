using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TensorFlowLite;

/// <summary>
/// BlazeFace from MediaPile
/// https://github.com/google/mediapipe
/// https://viz.mediapipe.dev/demo/face_detection
/// </summary>
public class FaceDetectionSample : MonoBehaviour
{
    [SerializeField, FilePopup("*.tflite")] string faceModelFile = "coco_ssd_mobilenet_quant.tflite";
    [SerializeField] RawImage cameraView = null;

    WebCamTexture webcamTexture;
    FaceDetect faceDetect;
    List<FaceDetect.Result> results;
    PrimitiveDraw draw;
    Vector3[] rtCorners = new Vector3[4];

    void Start()
    {
        string detectionPath = Path.Combine(Application.streamingAssetsPath, faceModelFile);
        faceDetect = new FaceDetect(detectionPath);

        string cameraName = WebCamUtil.FindName(new WebCamUtil.PreferSpec()
        {
            isFrontFacing = false,
            kind = WebCamKind.WideAngle,
        });
        webcamTexture = new WebCamTexture(cameraName, 1280, 720, 30);
        cameraView.texture = webcamTexture;
        webcamTexture.Play();
        Debug.Log($"Starting camera: {cameraName}");

        draw = new PrimitiveDraw(Camera.main, gameObject.layer);
    }

    void OnDestroy()
    {
        webcamTexture?.Stop();
        faceDetect?.Dispose();
        draw?.Dispose();
    }

    void Update()
    {
        faceDetect.Invoke(webcamTexture);
        cameraView.material = faceDetect.transformMat;
        cameraView.rectTransform.GetWorldCorners(rtCorners);

        var results = faceDetect.GetResults();
        if (results == null) return;

        DrawResults(results);
    }

    void DrawResults(List<FaceDetect.Result> results)
    {
        Vector3 min = rtCorners[0];
        Vector3 max = rtCorners[2];
        //Vector2
        draw.color = Color.blue;

        foreach (var result in results)
        {
            Rect rect = MathTF.Lerp(min, max, result.rect, true);
            //draw.Rect(rect, 0.05f);
            //Debug.Log(result.keypoints);
            //foreach (Vector2 p in result.keypoints)
            //{
                //Debug.Log(p);
            //    draw.Point(MathTF.Lerp(min, max, new Vector3(p.x, 1f - p.y, 0)), 0.1f);
            //}
            //Vector2 
            for(int i = 4; i < 6; i++)
            {
                draw.Point(MathTF.Lerp(min, max, new Vector3(result.keypoints[i].x, 1f - result.keypoints[i].y, 0)), 0.1f);
            }
            //foreach (Vector2 p in result.keypoints)
            //{
                //Debug.Log(p);
            //    draw.Point(MathTF.Lerp(min, max, new Vector3(p.x, 1f - p.y, 0)), 0.1f);
            // }
            //draw.Point 
        }
        draw.Apply();
    }


}
