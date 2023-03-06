using System;
using UnityEngine;

namespace GGL.Editor.Tools
{
    /// <summary>
    /// Displayable string that defines a path.
    /// </summary>
    [Serializable]
    public class FolderPath
    {
        [SerializeField]
        private string path;

        public string Path => path;

        public FolderPath(string path) => this.path = path;

        public override string ToString() => path;
    }
}
