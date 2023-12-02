using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject prefabItem;
    
    // Start is called before the first frame update
    private void Start()
    {
        for (var i = 0; i < 10; ++i)
        {
            var self = transform;
            Instantiate(prefabItem, self.position, self.rotation);
        }
        
    }
}
