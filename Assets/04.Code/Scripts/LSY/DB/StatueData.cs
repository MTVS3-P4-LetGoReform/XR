using GLTFast;
using UnityEngine;

public class StatueData
{
    public string modelId;
    public Sprite modelSprite;
    public GltfImport modelGltf;

    public StatueData(string mId, Sprite sprite, GltfImport gltfImport)
    {
        modelId = mId;
        modelSprite = sprite;
        modelGltf = gltfImport;
    }
}