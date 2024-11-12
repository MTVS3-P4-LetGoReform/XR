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

    public StorageDatabase(WebApiData webapi)
    {
        webApiData = webapi;
        storage = FirebaseStorage.DefaultInstance;
        storage_ref = storage.GetReferenceFromUrl(webApiData.StorageBaseUrl);
        //isstorage_ref = storage_ref.Child(webApiData.StorageModelsPoint+"/"+webApiData.TempModelName);
        //local_url = Application.persistentDataPath + webApiData.TempModelName;
    }
    
    public async UniTask DownModel(string modelName)
    {
        isstorage_ref = storage_ref.Child(webApiData.StorageModelsPoint + "/" + modelName);
        local_url = Application.persistentDataPath + "/" + modelName;
        Debug.Log(string.Format("{0}", local_url));
        await isstorage_ref.GetFileAsync(local_url);
    }
    
    public async UniTask DownModelPlaySession(string modelName, SessionUIManager _sessionUIManager)
    {
        isstorage_ref = storage_ref.Child(webApiData.StorageModelsPoint + "/" + modelName);
        local_url = Application.persistentDataPath + "/" + modelName;
        Debug.Log(string.Format("{0}", local_url));
        //GetFileAsync : 파일 비동기로 로컬 저장소에 다운로드
        // COntinueWith : 다운로드 작업이 완료된 후의 작업을 설정.
        await isstorage_ref.GetFileAsync(local_url);
        _sessionUIManager.CreatePlaySession();
    }
    public async UniTask DownModelPlaySessionDebug(string modelName, SessionUIManager _sessionUIManager)
    {
        
        _sessionUIManager.CreatePlaySession();
    }
}