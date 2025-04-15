using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class GameManager_PvP : MonoBehaviour
{
    public GameObject buttonPrefab;
    public GameObject endpanel;
    public ParticleSystem highlightEffect;
    public Transform[] buttonPositions;
    public TMP_Text player1PointsText;
    public TMP_Text player2PointsText;
    public TMP_Text player1PointsTextEnd;
    public TMP_Text player2PointsTextEnd;
    public TMP_Text winnerText;

    public AudioSource audio;

    public enum PlayerRole { None, X, O }
    private PlayerRole currentTurn = PlayerRole.X;

    private Button[] buttons;
    private List<Button> availableButtons = new List<Button>();
    private GameObject[] buttonMarks;

    private HashSet<int> usedMarks = new HashSet<int>();
    private int player1Points = 0;
    private int player2Points = 0;
    private int moves;

    private ParticleSystem currentHighlightInstance;

    void Start()
    {
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

        UpdatePointsUI();
        RoundManagerPvP.Instance.SetTurnText(currentTurn);
    }

    public void OnClick(int positionIndex)
    {
        Button clickedButton = buttons[positionIndex];
        if (!availableButtons.Contains(clickedButton)) return;

        ShowImage(clickedButton, currentTurn);
        availableButtons.Remove(clickedButton);

        audio.Play();

        moves++;
        CheckLineup();
        UpdatePointsUI();

        if (moves >= buttons.Length || availableButtons.Count == 0)
        {
            ShowEndPanel();
            endpanel.SetActive(true);
            return;
        }

        // Switch turn
        currentTurn = (currentTurn == PlayerRole.X) ? PlayerRole.O : PlayerRole.X;
        RoundManagerPvP.Instance.SetTurnText(currentTurn);
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
            imageX.tag = "Player1Mark";
            buttonMarks[Array.IndexOf(buttons, button)] = imageX.gameObject;
        }
        else
        {
            imageX.enabled = false;
            imageO.enabled = true;
            imageO.tag = "Player2Mark";
            buttonMarks[Array.IndexOf(buttons, button)] = imageO.gameObject;
        }

        int idx = Array.IndexOf(buttons, button);
        HighlightLastMark(idx);
        button.interactable = false;
    }

    private void HighlightLastMark(int index)
    {
        if (currentHighlightInstance != null)
            Destroy(currentHighlightInstance.gameObject);

        Button btn = buttons[index];
        Transform markTransform = btn.transform;

        currentHighlightInstance = Instantiate(highlightEffect, markTransform.position, Quaternion.identity);
        currentHighlightInstance.transform.SetParent(markTransform);
        currentHighlightInstance.transform.localPosition += Vector3.forward * 0.05f;

        if (!currentHighlightInstance.isPlaying)
        {
            currentHighlightInstance.Play();
        }
    }

    private void CheckLineup()
    {
        foreach (var combo in CalculateWinningCombinations())
        {
            int i = combo[0], j = combo[1], k = combo[2];

            if (buttonMarks[i] == null || buttonMarks[j] == null || buttonMarks[k] == null) continue;
            if (usedMarks.Contains(i) && usedMarks.Contains(j) && usedMarks.Contains(k)) continue;

            if (IsValidLineup(i, j, k))
            {
                string tag = buttonMarks[i].tag;
                if (tag == "Player1Mark") player1Points++;
                else if (tag == "Player2Mark") player2Points++;

                MarkAsUsed(i, j, k);
                ChangeColor(i, j, k);
            }
        }
    }

    private List<int[]> CalculateWinningCombinations()
    {
        List<int[]> result = new List<int[]>();
        for (int i = 0; i < buttons.Length; i++)
        {
            for (int j = i + 1; j < buttons.Length; j++)
            {
                for (int k = j + 1; k < buttons.Length; k++)
                {
                    Vector3 pos1 = buttons[i].transform.position;
                    Vector3 pos2 = buttons[j].transform.position;
                    Vector3 pos3 = buttons[k].transform.position;

                    if (Vector3.Cross(pos2 - pos1, pos3 - pos1).magnitude < 0.1f)
                        result.Add(new int[] { i, j, k });
                }
            }
        }
        return result;
    }

    private bool IsValidLineup(int i, int j, int k)
    {
        string tag1 = buttonMarks[i].tag;
        string tag2 = buttonMarks[j].tag;
        string tag3 = buttonMarks[k].tag;
        return tag1 == tag2 && tag2 == tag3 && tag1 != "";
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
        Color color = new Color(0.7f, 0.1f, 0.8f);
        image1.color = image2.color = image3.color = color;
    }

    public void UpdatePointsUI()
    {
        player1PointsText.text = "Player 1: " + player1Points;
        player2PointsText.text = "Player 2: " + player2Points;
    }

    public void ShowEndPanel()
    {
        player1PointsTextEnd.text = $"Player 1 Score: {player1Points}";
        player2PointsTextEnd.text = $"Player 2 Score: {player2Points}";

        if (player1Points > player2Points)
            winnerText.text = "Player 1 Wins!";
        else if (player2Points > player1Points)
            winnerText.text = "Player 2 Wins!";
        else
            winnerText.text = "It's a Tie!";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
