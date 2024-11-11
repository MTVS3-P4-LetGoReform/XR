#if IN3D_SDK_DEPENDENCIES_RESOLVED

using com.in3d.sdk.import.Credentials;
using UnityEditor;
using UnityEngine;

namespace com.in3d.sdk.import.Editor
{
    [CustomEditor(typeof(AccessKeyCredentials))]
    public class CredentialsEditor : UnityEditor.Editor
    {
        private CredentialsBase Credentials => (CredentialsBase)target;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Save as asset"))
            {
                Credentials.SaveToEditor();
            }
        }
    }
}

#endif