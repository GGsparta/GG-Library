namespace GGL.Animation
{
    /// <summary>
    /// Defines methods to save and load state of a <see cref="TriggerBehaviour"/>.
    /// </summary>
    public interface ITriggerState
    {
        /// <summary>
        /// Buffer the values of the target concerned by the animation. Used when restoring on disable.
        /// </summary>
        public void SaveState();

        /// <summary>
        /// Restore the values you buffered via <see cref="SaveState"/>. Used when restoring on disable.
        /// </summary>
        public void ResetState();
    }
}