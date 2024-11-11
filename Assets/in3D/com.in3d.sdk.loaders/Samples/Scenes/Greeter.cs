using System.IO;
using com.in3d.sdk.import.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.in3d.sdk.loaders.Samples.Scenes
{
    [InitializeOnLoad]
    public class Greeter
    {
        static Greeter () {
            EditorSceneManager.sceneOpened += OnSceneOpened;
        }

        private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            switch (scene.name)
            {
                case "AvatarLoading_EmptyScene":
                    // ShowMessage("com.in3d.sdk.loaders/Samples/Scenes/GreetingsMessage.txt");
                    break;
                case "Macarena":
                    // ShowMessage("com.in3d.sdk.loaders/Samples/Scenes/IslandVille/GreetingsMessage.txt");
                    ReimportModels();
                    break;
            }
        }
        
        private static void ReimportModels()
        {
            AssetDatabase.ImportAsset("Assets/in3D/com.in3d.sdk.loaders/Samples/Scenes/Macarena/Models/scene.gltf", ImportAssetOptions.ForceUpdate);
            AssetDatabase.ImportAsset("Assets/in3D/com.in3d.sdk.loaders/Samples/DefaultAvatar/model_T_regina.glb", ImportAssetOptions.ForceUpdate);
        }

        private static void ShowMessage(string filename)
        {
            var lockFile = Path.Combine(Application.persistentDataPath, filename.Replace("/", "."));
            if (File.Exists(lockFile)) return;
            File.Create(lockFile);
            
            var path = Path.Combine(Application.dataPath, filename);
            if (!File.Exists(path)) return;
            
            GreetingsWindow.ShowWindow("In3D Greetings", File.ReadAllText(path));
        }
    }
}