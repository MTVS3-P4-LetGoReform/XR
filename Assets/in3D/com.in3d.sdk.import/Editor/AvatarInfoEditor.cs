#if IN3D_SDK_DEPENDENCIES_RESOLVED

using System;
using System.Collections.Generic;
using com.in3d.sdk.import.Credentials;
using UnityEditor;
using UnityEngine;

namespace com.in3d.sdk.import.Editor
{
    [CustomEditor(typeof(AvatarInfo))]
    public class AvatarInfoEditor : UnityEditor.Editor
    {
        protected AvatarInfo Avatar => (AvatarInfo)target;

        private Dictionary<ModelFormat, bool> _foldouts = new Dictionary<ModelFormat, bool>();

        private AccessKeyCredentials _creds;

        public override void OnInspectorGUI()
        {
            EditorGUILayout.TextField("Id", Avatar.Id.ToString());

            foreach (var format in (ModelFormat[])Enum.GetValues(typeof(ModelFormat)))
            {
                if (!_foldouts.ContainsKey(format)) _foldouts.Add(format, false);

                _foldouts[format] = EditorGUILayout.BeginFoldoutHeaderGroup(_foldouts[format], format.ToString(),
                                                                            EditorStyles.foldout);
                if (_foldouts[format])
                {
                    Avatar[format].PreviewUrl = new Uri(EditorGUILayout.TextField("Preview URL", Avatar[format].PreviewUrl.ToString()));
                    Avatar[format].ModelUrl = new Uri(EditorGUILayout.TextField("Model URL", Avatar[format].ModelUrl.ToString()));
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
            }
            if (GUILayout.Button("Save as asset"))
            {
                Avatar.SaveToEditor();
            }

            if (GUILayout.Button("Download fbx"))
            {
#pragma warning disable CS4014
                Avatar.SaveFbxToEditorAsync();
#pragma warning restore CS4014
            }

            if (GUILayout.Button("Download glb"))
            {
#pragma warning disable CS4014
                Avatar.SaveGlbToEditorAsync();
#pragma warning restore CS4014
            }
        }
    }
}

#endif