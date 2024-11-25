using System;
using System.IO;
using System.Threading.Tasks;
using GLTFast;
using UnityEngine;

public static class GltfLoader
{
    public static async Task<GltfImport> LoadGLTF(string filePath)
    {
        if (File.Exists(filePath))
        {
            // glTFast를 이용해 GLB 파일을 로드
            GltfImport gltfImport = new GltfImport();
            var success = await gltfImport.Load(filePath); // 비동기 로드 처리
            if (success)
            {
                return gltfImport;
            }
            else
            {
                Debug.LogError("Failed to load GLTF file: " + filePath);
                return null;
            }
        }
        else
        {
            Debug.LogError("File does not exist: " + filePath);
            return null;
        }
    }
}