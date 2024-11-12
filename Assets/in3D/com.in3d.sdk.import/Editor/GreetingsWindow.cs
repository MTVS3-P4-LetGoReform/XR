using UnityEditor;
using UnityEngine;

namespace com.in3d.sdk.import.Editor
{
    public class GreetingsWindow : EditorWindow
    {
        public static void ShowWindow(string title, string message)
        {
            var window = GetWindow<GreetingsWindow>();
            window.titleContent = new GUIContent(title);
            window.Show();
            window._message = message;
        }

        private string _message;

        private void OnGUI()
        {
            GUILayout.Label(_message, EditorStyles.wordWrappedLabel);
        }
    }
}