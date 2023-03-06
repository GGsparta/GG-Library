using System;

namespace GGL.Observer
{
    /// <summary>
    /// Encapsulor that observes a value. 👀
    /// </summary>
    /// <typeparam name="T">Class or basic type.</typeparam>
    public class Observable<T>
    {
        /// <value>
        /// Even raised after changes. Define a listener as follow:
        /// </value>
        /// <code>(T newValue, T oldValue) => {...}</code>
        public event Action<T, T> OnChange;

        private T _value;
        
        /// <value>
        /// Sets the value and eventually raise event <see cref="OnChange"/>.
        /// </value>
        public T Value
        {
            get => _value;
            set
            {
                T oldValue = _value;
                _value = value;
                HasValue = _value != null;
                if(!Equals(value, oldValue)) OnChange?.Invoke(value, oldValue);
            }
        }

        /// <value>
        /// Is there a value set?
        /// </value>
        /// <remarks>Useful for basic types like integers...</remarks>
        public bool HasValue { get; private set; }

        /// <summary>
        /// Create an observable variable with no default value.
        /// </summary>
        public Observable() {}

        /// <summary>
        /// Create an observable variable with a default value.
        /// </summary>
        /// <param name="value">Base default value</param>
        public Observable(T value) => Value = value;
        
        /// <summary>
        /// Sets the value and eventually raise event <see cref="OnChange"/>.
        /// </summary>
        /// <param name="value">New value.</param>
        /// <param name="triggerEvent">Well... if you want to sneakily set a value, set this to false.</param>
        public void Set(T value, bool triggerEvent = true)
        {
            if (triggerEvent)
            {
                Value = value;
            }
            else
            {
                _value = value;
                HasValue = _value != null;
            }
        }

        /// <summary>
        /// Clear value and raise event <see cref="OnChange"/>.
        /// </summary>
        public void Clear()
        {
            T newValue = default, oldValue = _value;
            _value = newValue;
            HasValue = false;
            OnChange?.Invoke(newValue, oldValue);
        }

        /// <summary>
        /// Raise event <see cref="OnChange"/>.
        /// </summary>
        /// <remarks><b>The old value in the event will be the same as the new one...</b></remarks>
        public void Commit() => OnChange?.Invoke(this, this);

        public static implicit operator T(Observable<T> current) => current.Value;
        public static implicit operator bool(Observable<T> current) => current.HasValue;
    }
}