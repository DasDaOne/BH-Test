using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if(instance == null)
                    Debug.Log($"Cannot find singleton of type {typeof(T)} on scene");
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (!instance)
        {
            instance = GetComponent<T>();
        }
        else
        {
            Debug.LogError($"There is {FindObjectsOfType<T>().Length} objects of type {typeof(T)}");
        }
    }
}