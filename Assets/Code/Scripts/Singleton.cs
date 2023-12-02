using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static readonly Object SyncRoot = new();

    public static T Instance
    {
        get
        {
            var self = instance;
            if (self) return self;

            lock (SyncRoot)
            {
                if (self) return self;

                instance = FindObjectOfType(typeof(T)) as T;
                if (!self)
                    Debug.LogError("SingletoneBase<T>: Could not found GameObject of type " + typeof(T).Name);
            }
            return self;
        }
    }
}
