using UnityEngine;

namespace GGL.Math
{
    /// <summary>
    /// Model to define a line in 2D.
    /// </summary>
    public class Line2D
    {
        /// <value>
        /// A point where the line goes through.
        /// </value>
        public Vector2 Point { get; private set; }
        
        /// <value>
        /// Direction followed by the line. 
        /// </value>
        public Vector2 Coefficient { get; private set; }

        /// <summary>
        /// Create a 2D line from a point and a coefficient.
        /// </summary>
        public static Line2D CreateFromCoefficient(Vector2 point, Vector2 coef) => new()
        {
            Point = point,
            Coefficient = coef.normalized
        };

        /// <summary>
        /// Create a 2D line from 2 points.
        /// </summary>
        public static Line2D CreateFromPoints(Vector2 p1, Vector2 p2) => new()
        {
            Coefficient = (p2 - p1).normalized,
            Point = p1
        };
    }
}