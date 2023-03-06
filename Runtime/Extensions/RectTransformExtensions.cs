using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GGL.Extensions
{
    public static class RectTransformExtensions
    {
        #region RESET
        /// <summary>
        /// Reset the scale of a <see cref="RectTransform"/>
        /// </summary>
        public static void ResetScale(this RectTransform rectTransform)
        {
            rectTransform.localScale = new Vector3(1, 1, 1);
        }

        /// <summary>
        /// Reset the position of a <see cref="RectTransform"/>
        /// </summary>
        public static void ResetTransform(this RectTransform rectTransform)
        {
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localEulerAngles = Vector3.zero;
            rectTransform.sizeDelta = Vector2.zero;
        }

        /// <summary>
        /// Reset the offsets of a <see cref="RectTransform"/>
        /// </summary>
        public static void ResetRect(this RectTransform rectTransform)
        {
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        /// <summary>
        /// Reset the anchors of a <see cref="RectTransform"/>
        /// </summary>
        public static void ResetAnchors(this RectTransform rectTransform)
        {
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchorMin = Vector2.zero;
        }

        /// <summary>
        /// Reset a <see cref="RectTransform"/> (scale + position + anchors + offsets)
        /// </summary>
        public static void Reset(this RectTransform rectTransform)
        {
            rectTransform.ResetAnchors();
            rectTransform.ResetTransform();
            rectTransform.ResetRect();
            rectTransform.ResetScale();
        }
        #endregion


        #region SIZE
        /// <summary>
        /// Return the size of a <see cref="RectTransform"/>'s rect.
        /// </summary>
        /// <returns><see cref="Vector2"/> representing (width, height)</returns>
        public static Vector2 GetSize(this RectTransform rectTransform) => rectTransform.rect.size;

        /// <summary>
        /// Return the width of a <see cref="RectTransform"/>'s rect.
        /// </summary>
        public static float GetWidth(this RectTransform rectTransform) => rectTransform.rect.width;

        /// <summary>
        /// Return the height of a <see cref="RectTransform"/>'s rect.
        /// </summary>
        public static float GetHeight(this RectTransform rectTransform) => rectTransform.rect.height;

        /// <summary>
        /// Set the size of a <see cref="RectTransform"/>'s rect.
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="newSize"><see cref="Vector2"/> representing (width, height)</param>
        public static void SetSize(this RectTransform rectTransform, Vector2 newSize)
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newSize.x);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newSize.y);
        }

        /// <summary>
        /// Set the width of a <see cref="RectTransform"/>'s rect.
        /// </summary>
        public static void SetWidth(this RectTransform rectTransform, float width) =>
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

        /// <summary>
        /// Set the height of a <see cref="RectTransform"/>'s rect.
        /// </summary>
        public static void SetHeight(this RectTransform rectTransform, float height) =>
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        #endregion


        #region POSITION
        /// <summary>
        /// Set the pivot of a <see cref="RectTransform"/>'s rect.
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="newPivot">This is the local position. Differs from the anchored position.</param>
        public static void SetPivot(this RectTransform rectTransform, Vector2 newPivot) => 
            rectTransform.localPosition = new Vector3(newPivot.x, newPivot.y, rectTransform.localPosition.z);

        /// <summary>
        /// Set the bottom left corner position of a <see cref="RectTransform"/>'s rect.
        /// </summary>
        public static void SetBottomLeftPosition(this RectTransform rectTransform, Vector2 newPos)
        {
            rectTransform.localPosition = new Vector3(newPos.x + rectTransform.pivot.x * rectTransform.rect.width,
                newPos.y + rectTransform.pivot.y * rectTransform.rect.height,
                rectTransform.localPosition.z);
        }

        /// <summary>
        /// Set the top left corner position of a <see cref="RectTransform"/>'s rect.
        /// </summary>
        public static void SetTopLeftPosition(this RectTransform rectTransform, Vector2 newPos)
        {
            rectTransform.localPosition = new Vector3(newPos.x + rectTransform.pivot.x * rectTransform.rect.width,
                newPos.y - (1f - rectTransform.pivot.y) * rectTransform.rect.height,
                rectTransform.localPosition.z);
        }

        /// <summary>
        /// Set the bottom right corner position of a <see cref="RectTransform"/>'s rect.
        /// </summary>
        public static void SetBottomRightPosition(this RectTransform rectTransform, Vector2 newPos)
        {
            rectTransform.localPosition = new Vector3(
                newPos.x - (1f - rectTransform.pivot.x) * rectTransform.rect.width,
                newPos.y + rectTransform.pivot.y * rectTransform.rect.height,
                rectTransform.localPosition.z);
        }

        /// <summary>
        /// Set the top right corner position of a <see cref="RectTransform"/>'s rect.
        /// </summary>
        public static void SetTopRightPosition(this RectTransform rectTransform, Vector2 newPos)
        {
            rectTransform.localPosition = new Vector3(
                newPos.x - (1f - rectTransform.pivot.x) * rectTransform.rect.width,
                newPos.y - (1f - rectTransform.pivot.y) * rectTransform.rect.height,
                rectTransform.localPosition.z);
        }
        #endregion


        #region TRANSPOSITION
        /// <summary>
        /// <see cref="RectTransformUtility.ScreenPointToLocalPointInRectangle"/> but the camera is automatically detected.
        /// </summary>
        public static bool ScreenPointToLocalPointInRectangle(this RectTransform rectTransform, Vector2 screenPoint,
            out Vector2 result)
            => RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint,
                rectTransform.GetCanvasCamera(), out result);

        
        /// <summary>
        /// Just as <see cref="ScreenPointToLocalPointInRectangle"/> but the point will be extracted
        /// from rect to be considered as position in <see cref="RectTransform"/>'s parent.
        /// </summary>
        public static bool ScreenPointToLocalPosition(this RectTransform rectTransform, Vector2 screenPoint,
            out Vector2 result)
        {
            if (!rectTransform.ScreenPointToLocalPointInRectangle(screenPoint, out Vector2 localPoint))
            {
                result = Vector3.zero;
                return false;
            }

            result = ConvertLocalPointToPosition(rectTransform, localPoint);
            return true;
        }

        /// <summary>
        /// Extracts from a local point in a <see cref="RectTransform"/>'s rect its local position in the parent.
        /// </summary>
        public static Vector2 ConvertLocalPointToPosition(this RectTransform rectTransform, Vector3 localPoint)
        {
            Vector2 rectSize = rectTransform.rect.size;
            Vector2 invRectSize = new(1f / rectSize.x, 1f / rectSize.y);
            return Vector2.Scale(invRectSize, localPoint) + rectTransform.pivot;
        }

        /// <summary>
        /// Extracts from a local position its local point in a <see cref="RectTransform"/>'s rect.
        /// </summary>
        public static Vector2 ConvertPositionToLocalPoint(this RectTransform rectTransform, Vector3 worldPoint)
            => ConvertLocalPointToPosition(rectTransform, rectTransform.InverseTransformPoint(worldPoint));
        #endregion


        #region EXTRACTION
        /// <summary>
        /// Returns the root canvas of a <see cref="RectTransform"/>.
        /// </summary>
        public static Canvas GetRootCanvas(this RectTransform rectTransform)
        {
            Canvas canvas = rectTransform.GetComponentInParent<Canvas>();
            if (canvas == null)
                return null;
            return canvas.isRootCanvas ? canvas : canvas.rootCanvas;
        }

        
        /// <summary>
        /// Returns the camera of the root canvas where belongs the <see cref="RectTransform"/>.
        /// </summary>
        public static Camera GetCanvasCamera(this RectTransform rectTransform) =>
            rectTransform.GetRootCanvas().GetCanvasCamera();

        
        /// <summary>
        /// Returns the 'screen space' rect that represents a <see cref="RectTransform"/>.
        /// </summary>
        public static Rect ToScreenRect(this RectTransform rt)
        {
            Canvas canvas = rt.GetRootCanvas();
            Camera cam = canvas.GetCanvasCamera();

            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);

            Vector2 min = RectTransformUtility.WorldToScreenPoint(cam, corners[0]);
            Vector2 max = min;
            for (int i = corners.Length - 1; i > 0; --i)
            {
                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, corners[i]);

                if (screenPoint.x < min.x)
                {
                    min.x = screenPoint.x;
                }
                else if (screenPoint.x > max.x)
                {
                    max.x = screenPoint.x;
                }

                if (screenPoint.y < min.y)
                {
                    min.y = screenPoint.y;
                }
                else if (screenPoint.y > max.y)
                {
                    max.y = screenPoint.y;
                }
            }

            return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
        }
        #endregion


        #region EDITOR
#if UNITY_EDITOR
        [MenuItem("CONTEXT/RectTransform/Reset RectTransform")]
        private static void ResetRectTransform()
        {
            if (Selection.activeObject is not GameObject gameObject) return;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            Undo.RecordObject(rectTransform, $"{nameof(RectTransformExtensions)} {nameof(ResetRectTransform)}");
            Reset(rectTransform);
        }
#endif
        #endregion
    }
}