using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class ActivateImage : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform[] buttonPositions;
    public TMP_Text playerPointsText;
    public TMP_Text aiPointsText;

    public enum PlayerRole { None, X, O }
    private PlayerRole assignedRole = PlayerRole.None;
    private static PlayerRole firstAssignedRole = PlayerRole.None;

    private Button[] buttons;
    private List<Button> availableButtons = new List<Button>();
    private GameObject[] buttonMarks;

    private HashSet<int> usedMarks = new HashSet<int>();
    private int playerPoints = 0;
    private int aiPoints = 0;

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

        UpdatePointsUI();
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

        CheckLineup();
        UpdatePointsUI();
        RoundManager.EndTurn();
    }

    public void AITurn()
    {
        if (availableButtons.Count == 0) return;

        int randomIndex = UnityEngine.Random.Range(0, availableButtons.Count);
        Button aiButton = availableButtons[randomIndex];

        ShowImage(aiButton, assignedRole == PlayerRole.X ? PlayerRole.O : PlayerRole.X);
        availableButtons.Remove(aiButton);

        CheckLineup();
        UpdatePointsUI();
        RoundManager.EndTurn();
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

        button.enabled = false;
    }

    private void CheckLineup()
    {
        for (int i = 0; i < buttonMarks.Length; i++)
        {
            if (buttonMarks[i] == null) continue;

            for (int j = i + 1; j < buttonMarks.Length; j++)
            {
                if (buttonMarks[j] == null) continue;

                for (int k = j + 1; k < buttonMarks.Length; k++)
                {
                    if (buttonMarks[k] == null) continue;

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
        }
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


    private void UpdatePointsUI()
    {
        playerPointsText.text = "Player Points: " + playerPoints.ToString();
        aiPointsText.text = "AI Points: " + aiPoints.ToString();
    }
}
