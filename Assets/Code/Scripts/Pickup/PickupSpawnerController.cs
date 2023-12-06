using UnityEngine;

namespace Code.Scripts.Pickup
{
    public class PickupSpawnerController : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            PickupSpawnManager.Instance.AddSpawnPoint(transform.position);
        }
    }
}
