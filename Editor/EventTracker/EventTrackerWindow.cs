using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace GGL.Editor.EventTracker
{
    public class EventTrackerWindow : BaseWindow<EventTrackerWindow>
    {
        private const int MAX_ELEMENTS = 300;
        private const int TABULATION = 30;
        private const int MIN_CHARS_IN_SEARCH = 3;
        private const float LEFT_COLUMN_RELATIVE_WIDTH = 0.6f;

        private static List<EventReferenceInfo> _dependencies;
        private static string _searchString = "";
        private static string _currentSearch = "";

        private Vector2 _scroll = Vector2.zero;

        [MenuItem("Tools/GG-Library/Track events", false, 30)]
        public static void Open() => Open("Event Tracker");

        [DidReloadScripts]
        private static void RefreshDependencies() => FindDependencies(_searchString);


        private void OnEnable() => FindDependencies(_searchString);
        private void OnDisable() => _dependencies = null;


        protected override void Render(Vector2 pos, Vector2 size)
        {
            int drawnVertically = 0;

            string oldSearch = _searchString;

            EditorGUI.LabelField(new Rect(5, 8, 100, 16), " Search method");
            _searchString =
                EditorGUI.TextField(new Rect(110, 10, size.x - 120, 16),
                    _searchString);

            if (!_searchString.Equals(oldSearch))
            {
                _currentSearch = _searchString;
                FindDependencies(_searchString);
            }

            if (_searchString.Length >= MIN_CHARS_IN_SEARCH && _dependencies is { Count: > 0 and <= MAX_ELEMENTS })
            {
                GUILayout.Space(pos.x + 2);
                _scroll = GUILayout.BeginScrollView(_scroll, false, false);

                foreach (EventReferenceInfo d in _dependencies)
                {
                    drawnVertically += DrawDependency(d, new Rect(pos + Vector2.up * drawnVertically, size));
                    drawnVertically += ITEM_SPACING;
                }

                GUILayout.Space(drawnVertically + 40);
                GUILayout.EndScrollView();
            }
            else
            {
                string message;
                if (_searchString.Length < MIN_CHARS_IN_SEARCH)
                    message = $"Search your method in the field above :D";
                else if (_dependencies == null || _dependencies.Count == 0)
                    message = $"No results found for '{_currentSearch}'";
                else
                    message = $"{_dependencies.Count} results.\nTry to search for a more specific method!";

                EditorGUI.LabelField(new Rect(pos, size), message, new GUIStyle
                {
                    normal = new GUIStyleState
                    {
                        textColor = Color.white
                    },
                    alignment = TextAnchor.MiddleCenter
                });
            }
        }

        private int DrawDependency(EventReferenceInfo dependency, Rect depPos)
        {
            const int PADDING = 5;
            Vector2Int objectSize = new((int)(depPos.width * LEFT_COLUMN_RELATIVE_WIDTH - TABULATION), 16);

            int
                finalHeight = (objectSize.y + PADDING) * (dependency.Listeners.Count + 1) + PADDING,
                textX = objectSize.x + TABULATION,
                textWidth = (int)(depPos.width * (1 - LEFT_COLUMN_RELATIVE_WIDTH) - TABULATION - PADDING * 2);


            EditorGUI.DrawRect(new Rect(depPos.position, new Vector2(depPos.width, finalHeight)), COLOR_ITEM);

            depPos.position += Vector2.one * PADDING;
            depPos.size = new Vector2(depPos.width - PADDING * 2, finalHeight - PADDING * 2);


            EditorGUI.ObjectField(new Rect(depPos.position + Vector2.one, objectSize), dependency.Owner,
                typeof(MonoBehaviour), true);

            depPos.position += Vector2.right * TABULATION;
            for (int i = 0; i < dependency.Listeners.Count; i++)
            {
                depPos.position += Vector2.up * (objectSize.y + PADDING);
                EditorGUI.ObjectField(
                    new Rect(depPos.position, objectSize),
                    dependency.Listeners[i], typeof(MonoBehaviour), true);

                EditorGUI.LabelField(
                    new Rect(new Vector2(textX, depPos.position.y), new Vector2(textWidth, objectSize.y)),
                    dependency.MethodNames[i]);
            }

            return finalHeight;
        }

        private static void FindDependencies(string methodName)
        {
            if (string.IsNullOrEmpty(methodName))
            {
                _dependencies = EventTracker.FindAllUnityEventsReferences();
                return;
            }

            List<EventReferenceInfo> depens = EventTracker.FindAllUnityEventsReferences();
            List<EventReferenceInfo> onlyWithName = new();

            foreach (EventReferenceInfo d in depens)
            {
                if (!d.MethodNames.Any(m => m.ToLower().Contains(methodName.ToLower()))) continue;

                int[] indexes = d.MethodNames.Where(n => n.ToLower().Contains(methodName.ToLower()))
                    .Select(n => d.MethodNames.IndexOf(n)).ToArray();

                EventReferenceInfo info = new() { Owner = d.Owner };

                foreach (int i in indexes)
                {
                    info.Listeners.Add(d.Listeners[i]);
                    info.MethodNames.Add(d.MethodNames[i]);
                }

                onlyWithName.Add(info);
            }

            _dependencies = onlyWithName;
        }
    }
}