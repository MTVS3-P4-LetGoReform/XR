using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using Firebase.Storage;
using Unity.VisualScripting;
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
    
    public void DownModel(string modelName)
    {
        isstorage_ref = storage_ref.Child(webApiData.StorageModelsPoint + "/" + modelName);
        local_url = Application.persistentDataPath + "/" + modelName;
        Debug.Log(string.Format("{0}", local_url));
        //GetFileAsync : 파일 비동기로 로컬 저장소에 다운로드
        // COntinueWith : 다운로드 작업이 완료된 후의 작업을 설정.
        isstorage_ref.GetFileAsync(local_url).ContinueWith(file_task =>
        {
            if (file_task.IsCompletedSuccessfully)
            {
                Debug.Log("모델 파일 다운로드 성공");
            }
            else
            {
                Debug.Log("다운로드 실패");
                Debug.Log(file_task.Exception.ToString());
            }
        });
    }
}