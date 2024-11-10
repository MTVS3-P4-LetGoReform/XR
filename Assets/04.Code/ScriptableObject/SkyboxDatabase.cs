using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkyboxData", menuName = "Scriptable Objects/SkyboxData", order = 2)]
public class SkyboxDatabase : ScriptableObject
{
    [SerializeField] 
    public List<SkyboxData> skyboxData;
}

[Serializable]
public class SkyboxData
{
    [field : SerializeField]
    public string Name { get; private set; }
    [field: SerializeField]
    public int ID { get; private set; }
    [field: SerializeField]
    public Material ThemeSkybox { get; private set; }
}