using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBigCube : MonoBehaviour
{
    private Vector3 previousMousePosition;
    private Vector3 mouseDelta;
    private float speed = 200f;
    

    void Update()
    {
        HandleBackgroundRotation();
    }
   

    void HandleBackgroundRotation()
    {
        
        if ((Input.GetMouseButtonDown(0) ||
             (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            && !InteractionState.clickingOnCube
            && !CubeState.autoRotating
            && RoundManager.GetIsPlayerTurn())  
        {
            InteractionState.isDraggingBackground = true;
            previousMousePosition = Input.touchCount > 0
                ? (Vector2)Input.GetTouch(0).position
                : (Vector2)Input.mousePosition;
        }

        if ((Input.GetMouseButton(0) ||
             (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved))
            && InteractionState.isDraggingBackground
            && !InteractionState.draggingSide
            && !CubeState.autoRotating)
        {
            Vector3 currentPos = Input.touchCount > 0
                ? (Vector2)Input.GetTouch(0).position
                : Input.mousePosition;

            mouseDelta = currentPos - previousMousePosition;
            mouseDelta *= 0.1f;
            transform.rotation = Quaternion.Euler(mouseDelta.y, -mouseDelta.x, 0) * transform.rotation;
            previousMousePosition = currentPos;
        }

        if (Input.GetMouseButtonUp(0) ||
            (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            InteractionState.isDraggingBackground = false;
        }
    }
}
