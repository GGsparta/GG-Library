using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GGL.Editor.Tools
{
    public static class FolderUtils
    {
        public static IEnumerable<Object> Find(FolderPath[] parentsPath, Type type, bool iterateChildren = false)
        {
            List<Object> result = new();
            for(int index = parentsPath.Length - 1; index >= 0; --index)
            {
                result.AddRange(Find(parentsPath[index], type, iterateChildren));
            }
            return result.ToArray();
        }

        public static TYPE[] Find<TYPE>(FolderPath[] parentsPath, bool iterateChildren = false) where TYPE : Object
        {
            List<TYPE> result = new();
            for(int index = parentsPath.Length - 1; index >= 0; --index)
            {
                result.AddRange(Find<TYPE>(parentsPath[index], iterateChildren));
            }
            return result.ToArray();
        }

        public static List<TYPE> Find<TYPE>(List<FolderPath> parentsPath, bool iterateChildren = false) where TYPE : Object
        {
            List<TYPE> result = new();
            for(int index = parentsPath.Count - 1; index >= 0; --index)
            {
                result.AddRange(Find<TYPE>(parentsPath[index], iterateChildren));
            }
            return result;
        }

        public static Object[] Find(FolderPath parentPath, Type type, bool iterateChildren = false)
        {
            IEnumerable<string> paths = GetPaths(parentPath, iterateChildren);
            List<Object> result = new();

            foreach(string path in paths)
            {
                if(path.EndsWith(".meta"))
                {
                    continue;
                }
                UnityEngine.GameObject mb = AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(path);
                if(type.IsSubclassOf(typeof(Component)) && mb != null)
                {
                    Component[] objs = mb.GetComponentsInChildren(type, true);
                    result.AddRange(objs);
                }
                else
                {
                    Object obj = AssetDatabase.LoadAssetAtPath(path, type);
                    if(obj != null)
                    {
                        result.Add(obj);
                    }
                }
            }
            return result.ToArray();
        }

        public static TYPE[] Find<TYPE>(FolderPath parentPath, bool iterateChildren = false) where TYPE : Object
        {
            IEnumerable<string> paths	= GetPaths(parentPath, iterateChildren);
            List<TYPE>	result	= new();

            foreach(string path in paths)
            {
                if(path.EndsWith(".meta"))
                {
                    continue;
                }
                UnityEngine.GameObject mb = AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(path);
                if(typeof(TYPE).IsSubclassOf(typeof(Component)) && mb != null)
                {
                    TYPE[] objs = mb.GetComponentsInChildren<TYPE>(true);
                    result.AddRange(objs);
                }
                else
                {
                    TYPE obj = AssetDatabase.LoadAssetAtPath<TYPE>(path);
                    if(obj != null)
                    {
                        result.Add(obj);
                    }
                }
            }
            return result.ToArray();
        }

        public static IEnumerable<string> GetPaths(FolderPath parentPath, bool iterateChildren = false)
        {
            List<string> files = new();
            if(parentPath == null || string.IsNullOrEmpty(parentPath.ToString()))
            {
                return Array.Empty<string>();
            }
            GetFiles(parentPath.ToString(), files, iterateChildren);
            return files.ToArray();
        }

        private static void GetFiles(string parentPath, List<string> files, bool iterateChildren = false)
        {
            files.AddRange(Directory.GetFiles(parentPath));
            if (!iterateChildren) return;
            string[] directories = Directory.GetDirectories(parentPath);
            foreach (var t in directories) GetFiles(t, files, true);
        }
    }
}
