using System;
using UnityEditor;
using UnityEngine;

namespace Puppetry.Puppet.GUI
{
    public class UPathValidatorWindow : EditorWindow
    {
        string _upath = String.Empty;
        GameObject _foundGameObject = null;

        [MenuItem("Puppetry/UPath Validator")]
        public static void ShowWindow()
        {
            var w = GetWindow<UPathValidatorWindow>();
            w.titleContent = new GUIContent("UPath Validator");
        }

        void OnGUI()
        {
            GUILayout.Label ("Upath Validator Window", EditorStyles.boldLabel);
            _upath = EditorGUILayout.TextField ("UPath", _upath);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Find Game Object"))
            {
                if (string.IsNullOrEmpty(_upath))
                    ShowNotification(new GUIContent("Upath can't be null or empty"));
                else
                {
                    try
                    {
                        _foundGameObject = FindGameObjectHelper.FindGameObjectByUPath(_upath);
                    }
                    catch (Exception)
                    {
                        ShowNotification(new GUIContent("UPath expression is invalid"));
                        throw;
                    }

                    if (_foundGameObject == null)
                        ShowNotification(new GUIContent("There is no GameObject by this upath"));
                    else
                        Selection.activeGameObject = _foundGameObject;
                }
            }
            if (GUILayout.Button("Count"))
            {
                if (string.IsNullOrEmpty(_upath))
                    ShowNotification(new GUIContent("Upath can't be null or empty"));
                else
                {
                    try
                    {
                        ShowNotification(new GUIContent("There are " + FindGameObjectHelper.FindGameObjectsByUPath(_upath).Count + " GameObjects with this upath"));
                    }
                    catch (Exception)
                    {
                        ShowNotification(new GUIContent("UPath expression is invalid"));
                        throw;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}