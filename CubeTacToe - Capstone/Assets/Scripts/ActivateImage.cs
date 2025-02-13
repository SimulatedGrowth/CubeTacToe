using UnityEngine;
using UnityEngine.UI;

public class ActivateImage : MonoBehaviour
{
    public GameObject buttonPrefab; // Prefab for the button (with X and O images)
    public Transform[] buttonPositions;  // Array of Transforms that will hold the positions (from the scene)

    public enum PlayerRole { None, X, O }
    private PlayerRole assignedRole = PlayerRole.None;
    private static PlayerRole firstAssignedRole = PlayerRole.None;

    private Button[] buttons; // Array to store button instances

    void Start()
    {
        // Determine the first role randomly
        if (firstAssignedRole == PlayerRole.None)
        {
            firstAssignedRole = (Random.value > 0.5f) ? PlayerRole.X : PlayerRole.O;
        }

        AssignRole(firstAssignedRole == PlayerRole.X ? PlayerRole.O : PlayerRole.X);

        // Instantiate buttons at the start and set up listeners
        buttons = new Button[buttonPositions.Length]; // Initialize the buttons array
        for (int i = 0; i < buttonPositions.Length; i++)
        {
            // Instantiate the button prefab at the correct position
            GameObject newButton = Instantiate(buttonPrefab, buttonPositions[i].position, buttonPositions[i].rotation);

            // Parent the new button to the corresponding empty GameObject (keeps buttons moving with the cube)
            newButton.transform.SetParent(buttonPositions[i], true); // "true" keeps world space position

            // Assign the button reference in the buttons array
            buttons[i] = newButton.GetComponent<Button>();

            // Set up the OnClick listener for the button
            int index = i; // To avoid closure issues in the lambda
            buttons[i].onClick.AddListener(() => OnClick(index));
        }
    }

    // Assign a new role to the player
    public void AssignRole(PlayerRole role)
    {
        assignedRole = role;
    }

    // This method will be called when the button is clicked
    public void OnClick(int positionIndex)
    {
        // Get the Image components for X and O images
        Image[] images = buttons[positionIndex].GetComponentsInChildren<Image>();

        // Find the X and O images in the button
        Image imageX = null;
        Image imageO = null;

        foreach (var img in images)
        {
            if (img.name == "X") // Ensure the X image has the correct name in the prefab
            {
                imageX = img;
            }
            else if (img.name == "O") // Ensure the O image has the correct name in the prefab
            {
                imageO = img;
            }
        }

        // Show the image for the assigned role and hide the other
        if (assignedRole == PlayerRole.X)
        {
            imageX.enabled = true;
            imageO.enabled = false;
        }
        else if (assignedRole == PlayerRole.O)
        {
            imageX.enabled = false;
            imageO.enabled = true;
        }

        // Disable the button after it's clicked (optional, if you want to prevent multiple clicks)
        buttons[positionIndex].interactable = false;
    }
}
