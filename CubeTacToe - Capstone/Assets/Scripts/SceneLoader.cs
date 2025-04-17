using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // These can be assigned in the Inspector
    public string PvAISceneName = "PvAI";
    public string PvPSceneName = "PvP";
    public string Bullet = "PvB";
    public string Bounded = "PvL";

    public void LoadPvAIScene()
    {
        SceneManager.LoadScene(PvAISceneName);
    }

    public void LoadPvPScene()
    {
        SceneManager.LoadScene(PvPSceneName);
    }
    public void LoadBulletScene()
    {
        SceneManager.LoadScene(Bullet);
    }
    public void LoadBoundedScene()
    {
        SceneManager.LoadScene(Bounded);
    }
    public void menuLevel()
    {
        SceneManager.LoadScene("StartScreen");
    }
    public void restartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
