using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
// ReSharper disable Unity.PerformanceCriticalCodeInvocation

namespace Code.Scripts
{
    public abstract class Singleton<T> : Singleton where T : MonoBehaviour {
        #region  Fields
        [CanBeNull]
        private static T instance;

        [NotNull]
        // ReSharper disable once StaticMemberInGenericType
        private static readonly object Lock = new object();

        [SerializeField]
        private bool persistent = true;
        #endregion

        #region  Properties
        [NotNull]
        public static T Instance {
            get {
                if (Quitting) {
                    Debug.LogWarning($"[{nameof(Singleton)}<{typeof(T)}>] Instance will not be returned because the application is quitting.");
                    // ReSharper disable once AssignNullToNotNullAttribute
                    return null;
                }

                lock (Lock) {
                    if (instance) return instance;

                    var instances = FindObjectsOfType<T>();
                    var count = instances.Length;
                    if (count > 0) {
                        if (count == 1) return instance = instances[0];

                        Debug.LogWarning($"[{nameof(Singleton)}<{typeof(T)}>] There should never be more than one {nameof(Singleton)} of type {typeof(T)} in the scene, but {count} were found. The first instance found will be used, and all others will be destroyed.");

                        for (var i = 1; i < instances.Length; i++)
                            Destroy(instances[i]);
                        return instance = instances[0];
                    }

                    Debug.Log($"[{nameof(Singleton)}<{typeof(T)}>] An instance is needed in the scene and no existing instances were found, so a new instance will be created.");
                    return instance = new GameObject($"({nameof(Singleton)}){typeof(T)}")
                        .AddComponent<T>();
                }
            }
        }
        #endregion

        #region  Methods
        private void Awake() {
            // if (persistent)
            //     DontDestroyOnLoad(gameObject);
            OnAwake();
        }

        protected virtual void OnAwake() { }
        #endregion
    }

    public abstract class Singleton : MonoBehaviour {
        #region  Properties
        public static bool Quitting { get; private set; }
        #endregion

        #region  Methods
        private void OnApplicationQuit() {
            Quitting = true;
        }
        #endregion
    }
}