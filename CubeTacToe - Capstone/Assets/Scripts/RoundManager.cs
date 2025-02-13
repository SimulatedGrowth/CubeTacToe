using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance; // Singleton to access from other scripts

    public TMP_Text timerText;
    public TMP_Text turnText;
    public float turnTime = 60f;
    private float timeLeft;
    private static bool isPlayerTurn = true;

    private List<Button> availableButtons = new List<Button>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
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
                timeLeft -= Time.deltaTime;
                timerText.text = "Time Left: " + Mathf.Ceil(timeLeft).ToString() + "s";
                yield return null;
            }

            EndTurn();
        }
    }

    public void EndTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        UpdateUI();

        if (!isPlayerTurn)
        {
            Invoke("AITurn", 1f); // Delay AI move
        }
    }

    private void AITurn()
    {
        availableButtons.Clear();

        Button[] allButtons = FindObjectsOfType<Button>();
        foreach (Button btn in allButtons)
        {
            if (btn.interactable)
            {
                availableButtons.Add(btn);
            }
        }

        if (availableButtons.Count == 0)
        {
            Debug.Log("No available buttons left.");
            return;
        }

        // AI selects a random available button
        Button aiButton = availableButtons[Random.Range(0, availableButtons.Count)];
        aiButton.onClick.Invoke(); // Simulate AI clicking

        Debug.Log("AI clicked a button.");

        EndTurn();
    }

    private void UpdateUI()
    {
        turnText.text = isPlayerTurn ? "Player 1 Turn" : "Player 2 Turn";
    }

    public static bool GetIsPlayerTurn()
    {
        return isPlayerTurn;
    }
}
