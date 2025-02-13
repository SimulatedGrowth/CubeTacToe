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
    private ActivateImage activateImageScript;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        activateImageScript = FindObjectOfType<ActivateImage>();
        StartCoroutine(RoundTimer());
        UpdateUI();
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
        Instance.UpdateUI();

        if (!isPlayerTurn)
        {
            Instance.Invoke("AITurn", 1f); 
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
        turnText.text = isPlayerTurn ? "Player 1 Turn" : "AI Turn";
    }

    public static bool GetIsPlayerTurn()
    {
        return isPlayerTurn;
    }
}
