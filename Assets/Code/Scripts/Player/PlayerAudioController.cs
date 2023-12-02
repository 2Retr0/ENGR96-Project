using UnityEngine;

namespace Code.Scripts.Player
{
    public class PlayerAudioController : MonoBehaviour
    {
        private AudioSource audioSource;
        private PlayerController movementController;
        private float defaultVolume;

        // Start is called before the first frame update
        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            movementController = GetComponent<PlayerController>();
            defaultVolume = audioSource.volume;
        }

        public void PlayFootstepSound()
        {
            if (movementController)
                audioSource.pitch = Random.Range(-0.2f, 0.2f) + Mathf.Log10(movementController.speed);
            audioSource.volume = defaultVolume * Mathf.Lerp(1.0f, 0.6f, audioSource.pitch);
            audioSource.Play();
        }
    }
}
