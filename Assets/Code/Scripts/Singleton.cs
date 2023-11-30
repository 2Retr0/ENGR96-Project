using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance = null;
    private static readonly Object syncRoot = new Object();

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = FindObjectOfType(typeof(T)) as T;
                        if (instance == null)
                            Debug.LogError(
                                "SingletoneBase<T>: Could not found GameObject of type " + typeof(T).Name);
                    }
                }
            }

            return instance;
        }
    }
}
