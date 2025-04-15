using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }

    public TMP_Text timerText;
    public TMP_Text turnText;
    public float turnTime = 60f;

    private float timeLeft;
    private static bool isPlayerTurn = true;
    private GameManager activateImageScript;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        activateImageScript = FindAnyObjectByType<GameManager>();
        StartCoroutine(RoundTimer());
        UpdateUI();
        ResetTimer();
    }

    private IEnumerator RoundTimer()
    {
        while (true)
        {

            timeLeft = turnTime;

            while (timeLeft > 0)
            {
                if (isPlayerTurn)
                {
                    timeLeft -= Time.deltaTime;
                    timerText.text = Mathf.Ceil(timeLeft).ToString() + "s";
                }
                yield return null;
            }

            if (isPlayerTurn) EndTurn(); 
        }
    }

    public static void EndTurn()
    {
        isPlayerTurn = !isPlayerTurn;

        InteractionState.hasRotatedThisTurn = false;

        Instance.UpdateUI();
        Instance.ResetTimer();

        if (!isPlayerTurn)
        {
            string sceneName = SceneManager.GetActiveScene().name;

            if (sceneName == "PvAI")
            {
                Instance.Invoke("AITurn", 1.7f);
            }
            if (sceneName == "PvB")
            {
                Instance.Invoke("AITurn", 0.5f);
            }
        }
    }

    private void AITurn()
    {
        if (activateImageScript != null)
        {
            activateImageScript.AITurn();
        }
    }

    private void UpdateUI()
    {
        turnText.text = isPlayerTurn ? "Player's Turn" : "AI's Turn";
    }

    public static bool GetIsPlayerTurn()
    {
        return isPlayerTurn;
    }

    private void ResetTimer()
    {
        timeLeft = turnTime;
        timerText.text = Mathf.Ceil(timeLeft).ToString() + "s";
    }
}
