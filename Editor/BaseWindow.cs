using GGL.Extensions;
using UnityEditor;
using UnityEngine;

namespace GGL.Editor
{
    public abstract class BaseWindow<T> : EditorWindow where T : BaseWindow<T>
    {
        public static void Open(string title) => GetWindow<T>(title);

        protected static readonly Color 
            COLOR_HEADER = new(0.235f, 0.235f, 0.235f),
            COLOR_BACKGROUND = new(0.2f, 0.2f, 0.2f),
            COLOR_ITEM = new(0.25f, 0.25f, 0.25f);
        
        protected const int 
            ITEM_SPACING = 5;

        private Sprite _icon;
        private GUIStyle _style;

        private void Awake()
        {
            _icon = AssetDatabase.LoadAssetAtPath<Sprite>(
                AssetDatabase.GUIDToAssetPath("657b1e4b60de18a419b61015f50b77e9"));
        }

        protected void OnGUI()
        {
            const int
                HEADER_HEIGHT = 35,
                BOTTOM_PADDING = 16;

            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height), new GUIStyle
            {
                normal = new GUIStyleState
                {
                    background = new Texture2D(2, 2).Paint(COLOR_HEADER),
                }
            });

            GUILayout.BeginArea(new Rect(0, 0, Screen.width, HEADER_HEIGHT));
            GUILayout.BeginHorizontal();
            GUILayout.Box(new GUIContent(_icon.texture), new GUIStyle
            {
                fixedWidth = HEADER_HEIGHT,
                fixedHeight = HEADER_HEIGHT,
                padding = new RectOffset(5, 5, 5, 5)
            });
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.Label(titleContent.text, new GUIStyle
            {
                fontSize = 20,
                padding = new RectOffset(5, 5, 5, 5),
                normal = new GUIStyleState
                {
                    textColor = Color.white
                },
                fontStyle = FontStyle.Bold
            });
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            Vector2
                pos = new(0, HEADER_HEIGHT),
                size = new(Screen.width, Screen.height - HEADER_HEIGHT - BOTTOM_PADDING);
            GUILayout.BeginArea(
                new Rect(pos, size),
                new GUIStyle
                {
                    normal = new GUIStyleState
                    {
                        background = new Texture2D(2, 2).Paint(COLOR_BACKGROUND),
                    }
                });
            Render(pos, size);
            GUILayout.EndArea();
            GUILayout.EndArea();
        }

        protected abstract void Render(Vector2 pos, Vector2 size);
    }
}