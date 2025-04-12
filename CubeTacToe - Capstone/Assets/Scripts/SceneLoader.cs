using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // These can be assigned in the Inspector
    public string PvAISceneName = "PvAI";
    public string PvPSceneName = "PvP";

    public void LoadPvAIScene()
    {
        SceneManager.LoadScene(PvAISceneName);
    }

    public void LoadPvPScene()
    {
        SceneManager.LoadScene(PvPSceneName);
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
