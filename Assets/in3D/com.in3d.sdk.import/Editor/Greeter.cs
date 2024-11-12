using System.IO;
using UnityEditor;
using UnityEngine;

namespace com.in3d.sdk.import.Editor
{
#if IN3D_SDK_DEPENDENCIES_RESOLVED
    [InitializeOnLoad]
    public static class Greeter
    {
        static Greeter()
        {
            var lockFile = Path.Combine(Application.persistentDataPath, "in3d_package_greeter.txt");
            if (File.Exists(lockFile)) return;
            File.Create(lockFile);

            var filename = "in3D/com.in3d.sdk.import/GreetingsMessage.txt";
            var path = Path.Combine(Application.dataPath, filename);
            if (!File.Exists(path)) return;

            GreetingsWindow.ShowWindow("In3D Greetings", File.ReadAllText(path));
        }
    }
#endif
}