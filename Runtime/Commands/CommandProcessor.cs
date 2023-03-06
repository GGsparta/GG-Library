using GGL.Collections;
using UnityEngine;

namespace GGL.Commands
{
    /// <summary>
    /// Proccess every commands by priority.
    /// </summary>
    /// <typeparam name="TC">Your command class</typeparam>
    /// <remarks></remarks>
    [HelpURL(Settings.ARTICLE_URL + "patterns" + Settings.COMMON_EXT + "#commande")]
    public abstract class CommandProcessor<TC> : MonoBehaviour where TC : Command
    {
        /// <value>
        /// Command being executed. Null is nothing is being executed.
        /// </value>
        protected TC Current { get; private set; }

        /// <summary>
        /// Queue of commands
        /// </summary>
        protected readonly PriorityQueue<TC, int> commands = new();

        /// <inheritdoc cref="MonoBehaviour" />
        [ExcludeFromDocFx]
        protected virtual void LateUpdate() => ProcessCommands();

        private void ProcessCommands()
        {
            if(Current is { IsCompleted: false }) return;

            Current = null;
            if (commands.Count == 0) return;

            Current = commands.Dequeue();
            Current.Execute();
        }

        /// <summary>
        /// Enqueue a command.
        /// </summary>
        /// <param name="command">Your custom command</param>
        /// <param name="priority">Priority of command (lower is more important)</param>
        /// <remarks>It won't be processed before next frame.</remarks>
        public void EnqueueCommand(TC command, int priority) => commands.Enqueue(command, priority);

        /// <summary>
        /// Execute immeditately a command. If there is another command currenlty working, it will re-enqeue it with the
        /// highest priority.
        /// </summary>
        /// <param name="command"></param>
        public void ExecuteImmediate(TC command)
        {
            command.Execute();
            EnqueueCommand(Current, int.MinValue);
            Current = command;
        }
    }
}