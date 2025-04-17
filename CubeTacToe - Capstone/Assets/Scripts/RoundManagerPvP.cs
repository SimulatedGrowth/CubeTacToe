using UnityEngine;
using TMPro;
using System.Collections;

public class RoundManagerPvP : MonoBehaviour
{
    public static RoundManagerPvP Instance { get; private set; }

    public TMP_Text timerText;
    public TMP_Text turnText;
    public float turnTime = 60f;

    private float timeLeft;
    private GameManager_PvP gameManager;
    private static bool isPlayer1Turn = true;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager_PvP>();
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
                timeLeft -= Time.deltaTime;
                timerText.text = Mathf.Ceil(timeLeft).ToString() + "s";
                yield return null;
            }

            EndTurn();
        }
    }

    public void EndTurn()
    {
        isPlayer1Turn = !isPlayer1Turn;
        InteractionState.hasRotatedThisTurn = false;

        UpdateUI();
        ResetTimer();
        gameManager.SwitchTurnManually();
    }

    public static bool IsPlayer1Turn()
    {
        return isPlayer1Turn;
    }

    public void SetTurnText(GameManager_PvP.PlayerRole currentTurn)
    {
        turnText.text = (currentTurn == GameManager_PvP.PlayerRole.X) ? "Player 1's Turn" : "Player 2's Turn";
    }

    private void UpdateUI()
    {
        turnText.text = isPlayer1Turn ? "Player 1's Turn" : "Player 2's Turn";
    }

    private void ResetTimer()
    {
        timeLeft = turnTime;
        timerText.text = Mathf.Ceil(timeLeft).ToString() + "s";
    }
    public static void ResetState()
    {
        isPlayer1Turn = true;

        if (Instance != null)
        {
            Instance.StopAllCoroutines();
            Instance.timerText.text = "";
            Instance.turnText.text = "";
        }

        Instance = null;
    }
}
