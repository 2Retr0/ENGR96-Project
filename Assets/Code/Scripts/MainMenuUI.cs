using Code.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts
{
    public class MainMenuUI : Singleton
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button creditsButton;

        private void Awake() {
            playButton.onClick.AddListener(() => {
                Loader.Load(Loader.Scene.GameScene);

                var player = FindObjectOfType<PlayerController>();
                player.animator.Rebind();
                player.animator.Update(0f);
                player.animator.speed = 1f;
            });

            settingsButton.onClick.AddListener(() => {
                Loader.LoadInstant(Loader.Scene.SettingsScene);
            });

            quitButton.onClick.AddListener(() =>
            {
                Application.Quit();
            });
            
            creditsButton.onClick.AddListener(() => {
                Loader.LoadInstant(Loader.Scene.CreditsScene);
            });
        }
    }
}
