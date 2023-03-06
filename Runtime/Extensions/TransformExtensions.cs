using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GGL.Extensions
{
    public static class TransformExtensions
    {
        /// <summary>
        /// Reset a transform's position and rotation.
        /// </summary>
        /// <param name="transform"></param>
        public static void Reset(this Transform transform)
        {
            transform.localPosition = transform.localScale =Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        /// <summary>
        /// Return the <b>enabled</b> children of the transform.
        /// </summary>
        public static IEnumerable<Transform> GetChildren(this Transform transform)
        {
            Transform[] children = new Transform[transform.childCount];
            for (int index = 0; index < transform.childCount; ++index) 
                children[index] = transform.GetChild(index);
            return children;
        }

        /// <summary>
        /// Return the <b>enabled</b> sibling of the transform from its parent.
        /// </summary>
        public static IEnumerable<Transform> GetSiblings(this Transform transform, bool excludeSelf = true)
        {
            IEnumerable<Transform> siblings =  transform.parent ? transform.parent.GetChildren() : ArraySegment<Transform>.Empty;
            return excludeSelf ? siblings.Where(t => t != transform) : siblings;
        }
    }
}
