using System;
using System.Collections.Generic;
using GGL.Singleton;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace GGL.Pooling
{
    /// <summary>
    /// Simulates object instantiation and destruction by recycling them.
    /// </summary>
    [AddComponentMenu(Settings.NAME + "/Singletons/Object Pooler")]
    [HelpURL(Settings.ARTICLE_URL + "gameobject" + Settings.COMMON_EXT + "#optimisation")]
    public class ObjectPooler : SingletonBehaviour<ObjectPooler>
    {
        #region Variables
        #region Editor
        [Header("Pooling settings")]
        [SerializeField]
        private int maxPoolSize = 100;
        #endregion

        #region Private
        private Dictionary<Type, ObjectPool<Component>> _componentPool = new();
        private Dictionary<string, ObjectPool<GameObject>> _objectPool = new();
        private static Transform _parentBuffer;
        #endregion
        #endregion

        #region Methods
        #region Component
        /// <summary>
        /// Simulates an object instantiation by recycling if available (real instantiate otherwise).
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public new static T Instantiate<T>(T prefab, Transform parent = null) where T : Component
        {
            _parentBuffer = parent;
            if (!Application.isPlaying)
            {
                T ret = Pool_CreateComponent(prefab);
                Pool_PrepareComponent(ret, prefab);
                return ret;
            }

            Type type = typeof(T);
            if (!Instance._componentPool.ContainsKey(type))
            {
                Instance._componentPool.Add(type, new ObjectPool<Component>(
                    () => Pool_CreateComponent(prefab),
                    item => Pool_PrepareComponent(item, prefab),
                    Pool_StoreComponent,
                    Pool_DestroyComponent,
                    maxSize: Instance.maxPoolSize
                ));
            }

            return Instance._componentPool[type].Get() as T;
        }


        /// <summary>
        /// Simulate the destruction of an object by recycling him.
        /// </summary>
        /// <param name="poolObject"></param>
        public static void Destroy<T>(T poolObject) where T : Component
        {
            if (!Application.isPlaying)
            {
                DestroyImmediate(poolObject.gameObject);
                return;
            }

            Type type = typeof(T);
            if (!Instance._componentPool.ContainsKey(type))
                throw new EntryPointNotFoundException("Hmmmm... Where does that object come from? " + poolObject.name);

            Instance._componentPool[type].Release(poolObject);
        }


        private static T Pool_CreateComponent<T>(T prefab) where T : Component =>
            Object.Instantiate(prefab, _parentBuffer);

        private static void Pool_PrepareComponent<T>(T item, T prefab) where T : Component
        {
            item.gameObject.SetActive(true);
            item.transform.SetParent(_parentBuffer);
            item.transform.localScale = prefab.transform.localScale;
        }

        private static void Pool_StoreComponent<T>(T item) where T : Component => item.gameObject.SetActive(false);
        private static void Pool_DestroyComponent<T>(T item) where T : Component => Object.Destroy(item.gameObject);
        #endregion
        
        #region GameObject
        /// <summary>
        /// Simulates an object instantiation by recycling if available (real instantiate otherwise).
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        /// <remarks>DO NOT RENAME THE INSTANCIATED OBJECT PLEASE.</remarks>
        public static GameObject Instantiate(GameObject prefab, Transform parent = null)
        {
            _parentBuffer = parent;
            if (!Application.isPlaying)
            {
                GameObject ret = Pool_CreateGO(prefab);
                Pool_PrepareGO(ret, prefab);
                return ret;
            }

            string name = prefab.name;
            if (!Instance._objectPool.ContainsKey(name))
            {
                Instance._objectPool.Add(name, new ObjectPool<GameObject>(
                    () => Pool_CreateGO(prefab),
                    item => Pool_PrepareGO(item, prefab),
                    Pool_StoreGO,
                    Pool_DestroyGO,
                    maxSize: Instance.maxPoolSize
                ));
            }

            return Instance._objectPool[name].Get();
        }

        /// <summary>
        /// Simulates a gameobject creation by recycling if available (real new otherwise).
        /// </summary>
        /// <returns></returns>
        /// <remarks>DO NOT RENAME THE INSTANCIATED OBJECT PLEASE.</remarks>
        public static GameObject Create(string name, params Type[] components)
        {
            if (!Application.isPlaying)
                return new GameObject(name, components);

            if (!Instance._objectPool.ContainsKey(name))
            {
                Instance._objectPool.Add(name, new ObjectPool<GameObject>(
                    () => new GameObject(name, components),
                    item => item.SetActive(true),
                    item => item.SetActive(true),
                    Object.Destroy,
                    maxSize: Instance.maxPoolSize
                ));
            }

            return Instance._objectPool[name].Get();
        }

        /// <summary>
        /// Simulate the destruction of a gameobject by recycling him
        /// </summary>
        /// <param name="poolObject"></param>
        public static void Destroy(GameObject poolObject)
        {
            if (!Application.isPlaying)
            {
                DestroyImmediate(poolObject.gameObject);
                return;
            }

            string name = poolObject.name;
            if (!Instance._objectPool.ContainsKey(name))
                throw new EntryPointNotFoundException("Hmmmm... Where does that object come from? " + poolObject.name);

            Instance._objectPool[name].Release(poolObject);
        }


        private static GameObject Pool_CreateGO(GameObject prefab) => Object.Instantiate(prefab, _parentBuffer);

        private static void Pool_PrepareGO(GameObject item, GameObject prefab)
        {
            item.SetActive(true);
            item.transform.SetParent(_parentBuffer);
            item.transform.localScale = prefab.transform.localScale;
        }

        private static void Pool_StoreGO(GameObject item) => item.SetActive(false);

        private static void Pool_DestroyGO(GameObject item) => GameObject.Destroy(item);
        #endregion

        /// <summary>
        /// Clear the cache of registered objects.
        /// </summary>
        public static void Clear()
        {
            foreach (ObjectPool<Component> pool in Instance._componentPool.Values) pool.Clear();
            foreach (ObjectPool<GameObject> pool in Instance._objectPool.Values) pool.Clear();
        }
        #endregion
    }
}