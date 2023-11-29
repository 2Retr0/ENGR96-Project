using UnityEngine;

namespace Code.Scripts.Text
{
    public class TextController : MonoBehaviour
    {
        [SerializeField] public float lifetimeSeconds = 3;

        // Start is called before the first frame update
        void Start()
        {
            Destroy(gameObject, lifetimeSeconds);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
