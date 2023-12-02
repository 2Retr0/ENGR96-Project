using UnityEngine;

namespace Code.Scripts.Text
{
    public class TextController : MonoBehaviour
    {
        [SerializeField] public float lifetimeSeconds = 3;

        // Start is called before the first frame update
        private void Start()
        {
            Destroy(gameObject, lifetimeSeconds);
        }
    }
}
