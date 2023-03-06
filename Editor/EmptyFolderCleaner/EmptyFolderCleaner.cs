using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GGL.Editor.EmptyFolderCleaner
{
    public class EmptyFolderCleaner : AssetModificationProcessor
    {
        private const string CLEAN_ON_SAVE_KEY = "k1";

        public static event Action OnAutoClean;

        public static string[] OnWillSaveAssets(string[] paths)
        {
            if (!CleanOnSave) return paths;

            FillEmptyDirList(out List<DirectoryInfo> emptyDirs);

            if (emptyDirs is not { Count: > 0 }) return paths;

            DeleteAllEmptyDirAndMeta(ref emptyDirs);

            Debug.Log("[Clean] Cleaned Empty Directories on Save");

            OnAutoClean?.Invoke();

            return paths;
        }


        public static bool CleanOnSave
        {
            get => EditorPrefs.GetBool(CLEAN_ON_SAVE_KEY, false);
            set => EditorPrefs.SetBool(CLEAN_ON_SAVE_KEY, value);
        }


        public static void DeleteAllEmptyDirAndMeta(ref List<DirectoryInfo> emptyDirs)
        {
            foreach (DirectoryInfo dirInfo in emptyDirs)
                AssetDatabase.MoveAssetToTrash(GetRelativePathFromCd(dirInfo.FullName));

            emptyDirs = null;
        }

        public static void FillEmptyDirList(out List<DirectoryInfo> emptyDirs)
        {
            List<DirectoryInfo> newEmptyDirs = new List<DirectoryInfo>();
            emptyDirs = newEmptyDirs;

            DirectoryInfo assetDir = new DirectoryInfo(Application.dataPath);

            WalkDirectoryTree(assetDir, (dirInfo, areSubDirsEmpty) =>
            {
                bool isDirEmpty = areSubDirsEmpty && DirHasNoFile(dirInfo);
                if (isDirEmpty)
                    newEmptyDirs.Add(dirInfo);
                return isDirEmpty;
            });
        }

        // return: Is this directory empty?
        private delegate bool IsEmptyDirectory(DirectoryInfo dirInfo, bool areSubDirsEmpty);

        // return: Is this directory empty?
        private static bool WalkDirectoryTree(DirectoryInfo root, IsEmptyDirectory pred)
        {
            DirectoryInfo[] subDirs = root.GetDirectories();

            bool areSubDirsEmpty = true;
            foreach (DirectoryInfo dirInfo in subDirs)
            {
                if (false == WalkDirectoryTree(dirInfo, pred))
                    areSubDirsEmpty = false;
            }

            bool isRootEmpty = pred(root, areSubDirsEmpty);
            return isRootEmpty;
        }

        private static bool DirHasNoFile(DirectoryInfo dirInfo)
        {
            FileInfo[] files = null;

            try
            {
                files = dirInfo.GetFiles("*.*");
                files = files.Where(x => !IsMetaFile(x.Name)).ToArray();
            }
            catch (Exception)
            {
                // ignored
            }

            return files == null || files.Length == 0;
        }

        private static string GetRelativePathFromCd(string filespec)
        {
            return GetRelativePath(filespec, Directory.GetCurrentDirectory());
        }

        public static string GetRelativePath(string filespec, string folder)
        {
            Uri pathUri = new(filespec);
            // Folders must end in a slash
            if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                folder += Path.DirectorySeparatorChar;
            }

            Uri folderUri = new(folder);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString()
                .Replace('/', Path.DirectorySeparatorChar));
        }

        private static bool IsMetaFile(string path)
        {
            return path.EndsWith(".meta");
        }
    }
}