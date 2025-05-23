﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject buttonPrefab;
    public GameObject endpanel;
    public ParticleSystem highlightEffect;
    public Transform[] buttonPositions;
    public TMP_Text playerPointsText;
    public TMP_Text aiPointsText;
    public TMP_Text playerPointsTextEnd;
    public TMP_Text aiPointsTextEnd;
    public TMP_Text winnerText;

    public AudioSource audio;

    public int maxMoves = 54;

    public enum PlayerRole { None, X, O }
    private PlayerRole assignedRole = PlayerRole.None;
    private static PlayerRole firstAssignedRole = PlayerRole.None;

    private Button[] buttons;
    private List<Button> availableButtons = new List<Button>();
    private GameObject[] buttonMarks;

    private HashSet<int> usedMarks = new HashSet<int>();
    private int playerPoints = 0;
    private int aiPoints = 0;
    private int moves;
    

    private int lastPlayerMarkIndex = -1;
    private int lastAIMarkIndex = -1;
    private ParticleSystem currentHighlightInstance;

    void Start()
    {
        if (firstAssignedRole == PlayerRole.None)
        {
            firstAssignedRole = (UnityEngine.Random.value > 0.5f) ? PlayerRole.X : PlayerRole.O;
        }

        AssignRole(firstAssignedRole);
        buttons = new Button[buttonPositions.Length];
        buttonMarks = new GameObject[buttonPositions.Length];

        for (int i = 0; i < buttonPositions.Length; i++)
        {
            GameObject newButton = Instantiate(buttonPrefab, buttonPositions[i].position, buttonPositions[i].rotation);
            newButton.transform.SetParent(buttonPositions[i], true);

            Button btn = newButton.GetComponent<Button>();
            buttons[i] = btn;
            availableButtons.Add(btn);

            int index = i;
            btn.onClick.AddListener(() => OnClick(index));
        }

        moves = 0;
        endpanel.SetActive(false);

        CheckLineup();
        UpdatePointsUI();
    }
    public static void ResetStatic()
    {
        firstAssignedRole = PlayerRole.None;
    }

    public void ShowEndPanel()
    {
        
        playerPointsTextEnd.text = $"Player Score: {playerPoints}";
        aiPointsTextEnd.text = $"AI Score: {aiPoints}";

        if (playerPoints > aiPoints)
            winnerText.text = "Player Wins!";
        else if (playerPoints < aiPoints)
            winnerText.text = "AI Wins!";
        else
            winnerText.text = "It's a Tie!";
    }


    public void AssignRole(PlayerRole role)
    {
        assignedRole = role;
    }

    public void OnClick(int positionIndex)
    {
        if (!RoundManager.GetIsPlayerTurn()) return;

        Button clickedButton = buttons[positionIndex];
        if (!availableButtons.Contains(clickedButton)) return;

        ShowImage(clickedButton, assignedRole);
        availableButtons.Remove(clickedButton);

        audio.Play();

        moves++;
        if (moves >= (maxMoves))
        {
            endpanel.SetActive(true);
            ShowEndPanel();
            return;
        }

        RoundManager.EndTurn();
        CheckLineup();
        UpdatePointsUI();

        
    }

    public void AITurn()
    {
        if (availableButtons.Count == 0) 
        {
            
            endpanel.SetActive(true);
            ShowEndPanel();
            return;
            
        } 

        PlayerRole aiRole = (assignedRole == PlayerRole.X) ? PlayerRole.O : PlayerRole.X;

        int winIndex = GetStrategicMove(aiRole);
        if (winIndex != -1)
        {
            PlaceAIChoice(winIndex, aiRole);
            return;
        }

        int blockIndex = GetStrategicMove(assignedRole);
        if (blockIndex != -1)
        {
            PlaceAIChoice(blockIndex, aiRole);
            return;
        }

        int smartMoveIndex = GetBestMove(aiRole);
        if (smartMoveIndex != -1)
        {
            PlaceAIChoice(smartMoveIndex, aiRole);
            return;
        }

        

        int randomIndex = UnityEngine.Random.Range(0, availableButtons.Count);
        PlaceAIChoice(Array.IndexOf(buttons, availableButtons[randomIndex]), aiRole);

        
    }

    private void PlaceAIChoice(int index, PlayerRole role)
    {
        ShowImage(buttons[index], role);  
        availableButtons.Remove(buttons[index]);

        audio.Play();

        moves++;
        if (moves >= (maxMoves))
        {
            endpanel.SetActive(true);
            ShowEndPanel();
            return;
        }

        RoundManager.EndTurn();
        CheckLineup();
        UpdatePointsUI();

    }


    private int GetStrategicMove(PlayerRole role)
    {
        foreach (var combo in CalculateWinningCombinations())
        {
            int emptySpot = -1;
            int countRoleMarks = 0;

            foreach (int i in combo)
            {
                if (buttonMarks[i] == null)
                    emptySpot = i;
                else if (buttonMarks[i].tag == (role == assignedRole ? "PlayerMark" : "AIMark"))
                    countRoleMarks++;
            }

            if (countRoleMarks == 2 && emptySpot != -1)
                return emptySpot;
        }
        return -1;
    }

    private int GetBestMove(PlayerRole role)
    {
        Dictionary<int, int> moveScores = new Dictionary<int, int>();

        foreach (var combo in CalculateWinningCombinations())
        {
            foreach (int i in combo)
            {
                if (buttonMarks[i] == null)
                {
                    if (!moveScores.ContainsKey(i)) moveScores[i] = 0;
                    moveScores[i] += 1;
                }
            }
        }

        if (moveScores.Count > 0)
        {
            int bestMove = -1;
            int maxScore = -1;

            foreach (var kvp in moveScores)
            {
                if (kvp.Value > maxScore)
                {
                    maxScore = kvp.Value;
                    bestMove = kvp.Key;
                }
            }
            return bestMove;
        }

        return -1;
    }

    private void ShowImage(Button button, PlayerRole role)
    {
        Image[] images = button.GetComponentsInChildren<Image>();
        Image imageX = null, imageO = null;

        foreach (var img in images)
        {
            if (img.name == "X") imageX = img;
            if (img.name == "O") imageO = img;
        }

        if (role == PlayerRole.X)
        {
            imageX.enabled = true;
            imageO.enabled = false;
            imageX.tag = (assignedRole == PlayerRole.X) ? "PlayerMark" : "AIMark";
            buttonMarks[Array.IndexOf(buttons, button)] = imageX.gameObject;
        }
        else if (role == PlayerRole.O)
        {
            imageX.enabled = false;
            imageO.enabled = true;
            imageO.tag = (assignedRole == PlayerRole.O) ? "PlayerMark" : "AIMark";
            buttonMarks[Array.IndexOf(buttons, button)] = imageO.gameObject;
        }

        int idx = Array.IndexOf(buttons, button);
        if (role == assignedRole)
            lastPlayerMarkIndex = idx;
        else
            lastAIMarkIndex = idx;

        HighlightLastMark(idx, role);
        button.interactable = false;
    }
    private void HighlightLastMark(int index, PlayerRole role)
    {
        if (currentHighlightInstance != null)
            Destroy(currentHighlightInstance.gameObject);

        Button btn = buttons[index];
        Transform markTransform = btn.transform;

        currentHighlightInstance = Instantiate(highlightEffect, markTransform.position, Quaternion.identity);
        currentHighlightInstance.transform.SetParent(markTransform);
        currentHighlightInstance.transform.localPosition += Vector3.forward * 0.05f;

        if (currentHighlightInstance != null && !currentHighlightInstance.isPlaying)
        {
            currentHighlightInstance.Play();
        }
    }
    public void CheckLineup()
    {
        List<int[]> winningCombinations = CalculateWinningCombinations();
        foreach (var combination in winningCombinations)
        {
            int i = combination[0], j = combination[1], k = combination[2];

            if (buttonMarks[i] == null || buttonMarks[j] == null || buttonMarks[k] == null)
                continue;

            if (usedMarks.Contains(i) && usedMarks.Contains(j) && usedMarks.Contains(k))
                continue;

            if (IsValidLineup(i, j, k))
            {
                string winnerTag = buttonMarks[i].tag;

                if (winnerTag == "PlayerMark")
                {
                    playerPoints++;
                }
                else if (winnerTag == "AIMark")
                {
                    aiPoints++;
                }

                MarkAsUsed(i, j, k);
                ChangeColor(i, j, k);
            }
        }
    }

    private List<int[]> CalculateWinningCombinations()
    {
        List<int[]> winningCombinations = new List<int[]>();

        for (int i = 0; i < buttons.Length; i++)
        {
            for (int j = i + 1; j < buttons.Length; j++)
            {
                for (int k = j + 1; k < buttons.Length; k++)
                {
                    Vector3 pos1 = buttons[i].transform.position;
                    Vector3 pos2 = buttons[j].transform.position;
                    Vector3 pos3 = buttons[k].transform.position;

                    if (AreCollinear(pos1, pos2, pos3, 0.1f))
                    {
                        winningCombinations.Add(new int[] { i, j, k });
                    }
                }
            }
        }

        return winningCombinations;
    }

    private bool AreCollinear(Vector3 a, Vector3 b, Vector3 c, float threshold)
    {
        Vector3 ab = b - a;
        Vector3 ac = c - a;
        Vector3 crossProduct = Vector3.Cross(ab, ac);

        return crossProduct.magnitude < threshold;
    }

    private bool IsValidLineup(int i, int j, int k)
    {
        string tag1 = buttonMarks[i].tag;
        string tag2 = buttonMarks[j].tag;
        string tag3 = buttonMarks[k].tag;

        return tag1 == tag2 && tag2 == tag3 && tag1 != "" && tag2 != "" && tag3 != "";
    }

    private void MarkAsUsed(int i, int j, int k)
    {
        usedMarks.Add(i);
        usedMarks.Add(j);
        usedMarks.Add(k);
    }

    private void ChangeColor(int i, int j, int k)
    {
        Image image1 = buttonMarks[i].GetComponentInChildren<Image>();
        Image image2 = buttonMarks[j].GetComponentInChildren<Image>();
        Image image3 = buttonMarks[k].GetComponentInChildren<Image>();

        Color purplePink = new Color(0.7f, 0.1f, 0.8f);

        if (image1 != null) image1.color = purplePink;
        if (image2 != null) image2.color = purplePink;
        if (image3 != null) image3.color = purplePink;
    }

    public void UpdatePointsUI()
    {
        playerPointsText.text = "Player: " + playerPoints.ToString();
        aiPointsText.text = "AI: " + aiPoints.ToString();
    }
}
