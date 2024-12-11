using System.IO;
using UnityEngine;

public class TestPngFileDialog : MonoBehaviour
{
    public Stream testStream;
    public void FileOpenBtn()
    {
        testStream = PngFileDialog.FileOpen();
    }

    public void ConvertPngStreamToBase64Btn()
    {
        string base64 = PngFileDialog.ConvertPngStreamToBase64(testStream);
        Debug.Log($"[PngFileDialog]Png->Base 64 - {base64}");
    }
}