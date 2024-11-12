using UnityEngine;

public class StatueInventoryDB: MonoBehaviour
{
    private string _userId;
    private string[] hasModel;
    private string[] hasModelName;
    private StorageDatabase _storageDatabase;
    public WebApiData webApiData;
    
    private void Start()
    {
        _storageDatabase = new StorageDatabase(webApiData);
        if (UserData.Instance ==null)
            return;
        _userId = UserData.Instance.UserId;

        LoadUserModelData();
        
    }

    public void LoadUserModelData()
    {
        RealtimeDatabase.GetModelList(_userId, modelList =>
            {
                if (modelList != null)
                {
                    Debug.Log($"StatueInventoryDB : modelList - {modelList}");
                    foreach (var model in modelList.models)
                    {
                        Debug.Log($"StatueInventoryDB : model - {model}");
                    }
                }
                else
                {
                    Debug.LogError("StaueInventoryDB : 모델리스트 데이터를 찾을 수 없음.");
                }
            },
            error => { Debug.LogError($"StatueInventoryDB : 데이터 읽는 중 오류 -{error.Message}"); }
        );
    }

    
}