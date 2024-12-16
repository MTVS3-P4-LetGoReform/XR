using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon.StructWrapping;
using Firebase.Database;
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
    public static async UniTask DeleteSelectedObject(string key)
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

            var userId = UserData.Instance.UserId;
        
            // objects 노드의 데이터를 가져옴
            var objectsRef = FirebaseDatabase.DefaultInstance
                .GetReference($"user_land/{userId}/objects");
            var snapshot = await objectsRef.GetValueAsync();

            
            // objects 아래의 각 인덱스를 순회하며 검사
            foreach (var child in snapshot.Children)
            {
                var objectData = child.Value as Dictionary<string, object>;
                if (objectData != null && objectData.ContainsKey("key") && objectData["key"].ToString() == key)
                {
                    // 찾은 인덱스의 데이터를 삭제
                    await objectsRef.Child(child.Key).RemoveValueAsync();
                    Debug.Log($"오브젝트 삭제 완료: index {child.Key}, key {key}");
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"오브젝트 삭제 실패: {e.Message}");
        }
    }




    private void UpdateExistingObjects(List<LandObject> objects)
    {
        if (objects == null) return;
    
        foreach (var landObject in objects)
        {
            // landObject 자체가 null인지 먼저 확인
            if (landObject == null) continue;
        
            // key 속성에 접근하기 전에 null 체크
            try 
            {
                string objectKey = landObject.key;
                if (string.IsNullOrEmpty(objectKey)) continue;

                if (PlacedObjects.TryGetValue(objectKey, out GameObject existingObject))
                {
                    UpdateObjectTransform(existingObject, landObject);
                }
                else
                {
                    CreateNewObject(landObject);
                }
            }
            catch (NullReferenceException)
            {
                Debug.LogWarning($"Invalid land object data encountered");
                continue;
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
        if (currentObjects == null) return;

        var keysToRemove = new List<string>(PlacedObjects.Keys);
        foreach (var key in keysToRemove)
        {
            bool exists = currentObjects.Any(obj => obj != null && obj.key == key);
            if (!exists)
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