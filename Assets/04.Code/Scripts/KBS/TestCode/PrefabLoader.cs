using UnityEditor;
using UnityEngine;

public class PrefabLoader : MonoBehaviour
{
   public void LoadPrefab()
   {
      string prefabPath = "Assets/03.ThirdParty/UserAssets/Layer lab/3D Casual Character/Prefabs/Characters.prefab";

      GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

      if (prefab != null)
      {
         GameObject instance = Instantiate(prefab);
         instance.name = "tlqkfsus";
      }
      else
      {
         Debug.LogError("프리팹이 없다 시벌");
      }
   }
}
