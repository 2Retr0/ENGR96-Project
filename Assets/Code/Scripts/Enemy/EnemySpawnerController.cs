using Code.Scripts.Pickup;
using UnityEngine;

namespace Code.Scripts.Enemy
{
    public class EnemySpawnerController : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            EnemySpawnManager.Instance.AddSpawnPoint(transform.position);
        }
    }
}
