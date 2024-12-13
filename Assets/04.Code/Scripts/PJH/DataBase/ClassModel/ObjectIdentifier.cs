using UnityEngine;

public class ObjectIdentifier : MonoBehaviour
{
    public string Key { get; private set; }

    public void SetKey(string key)
    {
        Key = key;
    }
}