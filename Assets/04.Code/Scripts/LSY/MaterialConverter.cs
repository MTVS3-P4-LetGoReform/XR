using UnityEngine;

public static class MaterialConverter
{
    static void ChangeShader(GameObject obj, Shader shader)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError("MaterialConverter : Renderer is null");
            return;
        }

        Material targetMat = renderer.material;

        if (shader == null)
        {
            Debug.LogError($"MaterialConverter : {shader is null}");
            return;
        }

        targetMat.shader = shader;
        Debug.Log($"MaterialConverter : Shader changed");
    }
}