using System;
using UnityEngine;

namespace Utility
{
    public class SingletonBehavior<T>: MonoBehaviour where T: MonoBehaviour
    {
        private static T instance;
        private static bool applicationIsQuitting = false;

        public static T Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    return null;
                }
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        GameObject singletonObject = new GameObject(typeof(T).Name);
                        instance = singletonObject.AddComponent<T>();
                        (instance as SingletonBehavior<T>)?.Init();
                        DontDestroyOnLoad(singletonObject);
                    }
                }
                return instance;
            }
        }

        protected virtual void Awake()
        {
            if(instance == null)
                instance = this as T;
            Init();
        }

        protected virtual void OnDestroy()
        {
            instance = null;
        }

        protected virtual void Init()
        {
            
        }
    }
}