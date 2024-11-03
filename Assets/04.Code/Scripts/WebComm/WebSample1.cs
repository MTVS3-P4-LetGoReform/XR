using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using GLTFast;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class WebSample1 : MonoBehaviour
{
    public TMP_Text promptInput;
    public List<Image> promptImages;
    public string fileName = "958462d6cbd04f441aad81e88529b8a2.glb";
    public string localSavePath = "Assets/DownloadedGLB/";
    public string baseUrl = "http://metaai2.iptime.org:13089/";
    public string downGLBUrl = "download3d/";

    public string directory = @"C:\Users\Admin\MTVS3_P4_Private\Assets\DownloadedGLB";

    public string filePath;
    private Stopwatch stopwatch;
    void Start()
    {
        StartCoroutine(DownloadGLBFile(fileName));
        stopwatch = new Stopwatch();
        stopwatch.Start();
    }

 

    public IEnumerator DownloadGLBFile(string fileName)
    {
        // JSON 데이터 생성
        string jsonData = "{\"filename\":\"" + fileName + "\"}";

        UnityWebRequest request = new UnityWebRequest(baseUrl + downGLBUrl, "POST");
        byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 요청 전송
        yield return request.SendWebRequest();

        stopwatch.Stop();
        Debug.Log(" response time : "+stopwatch.ElapsedMilliseconds+"ms");
        // 응답 체크
        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            // 응답 데이터가 존재하면 파일 저장
            stopwatch.Start();
            byte[] fileData = request.downloadHandler.data;
            stopwatch.Stop();
            Debug.Log(" downlaodHandler time : "+stopwatch.ElapsedMilliseconds+"ms");
            // 로컬 경로에 .glb 파일 저장
            stopwatch.Start();
            //string filePath = Path.Combine(Application.dataPath, "DownloadedGLB", "958462d6cbd04f441aad81e88529b8a2.glb");
            filePath = Path.Combine(directory, fileName);
            File.WriteAllBytes(filePath, fileData);
            stopwatch.Stop();
            Debug.Log("writeBytes time : " + stopwatch.ElapsedMilliseconds + "ms");
            Debug.Log("File downloaded and saved to: " + filePath);
        }
    }

    public async void LoadAndInstantiateGLB()
    {
        //string filePath = Path.Combine(Application.dataPath, "DownloadedGLB", "958462d6cbd04f441aad81e88529b8a2.glb");
        // 파일이 존재하는지 확인
        if (File.Exists(filePath))
        {
            // glTFast를 이용해 GLB 파일을 로드
            GltfImport gltfImport = new GltfImport();
            bool success = await gltfImport.Load(filePath); // 비동기 로드 처리

            // 파일 로드에 성공하면 씬에 배치
            if (success)
            {
                GameObject glbObject = new GameObject("GLBModel");
                await gltfImport.InstantiateMainSceneAsync(glbObject.transform);
                Debug.Log("GLB file instantiated in scene.");
            }
            else
            {
                Debug.LogError("Failed to load GLB file using glTFast.");
            }
        }
        else
        {
            Debug.LogError("File does not exist: " + filePath);
        }
    }
}