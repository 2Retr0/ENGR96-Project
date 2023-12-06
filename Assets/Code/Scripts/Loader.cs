using Code.Scripts;
using UnityEngine.SceneManagement;

public static class Loader
{
    private static Scene targetScene;

    public enum Scene { 
        MainMenuScene,
        GameScene,
        LoadingScene,
        SettingsScene,
        CreditsScene
    }

    public static void Load(Scene target) {
        targetScene = target;

        SceneManager.LoadScene(Scene.LoadingScene.ToString());
        PostManager.Instance.Reset();
    }
    
    public static void LoadInstant(Scene target) {
        targetScene = target;

        SceneManager.LoadScene(targetScene.ToString());
        //PostManager.Instance.Reset();
    }
    
    
    public static void LoaderCallback() {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
