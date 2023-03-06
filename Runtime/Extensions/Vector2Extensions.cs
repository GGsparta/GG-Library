using GGL.Math;
using UnityEngine;

namespace GGL.Extensions
{
    public enum RotationType
    {
        /// <summary>
        /// Degrees
        /// </summary>
        DEG, 
        /// <summary>
        /// Radians
        /// </summary>
        RAD
    }

    public static class Vector2Extensions {
        /// <summary>
        /// Rotate a vector by an angle.
        /// </summary>
        public static Vector2 Rotate(this Vector2 vector, float angle, RotationType mode = RotationType.DEG)
        {
            if (mode == RotationType.DEG)
                angle *= Mathf.Deg2Rad;
            
            float sin = Mathf.Sin(angle);
            float cos = Mathf.Cos(angle);
         
            float tx = vector.x;
            float ty = vector.y;
            vector.x = cos * tx - sin * ty;
            vector.y = sin * tx + cos * ty;
            return vector;
        }
        
        /// <summary>
        /// Project a vector to a line
        /// </summary>
        /// <returns>Vector that is part of the line</returns>
        public static Vector2 ProjectToLine(this Vector2 vector, Line2D line) 
            => line.Point + line.Coefficient * Vector2.Dot(vector - line.Point, line.Coefficient);
    }
}