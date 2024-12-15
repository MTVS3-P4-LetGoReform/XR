using GLTFast;
using UnityEngine;

public class StatueData
{
    public string modelId;
    public string imageName;
    public string modelName;
    public Sprite modelSprite;
    public GltfImport modelGltf;
    public string creatorId;

    public StatueData(string mId, string imageName, string modelName, Sprite sprite, GltfImport gltfImport, string creatorId)
    {
        modelId = mId;
        imageName = this.imageName;
        modelName = this.modelName;
        modelSprite = sprite;
        modelGltf = gltfImport;
        creatorId = this.creatorId;
    }
}