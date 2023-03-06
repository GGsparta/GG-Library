using System;
using System.Globalization;
using System.Text.RegularExpressions;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace GGL.Events
{
    /// <summary>
    /// Converts an object event in a formatted and pre-computed string.
    /// </summary>
    [AddComponentMenu(Settings.NAME + "/Events/Format Event")]
    public class FormatEvent : MonoBehaviour
    {
        /// <summary>
        /// Wether or not Unity shoud pre-compute the given value to an expression
        /// </summary>
        [Tooltip("Wether or not Unity shoud pre-compute the given value to an expression")]
        [Label("Pre-compute Expression")]
        public bool computeExpression;

        /// <summary>
        /// Formattable string to compute with thegiven value.
        /// </summary>
        /// <remarks>Do not forget to put '{0}'</remarks>
        [Tooltip("Formattable string to compute with thegiven value. Do not forget to put '{0}'")]
        [ShowIf(nameof(computeExpression))] public string expression = "{0} * 100";

        /// <value>
        /// String format.
        /// </value>
        /// <remarks>Do not forget to put '{0}'</remarks>
        [Tooltip("String format. Do not forget to put '{0}'")]
        public string format = "{0}";
        
        [SerializeField]
        [Tooltip("Event with the formatted output. Send it to your fav text displayer :p")]
        private UnityEvent<string> onFormat;

        /// <value>
        /// Event triggered with the formatted output.
        /// </value>
        public UnityEvent<string> OnFormat => onFormat;


        /// <summary>
        /// Format a value as defined by format.
        /// </summary>
        /// <param name="value"></param>
        public string Format(object value)
        {
            if (computeExpression)
            {
                if (Regex.IsMatch(expression, @".*\{0([,:]{1}.*)?\}.*"))
                {
                    switch (value)
                    {
                        case int:
                            ExpressionEvaluator.Evaluate(string.Format(CultureInfo.InvariantCulture, expression, value), out int i);
                            value = i;
                            break;
                        case float:
                            ExpressionEvaluator.Evaluate(string.Format(CultureInfo.InvariantCulture, expression, value), out float f);
                            value = f;
                            break;
                        case long:
                            ExpressionEvaluator.Evaluate(string.Format(CultureInfo.InvariantCulture, expression, value), out long l);
                            value = l;
                            break;
                        case double:
                            ExpressionEvaluator.Evaluate(string.Format(CultureInfo.InvariantCulture, expression, value), out double d);
                            value = d;
                            break;
                    }
                }
                else
                    Debug.LogWarning("Bad expresion format will be ignored:\n" + expression);
            }

            if (!Regex.IsMatch(format, @".*\{0([,:]{1}.*)?\}.*"))
            {
                Debug.LogError("Bad format!\n" + format);
                return string.Empty;
            }

            string result = string.Format(format, value);
            onFormat.Invoke(result);
            return result;
        }

        /// <inheritdoc cref="Format(object)"/>
        public void Format(float value) => Format((object)value);

        /// <inheritdoc cref="Format(object)"/>
        public void Format(double value) => Format((object)value);

        /// <inheritdoc cref="Format(object)"/>
        public void Format(string value) => Format((object)value);

        /// <inheritdoc cref="Format(object)"/>
        public void Format(int value) => Format((object)value);

        /// <inheritdoc cref="Format(object)"/>
        public void Format(long value) => Format((object)value);

        /// <inheritdoc cref="Format(object)"/>
        public void Format(bool value) => Format((object)value);

        /// <inheritdoc cref="Format(object)"/>
        public void Format(byte value) => Format(Convert.ToInt32(value));

        /// <inheritdoc cref="Format(object)"/>
        public void Format(char value) => Format((object)value);

        /// <inheritdoc cref="Format(object)"/>
        public void Format(decimal value) => Format(Convert.ToDouble(value));

        /// <inheritdoc cref="Format(object)"/>
        public void Format(ulong value) => Format(Convert.ToInt32(value));

        /// <inheritdoc cref="Format(object)"/>
        public void Format(uint value) => Format(Convert.ToInt32(value));

        /// <inheritdoc cref="Format(object)"/>
        public void Format(ushort value) => Format(Convert.ToInt32(value));

        /// <inheritdoc cref="Format(object)"/>
        public void Format(short value) => Format(Convert.ToInt32(value));
    }
}