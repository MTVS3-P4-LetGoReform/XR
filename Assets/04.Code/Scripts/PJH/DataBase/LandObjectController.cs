using System;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;

public class LandObjectController : MonoBehaviour
{
    [SerializeField] private ObjectDatabase prefabDatabase;
    public static Dictionary<string, GameObject> PlacedObjects = new Dictionary<string, GameObject>();

    public void UpdateObjects(List<LandObject> objects)
    {
        if (objects == null) return;

        UpdateExistingObjects(objects);
        RemoveDeletedObjects(objects);
    }
    
    public static void AddPlacedObject(string key, GameObject obj)
    {
        if (PlacedObjects.ContainsKey(key))
        {
            Debug.LogWarning($"Object with key {key} already exists. Updating instead.");
            PlacedObjects[key] = obj;
        }
        else
        {
            PlacedObjects.Add(key, obj);
        }
    }
    
    /// <summary>
    /// 선택한 오브젝트를 삭제합니다.
    /// </summary>
    /// <param name="key">삭제할 오브젝트의 키</param>
    public static async void DeleteSelectedObject(string key)
    {
        if (!PlacedObjects.TryGetValue(key, out GameObject obj))
        {
            Debug.LogWarning($"삭제할 오브젝트를 찾을 수 없습니다: {key}");
            return;
        }

        try
        {
            // 씬에서 오브젝트 제거
            Destroy(obj);
            PlacedObjects.Remove(key);

            // Firebase DB에서 오브젝트 삭제
            var userId = UserData.Instance.UserId;
            await RealtimeDatabase.DeleteDataAsync($"user_land/{userId}/objects/{key}");
            
            Debug.Log($"오브젝트 삭제 완료: {key}");
        }
        catch (Exception e)
        {
            Debug.LogError($"오브젝트 삭제 실패: {e.Message}");
        }
    }

    /// <summary>
    /// 영지정보를 업데이트 합니다.
    /// </summary>
    /// <param name="objects">DB에서 받아온 영지 정보</param>
    private void UpdateExistingObjects(List<LandObject> objects)
    {
        foreach (var landObject in objects)
        {
            if (landObject == null)
                return;
            
            if (string.IsNullOrEmpty(landObject.key)) 
                continue;

            if (PlacedObjects.TryGetValue(landObject.key, out GameObject existingObject))
            {
                UpdateObjectTransform(existingObject, landObject);
            }
            else
            {
                CreateNewObject(landObject);
            }
        }
    }

    private void UpdateObjectTransform(GameObject obj, LandObject landObject)
    {
        if (obj == null)
        {
            PlacedObjects.Remove(landObject.key);
            CreateNewObject(landObject);
            return;
        }

        obj.transform.position = landObject.position.ToVector3();
        obj.transform.rotation = Quaternion.Euler(landObject.rotation.ToVector3());
        obj.transform.localScale = landObject.scale.ToVector3();
    }

    private void CreateNewObject(LandObject landObject)
    {
        if (!ValidateObjectIndex(landObject.objectIndex)) return;

        Debug.Log(landObject.key);
        GameObject newObject = Instantiate(prefabDatabase.objectData[landObject.objectIndex].Prefab);
    
        // ObjectIdentifier 컴포넌트 추가 및 키 설정
        var identifier = newObject.GetComponent<ObjectIdentifier>();
        identifier.SetKey(landObject.key);
        
        UpdateObjectTransform(newObject, landObject);
        PlacedObjects[landObject.key] = newObject;
    }


    private void RemoveDeletedObjects(List<LandObject> currentObjects)
    {
        var keysToRemove = new List<string>(PlacedObjects.Keys);
        foreach (var key in keysToRemove)
        {
            if (!currentObjects.Exists(obj => obj.key == key))
            {
                if (PlacedObjects.TryGetValue(key, out GameObject obj))
                {
                    Destroy(obj);
                    PlacedObjects.Remove(key);
                }
            }
        }
    }

    private bool ValidateObjectIndex(int index)
    {
        if (index < 0 || index >= prefabDatabase.objectData.Count)
        {
            Debug.LogError($"유효하지 않은 오브젝트 인덱스: {index}");
            return false;
        }
        return true;
    }
}