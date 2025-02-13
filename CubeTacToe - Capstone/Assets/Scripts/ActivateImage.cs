using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ActivateImage : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform[] buttonPositions;

    public enum PlayerRole { None, X, O }
    private PlayerRole assignedRole = PlayerRole.None;
    private static PlayerRole firstAssignedRole = PlayerRole.None;

    private Button[] buttons;
    private List<Button> availableButtons = new List<Button>();

    void Start()
    {
        
        if (firstAssignedRole == PlayerRole.None)
        {
            firstAssignedRole = (Random.value > 0.5f) ? PlayerRole.X : PlayerRole.O;
        }

        AssignRole(firstAssignedRole);
        buttons = new Button[buttonPositions.Length];

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

        RoundManager.EndTurn();
    }

    public void AITurn()
    {
        if (availableButtons.Count == 0) return; 

        int randomIndex = Random.Range(0, availableButtons.Count);
        Button aiButton = availableButtons[randomIndex];

        ShowImage(aiButton, assignedRole == PlayerRole.X ? PlayerRole.O : PlayerRole.X);
        availableButtons.Remove(aiButton);

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
        }
        else
        {
            imageX.enabled = false;
            imageO.enabled = true;
        }

        button.enabled = false;
    }
}
