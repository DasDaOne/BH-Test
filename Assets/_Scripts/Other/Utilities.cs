using System;
using System.Collections;
using System.Linq;
using Mirror;
using UnityEngine;

namespace Utilities
{
    public class TimeUtilities
    {
        public static IEnumerator Timer(float seconds, Action callback)
        {
            yield return new WaitForSeconds(seconds);
            callback.Invoke();
        }

        public static IEnumerator EndFrameTimer(Action callback)
        {
            yield return new WaitForEndOfFrame();
            callback.Invoke();
        }
    }

    namespace Singletons
    {
        public class Singleton<T> : MonoBehaviour where T : Singleton<T>, new()
        {
            private static T instance;

            public static T Instance
            {
                get
                {
                    if (instance == null)
                    {
                        T[] instances = Resources.FindObjectsOfTypeAll<T>();
                        if (instances.Length > 1)
                        {
                            Debug.LogError($"There is {instances.Length} instances of type {typeof(T)}");
                        }
                        else if (instances.Length == 0)
                        {
                            Debug.LogError($"Cannot find singleton of type {typeof(T)} on scene");
                        }
                        else
                        {
                            instance = instances.First();
                        }
                    }
                    return instance;
                }
            }

            public static bool IsInstanceExist()
            {
                return instance != null;
            }
        }
        public class NetworkSingleton<T> : NetworkBehaviour where T : NetworkSingleton<T>, new()
        {
            private static T instance;

            public static T Instance
            {
                get
                {
                    if (instance == null)
                    {
                        T[] instances = Resources.FindObjectsOfTypeAll<T>();
                        if (instances.Length > 1)
                        {
                            Debug.LogError($"There is {instances.Length} instances of type {typeof(T)}");
                        }
                        else if (instances.Length == 0)
                        {
                            Debug.LogError($"Cannot find singleton of type {typeof(T)} on scene");
                        }
                        else
                        {
                            instance = instances.First();
                        }
                    }
                    return instance;
                }
            }

            public static bool IsInstanceExist()
            {
                return instance != null;
            }
        }
    }
}