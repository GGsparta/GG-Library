namespace GGL.Commands
{
    /// <summary>
    /// Base class for your commands.
    /// </summary>
    public abstract class Command
    {
        /// <value>
        /// When to consider the command is completed. Called every frame once command is executes until it returns true.
        /// </value>
        public abstract bool IsCompleted { get; }
        
        /// <summary>
        /// Piece of code to execute once only being processed by command processor.
        /// </summary>
        public abstract void Execute();
    }
}