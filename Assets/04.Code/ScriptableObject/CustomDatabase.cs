using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomDatabase", menuName = "Scriptable Objects/NewScriptableObjectScript", order = 2)]
public class CustomDatabase : ScriptableObject
{
    [SerializeField]
    public List<CustomData> customData;
}

[Serializable]
public class CustomData
{
    [field:SerializeField]
    public string Name { get; private set; }
    [field:SerializeField]
    public int ID { get; private set; }
    [field:SerializeField]
    public Vector2Int Size { get; private set; } = Vector2Int.one;
    [field:SerializeField]
    public GameObject PartObject { get; private set; }
}