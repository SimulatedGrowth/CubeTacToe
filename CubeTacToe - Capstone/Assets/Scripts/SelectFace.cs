using UnityEngine;
using System.Collections.Generic;

public class SelectFace : MonoBehaviour
{
    private CubeState cubeState;
    private ReadCube readCube;
    private int layerMask = 1 << 8;

    void Start()
    {
        readCube = FindAnyObjectByType<ReadCube>();
        cubeState = FindAnyObjectByType<CubeState>();
    }

    void Update()
    {
        
        InteractionState.clickingOnCube = false;

       
        if ((Input.GetMouseButtonDown(0) ||
             (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            && !CubeState.autoRotating
            && !InteractionState.hasRotatedThisTurn
            && RoundManager.GetIsPlayerTurn())
        {
            readCube.ReadState();

            Vector3 inputPosition = Input.touchCount > 0
                ? (Vector3)Input.GetTouch(0).position
                : Input.mousePosition;

            Ray ray = Camera.main.ScreenPointToRay(inputPosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, layerMask))
            {
                GameObject face = hit.collider.gameObject;

                // all six sides
                List<List<GameObject>> cubeSides = new List<List<GameObject>>()
                {
                    cubeState.up,
                    cubeState.down,
                    cubeState.left,
                    cubeState.right,
                    cubeState.front,
                    cubeState.back
                };

                foreach (var side in cubeSides)
                {
                    if (side.Contains(face))
                    {
                        InteractionState.clickingOnCube = true;          // block background for this frame
                        cubeState.PickUp(side);
                        side[4].transform.parent
                            .GetComponent<PivotRotation>()
                            .Rotate(side);
                        InteractionState.hasRotatedThisTurn = true;     // no more side‑rotations this turn
                        break;
                    }
                }
            }
        }
    }
}
