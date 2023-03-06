using UnityEditor;
using UnityEngine;

namespace GGL.Editor.MissingReferencesTracker
{
    public class MissingReferencesWindow : BaseWindow<MissingReferencesWindow>
    {
        private static int _goCount, _componentsCount, _missingCount;

        [MenuItem("Tools/GG-Library/Track 'Missing scripts'", false, 30)]
        public static void Open() => Open("Missing References Tracker");

        public void OnEnable() => Selection.selectionChanged += Repaint;
        public void OnDisable() => Selection.selectionChanged -= Repaint;

        protected override void Render(Vector2 pos, Vector2 size)
        {
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (Selection.gameObjects.Length == 0)
                GUILayout.Label("Select some objects to check in your scene...");
            else if (GUILayout.Button($"Find 'Missing Scripts' in {Selection.gameObjects.Length} selected\nobjects and their children")) 
                MissingReferences.CheckMissingReferences(Selection.gameObjects);
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
        }
    }
}