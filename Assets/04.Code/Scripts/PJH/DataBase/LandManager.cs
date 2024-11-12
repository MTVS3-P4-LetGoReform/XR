using System.Collections.Generic;
using UnityEngine;

public class LandManager : MonoBehaviour
{
    public string userId = "TestUser"; // 유저 ID
    public ObjectDatabase prefab; // LandObject에 맞는 프리팹
    public static Dictionary<string, GameObject> PlacedObjects = new Dictionary<string, GameObject>(); // 이미 배치된 오브젝트 목록

    private void Start()
    {
        if (UserData.Instance != null)
        {
            userId = UserData.Instance.UserId;
        }
        
        RealtimeDatabase.ListenForUserLandChanges(userId, UpdateLandObjects, exception => Debug.LogError("실시간 데이터 수신 오류: " + exception.Message));
    }

    private void PlaceLandObject(LandObject landObject)
    {
        var index = landObject.objectIndex;
        GameObject obj = Instantiate(prefab.objectData[index].Prefab);

        obj.transform.position = landObject.position.ToVector3();
        obj.transform.eulerAngles = landObject.rotation.ToVector3();
        obj.transform.localScale = landObject.scale.ToVector3();

        PlacedObjects[landObject.key] = obj;
    }

    private void UpdateLandObjects(UserLand updatedUserLand)
    {
        if (updatedUserLand == null || updatedUserLand.objects == null)
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
                    //PlaceLandObject(landObject);
                }
            }
            else
            {
                Debug.Log("배치되었습니다."+landObject.key);
                //PlaceLandObject(landObject);
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
}
