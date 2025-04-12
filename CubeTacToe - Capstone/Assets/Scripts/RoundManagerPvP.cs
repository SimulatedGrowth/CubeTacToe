using UnityEngine;
using TMPro;

public class RoundManagerPvP : MonoBehaviour
{
    public static RoundManagerPvP Instance { get; private set; }
    public TMP_Text turnText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetTurnText(GameManager_PvP.PlayerRole currentTurn)
    {
        turnText.text = (currentTurn == GameManager_PvP.PlayerRole.X) ? "Player 1's Turn" : "Player 2's Turn";
    }
}
