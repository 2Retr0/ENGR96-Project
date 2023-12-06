using Code.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts
{
    public class CreditsSceneScript : MonoBehaviour
    {
        [SerializeField] private Button backButton;

        private void Awake() {
            backButton.onClick.AddListener(() => {
                Loader.LoadInstant(Loader.Scene.MainMenuScene);
            });
        }
    }
}
