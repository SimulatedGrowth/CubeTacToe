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
        // Start background drag only if the click started away from cube
        if (Input.GetMouseButtonDown(0) && !InteractionState.clickingOnCube && !CubeState.autoRotating)
        {
            InteractionState.isDraggingBackground = true;
            previousMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0) && InteractionState.isDraggingBackground &&
            !InteractionState.draggingSide && !CubeState.autoRotating)
        {
            Vector3 currentMousePosition = Input.mousePosition;
            mouseDelta = currentMousePosition - previousMousePosition;
            mouseDelta *= 0.1f;
            transform.rotation = Quaternion.Euler(mouseDelta.y, -mouseDelta.x, 0) * transform.rotation;
            previousMousePosition = currentMousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            InteractionState.isDraggingBackground = false;
        }
    }
}
