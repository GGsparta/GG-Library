using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GGL.Editor.EmptyFolderCleaner
{
    public class EmptyFolderCleanerWindow : BaseWindow<EmptyFolderCleanerWindow>
    {
        private List<DirectoryInfo> _emptyDirs;
        private Vector2 _scrollPosition;
        private bool _lastCleanOnSave;
        private string _delayedNotiMsg;
        private GUIStyle _updateMsgStyle;

        private bool HasNoEmptyDir => _emptyDirs == null || _emptyDirs.Count == 0;

        private const float DIR_LABEL_HEIGHT = 21;

        [MenuItem("Tools/GG-Library/Clean empty folders", false, 100)]
        public static void Open() => Open("Empty Folder Cleaner");

        private void OnEnable()
        {
            _lastCleanOnSave = EmptyFolderCleaner.CleanOnSave;
            EmptyFolderCleaner.OnAutoClean += Core_OnAutoClean;
            _delayedNotiMsg = "Click 'Find Empty Dirs' Button.";
        }

        private void OnDisable()
        {
            EmptyFolderCleaner.CleanOnSave = _lastCleanOnSave;
            EmptyFolderCleaner.OnAutoClean -= Core_OnAutoClean;
        }

        private void Core_OnAutoClean() => _delayedNotiMsg = "Cleaned on Save";


        protected override void Render(Vector2 pos, Vector2 size)
        {
            if (_delayedNotiMsg != null)
            {
                ShowNotification(new GUIContent(_delayedNotiMsg));
                _delayedNotiMsg = null;
            }

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Find Empty Dirs"))
                    {
                        EmptyFolderCleaner.FillEmptyDirList(out _emptyDirs);

                        if (HasNoEmptyDir) ShowNotification(new GUIContent("No Empty Directory"));
                        else RemoveNotification();
                    }


                    if (ColorButton("Delete All", !HasNoEmptyDir, Color.red))
                    {
                        EmptyFolderCleaner.DeleteAllEmptyDirAndMeta(ref _emptyDirs);
                        ShowNotification(new GUIContent("Deleted All"));
                    }
                }
                EditorGUILayout.EndHorizontal();


                bool cleanOnSave = GUILayout.Toggle(_lastCleanOnSave, " Clean Empty Dirs Automatically On Save");
                if (cleanOnSave != _lastCleanOnSave)
                {
                    _lastCleanOnSave = cleanOnSave;
                    EmptyFolderCleaner.CleanOnSave = cleanOnSave;
                }

                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

                if (!HasNoEmptyDir)
                {
                    _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandWidth(true));
                    {
                        EditorGUILayout.BeginVertical();
                        {
                            GUIContent folderContent = EditorGUIUtility.IconContent("Folder Icon");

                            foreach (DirectoryInfo dirInfo in from dirInfo in _emptyDirs
                                     let assetObj = AssetDatabase.LoadAssetAtPath("Assets", typeof(Object))
                                     where null != assetObj
                                     select dirInfo)
                            {
                                folderContent.text =
                                    EmptyFolderCleaner.GetRelativePath(dirInfo.FullName, Application.dataPath);
                                GUILayout.Label(folderContent, GUILayout.Height(DIR_LABEL_HEIGHT));
                            }
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndScrollView();
                }
            }
            EditorGUILayout.EndVertical();
        }

        private bool ColorButton(string label, bool enabled, Color color)
        {
            bool oldEnabled = GUI.enabled;
            Color oldColor = GUI.color;

            GUI.enabled = enabled;
            GUI.color = color;

            bool ret = GUILayout.Button(label);

            GUI.enabled = oldEnabled;
            GUI.color = oldColor;

            return ret;
        }
    }
}