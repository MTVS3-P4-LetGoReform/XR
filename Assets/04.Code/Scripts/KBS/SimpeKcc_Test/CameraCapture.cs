using System;
using System.IO;
using Fusion;
using UnityEngine;

public class CameraCapture : NetworkBehaviour
{
    public Camera FaceCamera;

    public int captureWidth = 1920;

    public int captureHeight = 1080;

    private RenderTexture renderTexture;

    private Texture2D screenShot;

    public Canvas CaptureCanvas;

    
    
    void Start()
    {
        renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
        screenShot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            CaptureAndSave();
        }
    }

    public void CaptureAndSave()
    {
        CaptureCanvas.gameObject.SetActive(false);
        
        string customPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "BlockBusters");
        if (!Directory.Exists(customPath))
        {
            Directory.CreateDirectory(customPath);
        }
        
        FaceCamera.targetTexture = renderTexture;
        FaceCamera.Render();

        RenderTexture.active = renderTexture;
        screenShot.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
        screenShot.Apply();

        FaceCamera.targetTexture = null;
        RenderTexture.active = null;

        
        byte[] bytes = screenShot.EncodeToPNG();
        
        
        string timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fileName = $"ScreenShot_{timeStamp}.png";
        
        string filePath = Path.Combine(customPath, fileName);
        
        File.WriteAllBytes(filePath, bytes);
        
        Debug.Log($"스샷찍혔음 ㅋㅋ : {filePath}");
        
        CaptureCanvas.gameObject.SetActive(true);
    }
}
