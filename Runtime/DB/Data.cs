using System;

namespace GGL.DB
{
    /// <summary>
    /// Base class for any data used in <see cref="Database"/>.
    /// </summary>
    /// <remarks>Abstract types will not be stored, but they may be used to select children class.</remarks>
    [Serializable]
    public abstract class Data
    {
        /// <value>
        /// Pseudo read-only ID used for database storage.
        /// </value>
        public ulong Id { get; private set; }
        
        /// <value>
        /// Event triggered by <see cref="Table"/> if for some reason the ID had to change.
        /// </value>
        public event Action OnRebase;

        protected Data() {}
        protected Data(ulong id) => Id = id;

        /// <summary>
        /// This method is used by <see cref="Table"/> to set a new ID and trigger <see cref="OnRebase"/>.
        /// </summary>
        /// <param name="newID"></param>
        private void Rebase(ulong newID)
        {
            Id = newID;
            OnRebase?.Invoke();
        }
    }
}