using System.Collections.Generic;
using UnityEngine;

public class LandObjectController : MonoBehaviour
{
    [SerializeField] private ObjectDatabase prefabDatabase;
    public static Dictionary<string, GameObject> _placedObjects = new Dictionary<string, GameObject>();

    public void UpdateObjects(List<LandObject> objects)
    {
        if (objects == null) return;

        UpdateExistingObjects(objects);
        RemoveDeletedObjects(objects);
    }
    
    public static void AddPlacedObject(string key, GameObject obj)
    {
        if (_placedObjects.ContainsKey(key))
        {
            Debug.LogWarning($"Object with key {key} already exists. Updating instead.");
            _placedObjects[key] = obj;
        }
        else
        {
            _placedObjects.Add(key, obj);
        }
    }


    private void UpdateExistingObjects(List<LandObject> objects)
    {
        foreach (var landObject in objects)
        {
            if (string.IsNullOrEmpty(landObject.key)) continue;

            if (_placedObjects.TryGetValue(landObject.key, out GameObject existingObject))
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
            _placedObjects.Remove(landObject.key);
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

        GameObject newObject = Instantiate(prefabDatabase.objectData[landObject.objectIndex].Prefab);
        UpdateObjectTransform(newObject, landObject);
        _placedObjects[landObject.key] = newObject;
    }

    private void RemoveDeletedObjects(List<LandObject> currentObjects)
    {
        var keysToRemove = new List<string>(_placedObjects.Keys);
        foreach (var key in keysToRemove)
        {
            if (!currentObjects.Exists(obj => obj.key == key))
            {
                if (_placedObjects.TryGetValue(key, out GameObject obj))
                {
                    Destroy(obj);
                    _placedObjects.Remove(key);
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