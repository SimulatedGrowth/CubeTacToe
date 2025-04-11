using UnityEngine;
using TMPro;
using System.Collections;

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
                    timerText.text = "Time Left: " + Mathf.Ceil(timeLeft).ToString() + "s";
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
            Instance.Invoke("AITurn", 1.5f);
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
        turnText.text = isPlayerTurn ? "Player 1's Turn" : "AI's Turn";
    }

    public static bool GetIsPlayerTurn()
    {
        return isPlayerTurn;
    }

    private void ResetTimer()
    {
        timeLeft = turnTime;
        timerText.text = "Time Left: " + Mathf.Ceil(timeLeft).ToString() + "s";
    }
}
