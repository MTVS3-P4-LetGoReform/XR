using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectDatabase", menuName = "Scriptable Objects/ObjectDatabase", order = 1)]
public class ObjectDatabase : ScriptableObject
{
    [SerializeField]
    public List<ObjectData> objectData;
}

[Serializable]
public class ObjectData
{
    [field:SerializeField]
    public string Name { get; private set; }
    [field:SerializeField]
    public int ID { get; private set; }
    [field:SerializeField]
    public Vector2Int Size { get; private set; } = Vector2Int.one;
    [field:SerializeField]
    public GameObject Prefab { get; private set; }
    [field:SerializeField]
    public GameObject PreviewPrefab { get; private set; }
}
