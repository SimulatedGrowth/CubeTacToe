using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string PvAISceneName = "PvAI";
    public string PvPSceneName = "PvP";
    public string Bullet = "PvB";
    public string Bounded = "PvL";

    public void LoadPvAIScene()
    {
        GameStateResetter.ResetAll();
        SceneManager.LoadScene(PvAISceneName);
    }

    public void LoadPvPScene()
    {
        GameStateResetter.ResetAll();
        SceneManager.LoadScene(PvPSceneName);
    }
    public void LoadBulletScene()
    {
        GameStateResetter.ResetAll();
        SceneManager.LoadScene(Bullet);
    }
    public void LoadBoundedScene()
    {
        GameStateResetter.ResetAll();
        SceneManager.LoadScene(Bounded);
    }
    public void menuLevel()
    {
        GameStateResetter.ResetAll();
        SceneManager.LoadScene("StartScreen");
    }
    public void restartLevel()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
public static class GameStateResetter
{
    public static void ResetAll()
    {
        InteractionState.Reset();

        string scene = SceneManager.GetActiveScene().name;

        if (scene == "PvAI" || scene == "PvB" || scene == "PvL")
        {
            RoundManager.ResetState();
            GameManager.ResetStatic();
        }
        else if (scene == "PvP")
        {
            RoundManagerPvP.ResetState();
            
        }
    }



}