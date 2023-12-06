using UnityEngine;

namespace Code.Scripts.Player
{
    public class MainAudioSource : MonoBehaviour
    {
        [SerializeField] public AudioSource damageSource;
        [SerializeField] public AudioSource equipSource;
        [SerializeField] public AudioSource concealSource;

        [SerializeField] public AudioSource fireSource;
        [SerializeField] public AudioSource deathSource;

        [SerializeField] public AudioSource pickupSource;
        [SerializeField] public AudioSource levelUpSource;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
