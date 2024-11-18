using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Firebase.Storage;
using Application = UnityEngine.Device.Application;


public class StorageDatabase
{
    private FirebaseStorage storage;
    private StorageReference storage_ref;
    private StorageReference isstorage_ref;
    private string local_url;
    private WebApiData webApiData;
    public DebugModeData _debugModeData;

    public StorageDatabase(WebApiData webapi, DebugModeData debugmodedata)
    {
        webApiData = webapi;
        _debugModeData = debugmodedata;
        storage = FirebaseStorage.DefaultInstance;
        storage_ref = storage.GetReferenceFromUrl(webApiData.StorageBaseUrl);
        //isstorage_ref = storage_ref.Child(webApiData.StorageModelsPoint+"/"+webApiData.TempModelName);
        //local_url = Application.persistentDataPath + webApiData.TempModelName;
    }
    
    public async UniTask DownModel(string modelName)
    {
        if (_debugModeData.DebugMode == true)
        {
            return;
        }
        isstorage_ref = storage_ref.Child(webApiData.StorageModelsPoint).Child(modelName);
        Debug.Log("StorageDatabase : isstorage_ref - "+ isstorage_ref);
        local_url = Path.Combine(Application.persistentDataPath, modelName);
        Debug.Log(string.Format("{0}", local_url));
        await isstorage_ref.GetFileAsync(local_url);
    }
    
    public async UniTask DownImage(string imageName)
    {
        if (_debugModeData.DebugMode == true)
        {
            return;
        }
        isstorage_ref = storage_ref.Child("images" + "/" + imageName);
        Debug.Log("StorageDatabase : isstorage_ref - "+ isstorage_ref);
        local_url = Application.persistentDataPath + "/" + imageName;
        Debug.Log(string.Format("{0}", local_url));
        await isstorage_ref.GetFileAsync(local_url);
    }
    
    public async UniTask DownModelPlaySession(string modelName, SessionUIManager _sessionUIManager)
    {
        if (_debugModeData.DebugMode == false)
        {
            isstorage_ref = storage_ref.Child(webApiData.StorageModelsPoint + "/" + modelName);
            local_url = Application.persistentDataPath + "/" + modelName;
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