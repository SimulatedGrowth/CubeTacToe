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
        if (Input.GetMouseButtonDown(0) && !CubeState.autoRotating && !InteractionState.hasRotatedThisTurn)
        {
            InteractionState.clickingOnCube = false;

            readCube.ReadState();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100.0f, layerMask))
            {
                GameObject face = hit.collider.gameObject;

                List<List<GameObject>> cubeSides = new List<List<GameObject>>()
                {
                    cubeState.up,
                    cubeState.down,
                    cubeState.left,
                    cubeState.right,
                    cubeState.front,
                    cubeState.back
                };

                foreach (List<GameObject> cubeSide in cubeSides)
                {
                    if (cubeSide.Contains(face))
                    {
                        InteractionState.clickingOnCube = true;
                        cubeState.PickUp(cubeSide);
                        cubeSide[4].transform.parent.GetComponent<PivotRotation>().Rotate(cubeSide);
                        InteractionState.hasRotatedThisTurn = true;
                        break;
                    }
                }
            }
        }
    }
}
