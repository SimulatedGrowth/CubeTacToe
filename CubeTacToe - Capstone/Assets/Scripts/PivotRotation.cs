using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotRotation : MonoBehaviour
{
    private List<GameObject> activeSide;
    private Vector3 localForward;
    private Vector3 mouseRef;
    private bool dragging = false;

    private bool autoRotating = false;
    private float sensitivity = 0.4f;
    private float speed = 300f;
    private Vector3 rotation;

    private Quaternion targetQuaternion;

    private ReadCube readCube;
    private CubeState cubeState;
    private GameManager gameManager;

    void Start()
    {
        readCube = FindAnyObjectByType<ReadCube>();
        cubeState = FindAnyObjectByType<CubeState>();
        gameManager = FindAnyObjectByType<GameManager>();
    }

    void LateUpdate()
    {
        if (dragging && !autoRotating)
        {
            SpinSide(activeSide);
            if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
            {
                dragging = false;
                RotateToRightAngle();
            }
        }
        if (autoRotating)
        {
            AutoRotate();
        }
    }

    private void SpinSide(List<GameObject> side)
    {
        rotation = Vector3.zero;
        Vector2 offset = Vector2.zero;

        if (Input.touchCount > 0)
        {
            offset = (Input.GetTouch(0).position - new Vector2(mouseRef.x, mouseRef.y));
        }
        else
        {
            offset = (Input.mousePosition - new Vector3(mouseRef.x, mouseRef.y, 0f));
        }

        // Similar side handling logic for cube rotations
        if (side == cubeState.up) rotation.y = (offset.x + offset.y) * sensitivity * 1;
        if (side == cubeState.down) rotation.y = (offset.x + offset.y) * sensitivity * -1;
        if (side == cubeState.left) rotation.z = (offset.x + offset.y) * sensitivity * 1;
        if (side == cubeState.right) rotation.z = (offset.x + offset.y) * sensitivity * -1;
        if (side == cubeState.front) rotation.x = (offset.x + offset.y) * sensitivity * -1;
        if (side == cubeState.back) rotation.x = (offset.x + offset.y) * sensitivity * 1;

        transform.Rotate(rotation, Space.Self);
        mouseRef = Input.touchCount > 0 ? new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0f) : Input.mousePosition;
    }


    public void Rotate(List<GameObject> side)
    {
        activeSide = side;
        mouseRef = (Input.touchCount > 0) ?
            new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0f)
            : Input.mousePosition;

        dragging = true;
        InteractionState.draggingSide = true; // set dragging
        localForward = Vector3.zero - side[4].transform.parent.transform.localPosition;
    }


    public void StartAutoRotate(List<GameObject> side, float angle)
    {
        cubeState.PickUp(side);
        localForward = Vector3.zero - side[4].transform.parent.transform.localPosition;
        targetQuaternion = Quaternion.AngleAxis(angle, localForward) * transform.localRotation;
        activeSide = side;
        autoRotating = true;
    }

    public void RotateToRightAngle()
    {
        Vector3 vec = transform.localEulerAngles;
        vec.x = Mathf.Round(vec.x / 90) * 90;
        vec.y = Mathf.Round(vec.y / 90) * 90;
        vec.z = Mathf.Round(vec.z / 90) * 90;

        targetQuaternion.eulerAngles = vec;
        autoRotating = true;

        InteractionState.draggingSide = false; // 🔹 stop side dragging
        gameManager?.CheckLineup();
        gameManager?.UpdatePointsUI();
    }


    private void AutoRotate()
    {
        dragging = false;
        var step = speed * Time.deltaTime;
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetQuaternion, step);

        if (Quaternion.Angle(transform.localRotation, targetQuaternion) <= 1)
        {
            transform.localRotation = targetQuaternion;
            cubeState.PutDown(activeSide, transform.parent);
            readCube.ReadState();
            CubeState.autoRotating = false;
            autoRotating = false;
            dragging = false;
            InteractionState.draggingSide = false; // reset
            gameManager?.CheckLineup();
            gameManager?.UpdatePointsUI();
        }

    }
}
