using UnityEngine;

public static class InteractionState
{
    public static bool clickingOnCube = false;
    public static bool draggingSide = false;
    public static bool isDraggingBackground = false;
    public static bool hasRotatedThisTurn = false;
    public static void Reset()
    {
        clickingOnCube = false;
        draggingSide = false;
        isDraggingBackground = false;
        hasRotatedThisTurn = false;
    }
}

