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

public class ModelDown
{
    //public TMP_Text promptInput;
    //public List<Image> promptImages;
    //public string fileName = "958462d6cbd04f441aad81e88529b8a2.glb";
    //public string localSavePath = "Assets/DownloadedGLB/";
    //public string baseUrl = "http://metaai2.iptime.org:13089/";
    //public string downGLBUrl = "download3d/";

    //public string directory = @"C:\Users\Admin\MTVS3_P4_Private\Assets\DownloadedGLB";

    public ModelDownReq _modelDownReq;

    private WebApiData webApiData;
    //private Stopwatch stopwatch;
    // void Start()
    // {
    //     StartCoroutine(DownloadGLBFile(fileName));
    //     stopwatch = new Stopwatch();
    //     stopwatch.Start();
    // }
    public ModelDown(WebApiData webapi)
    {
        _modelDownReq = new ModelDownReq();
        webApiData = webapi;
    }

    public IEnumerator DownloadGLBFile(string fName)
    {
        // JSON 데이터 생성
        //string jsonData = "{\"filename\":\"" + fileName + "\"}";
        if (string.IsNullOrEmpty(fName))
        {
            Debug.LogError("Modeldown: fName" +
                           " is null or empty.");
            yield break;
        }

        _modelDownReq.filename = fName;
        UnityWebRequest request = new UnityWebRequest(webApiData.Baseurl + webApiData.ModelDownPoint, "POST");
        string jsonData = JsonUtility.ToJson(_modelDownReq);
        byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 요청 전송
        yield return request.SendWebRequest();

        
        if (request.result == UnityWebRequest.Result.Success)
        {
            // 응답 데이터가 존재하면 파일 저장
            byte[] fileData = request.downloadHandler.data;
            
            // 이미지 폴더 경로 생성
            string folderPath = Path.Combine(Application.persistentDataPath, "Models");
            // 폴더가 없을 시 폴더 생성
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                Debug.Log("Folder created at: "+folderPath);
            }
            else
            {
                Debug.Log("Folder alread exits at: "+folderPath);
            }
            
            string filePath = Path.Combine(folderPath, _modelDownReq.filename);
            File.WriteAllBytes(filePath, fileData);
            Debug.Log("File downloaded and saved to: " + filePath);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }

    public async void LoadAndInstantiateGLB(string fName)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "Models");
        string filePath = Path.Combine(folderPath, fName);
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