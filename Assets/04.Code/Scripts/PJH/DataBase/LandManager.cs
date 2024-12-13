using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LandManager : MonoBehaviour
{
    public string userId = "TestUser"; // 유저 ID
    public string userName;
    public ObjectDatabase prefab; // LandObject에 맞는 프리팹
    public static Dictionary<string, GameObject> PlacedObjects = new Dictionary<string, GameObject>(); // 이미 배치된 오브젝트 목록
    
    public LandUIController uIController;
    
   
    private async void Start()
    {
        var properties = RunnerManager.Instance.runner.SessionInfo.Properties;
        if (properties.TryGetValue("UserId", out var sessionProperty))
        {
            userId = sessionProperty;
        }

        userName = await RealtimeDatabase.FindNameByIdAsync(userId);
        
        Debug.Log($"DB에서 정보를 불러옵니다. 현재 영지 : {userId}");
        RunnerManager.Instance.IsSpawned += AfterSpawn;
    }

    private void AfterSpawn()
    {
        RealtimeDatabase.ListenForUserLandChanges(userId, UpdateLandObjects, exception => Debug.LogError("실시간 데이터 수신 오류: " + exception.Message));
    }
    

    private void UpdateLandObjects(UserLand updatedUserLand)
    {
        if (updatedUserLand == null)
        {
            Debug.LogWarning("updatedUserLand가 null입니다. 업데이트를 건너뜁니다.");
            return;
        }
        
        // 데이터 구조 디버깅
        Debug.Log($"UserLand 데이터: {JsonUtility.ToJson(updatedUserLand)}");
        
        if (updatedUserLand.landInfo == null)
        {
            Debug.Log("LandInfo 초기화 중...");
            var newLandInfo = new LandInfo(userName);
        
            // Firebase에 LandInfo 저장
            RealtimeDatabase.SetUserLandInfo(userId, newLandInfo, 
                onSuccess: () => {
                    Debug.Log("LandInfo 저장 성공");
                    updatedUserLand.landInfo = newLandInfo;
                    uIController.SetInfo(userId, userName, newLandInfo);
                },
                onFailure: (error) => {
                    Debug.LogError($"LandInfo 저장 실패: {error.Message}");
                });
            return;
        }
        
        if (updatedUserLand.objects == null)
        {
            Debug.LogWarning("updatedUserLand 또는 updatedUserLand.objects가 null입니다. 업데이트를 건너뜁니다.");
            return;
        }

        foreach (LandObject landObject in updatedUserLand.objects)
        {
            if (string.IsNullOrEmpty(landObject.key))
            {
                Debug.LogWarning("landObject.key가 null이거나 비어있습니다. 해당 오브젝트를 무시합니다.");
                continue;
            }

            if (PlacedObjects.TryGetValue(landObject.key, out var obj))
            {
                if (obj != null)
                {
                    obj.transform.position = landObject.position.ToVector3();
                    obj.transform.eulerAngles = landObject.rotation.ToVector3();
                    obj.transform.localScale = landObject.scale.ToVector3();
                }
                else
                {
                    PlacedObjects.Remove(landObject.key);
                    PlaceLandObject(landObject);
                }
            }
            else
            {
                Debug.Log("배치되었습니다."+landObject.key);
                PlaceLandObject(landObject);
            }
        }
        
        var keysToRemove = new List<string>(PlacedObjects.Keys);
        foreach (var key in keysToRemove)
        {
            if (!updatedUserLand.objects.Exists(obj => obj.key == key))
            {
                GameObject objToDestroy = PlacedObjects[key];
                if (objToDestroy != null)
                {
                    Destroy(objToDestroy);
                    Debug.Log($"오브젝트 '{key}'가 씬에서 삭제되었습니다.");
                    
                    PlacedObjects.Remove(key);
                }
                else
                {
                    Debug.LogWarning($"오브젝트 '{key}'가 이미 씬에서 삭제되었거나 null입니다.");
                    PlacedObjects.Remove(key);
                }
            }
        }
    }
    
    private void PlaceLandObject(LandObject landObject)
    {
        var index = landObject.objectIndex;
        if (index < 0 || index >= prefab.objectData.Count)
        {
            Debug.LogError($"인덱스 {index}가 범위를 벗어났습니다. prefab.objectData의 크기: {prefab.objectData.Count}");
            return;
        }
        GameObject obj = Instantiate(prefab.objectData[index].Prefab);

        obj.transform.position = landObject.position.ToVector3();
        obj.transform.eulerAngles = landObject.rotation.ToVector3();
        obj.transform.localScale = landObject.scale.ToVector3();

        PlacedObjects[landObject.key] = obj;
    }
}
