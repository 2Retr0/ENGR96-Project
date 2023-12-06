//using Code.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private Button restartButton;
        [SerializeField] private Button quitButton;

        private void Awake() {
            restartButton.onClick.AddListener(() => {
                Loader.Load(Loader.Scene.GameScene);


                //var player = FindObjectOfType<PlayerController>();
                //player.animator.Rebind();
                //player.animator.Update(0f);
                //player.animator.speed = 1f;
            });
            
            quitButton.onClick.AddListener(() =>
            {
                Application.Quit();
            });
        }
    }
}