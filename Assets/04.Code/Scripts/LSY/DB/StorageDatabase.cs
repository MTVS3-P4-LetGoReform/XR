using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Firebase.Storage;
using Application = UnityEngine.Device.Application;


public static class StorageDatabase
{
    private static FirebaseStorage storage;
    private static StorageReference storage_ref;
    private static StorageReference isstorage_ref;
    private static string local_url;
    private static WebApiData webApiData;
    public static DebugModeData _debugModeData;

    public static void InitializStorageDatabase(WebApiData webapi, DebugModeData debugmodedata)
    {
        webApiData = webapi;
        _debugModeData = debugmodedata;
        storage = FirebaseStorage.DefaultInstance;
        storage_ref = storage.GetReferenceFromUrl(webApiData.StorageBaseUrl);
    }
    
    public static async UniTask DownModel(string modelName)
    {
        if (_debugModeData.DebugMode == true)
        {
            return;
        }
        isstorage_ref = storage_ref.Child(webApiData.StorageModelsPoint).Child(modelName);
        Debug.Log("StorageDatabase : isstorage_ref - "+ isstorage_ref);
        local_url = Path.Combine(Application.persistentDataPath,"Models", modelName);
        if (!File.Exists(local_url))
        {
            Debug.LogWarning("이미 파일이 존재합니다. 파일명 :" + local_url);
        }
        Debug.Log(string.Format("{0}", local_url));
        await isstorage_ref.GetFileAsync(local_url);
    }
    
    public static async UniTask DownImage(string imageName)
    {
        if (_debugModeData.DebugMode == true)
        {
            return;
        }
        isstorage_ref = storage_ref.Child("images").Child(imageName);
        Debug.Log("StorageDatabase : isstorage_ref - "+ isstorage_ref);
        local_url = Path.Combine(Application.persistentDataPath,"images",imageName);
        Debug.Log(string.Format("{0}", local_url));
        if (!File.Exists(local_url))
        {
            Debug.LogWarning("이미 파일이 존재합니다. 파일명 :" + local_url);
        }
        Debug.Log("로컬 다운로드 주소 : "+local_url);
        await isstorage_ref.GetFileAsync(local_url);
    }
    
    public static async UniTask DownModelPlaySession(string modelName, SessionUIManager _sessionUIManager)
    {
        if (_debugModeData.DebugMode == false)
        {
            isstorage_ref = storage_ref.Child(webApiData.StorageModelsPoint).Child(modelName);
            local_url = Path.Combine(Application.persistentDataPath,"Models",modelName);
            Debug.Log(string.Format("{0}", local_url));
            //GetFileAsync : 파일 비동기로 로컬 저장소에 다운로드
            // COntinueWith : 다운로드 작업이 완료된 후의 작업을 설정.
            await isstorage_ref.GetFileAsync(local_url);
        }
        
        _sessionUIManager.CreatePlaySession();
    }
    // public async UniTask DownModelPlaySessionDebug(string modelName, SessionUIManager _sessionUIManager)
    // {
    //     
    //     _sessionUIManager.CreatePlaySession();
    // }
}