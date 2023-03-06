using UnityEngine;

namespace GGL.Editor.MissingReferencesTracker
{
    public class MissingReferences
    {
        private struct Results
        {
            public int goCount;
            public int compCount;
            public int missingCount;
        }
        
        public static void CheckMissingReferences(params GameObject[] go)
        {
            Results results = new();
            
            foreach (GameObject g in go) 
                CheckGameObject(g, ref results);
            
            Debug.Log($"Searched {results.goCount} GameObjects, {results.compCount} components, found {results.missingCount} missing");
        }
        
        private static void CheckGameObject(GameObject go, ref Results results)
        {
            results.goCount++;
            Component[] components = go.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                results.compCount++;
                if (components[i] == null)
                {
                    results.missingCount++;
                    string s = go.name;
                    Transform t = go.transform;
                    while (t.parent != null)
                    {
                        s = t.parent.name + "/" + s;
                        t = t.parent;
                    }
                    Debug.Log(s + " has an empty script attached in position: " + i, go);
                }
            }
            
            // Now recurse through each child GO (if there are any):
            foreach (Transform childT in go.transform) 
                CheckGameObject(childT.gameObject, ref results);
        }
    }
}