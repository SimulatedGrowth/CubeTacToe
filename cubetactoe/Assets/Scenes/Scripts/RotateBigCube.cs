using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RotateBigCube : MonoBehaviour
{
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;
    Vector3 prevMousePos;
    Vector3 mouseDelta;

    public GameObject target;

    float speed = 200f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Swipe();
        Drag();
    }
    void Drag()
    {
        if (Input.GetMouseButtonDown(1))
        {
            //while the mouse is held down the cube can be moved around
            mouseDelta = Input.mousePosition - prevMousePos;
            mouseDelta *= 0.1f; //reduction of rotation speed
            transform.rotation = Quaternion.Euler(mouseDelta.y, -mouseDelta.x, 0) * transform.rotation;
        }
        else
        {
            //auto move to the target position
            if (transform.rotation != target.transform.rotation)
            {
                var step = speed * Time.deltaTime;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, target.transform.rotation, step);
            }
        }
        prevMousePos = Input.mousePosition;
    }
    void Swipe()
    {
        if (Input.GetMouseButtonDown(1)) {
            // get the 2D pos of the first mouse click
            firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            
        }
        if (Input.GetMouseButtonUp(1))
        {
            //get 2D pos of second mouse click
            secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            //create a vector from the first ad second click pos
            currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);
            // normalize the 2D vector
            currentSwipe.Normalize();
            if (LeftSwipe(currentSwipe))
            {
                target.transform.Rotate(0, 90, 0, Space.World);
            }
            else if (RightSwipe(currentSwipe))
            {
                target.transform.Rotate(0, -90, 0, Space.World);
            }
            else if (UpLeftSwipe(currentSwipe))
            {
                target.transform.Rotate(90, 0, 0, Space.World);
            }
            else if (UpRightSwipe(currentSwipe))
            {
                target.transform.Rotate(0, 0, -90, Space.World);
            }
            else if (DownLeftSwipe(currentSwipe))
            {
                target.transform.Rotate(0, 0, 90, Space.World);
            }
            else if (DownRightSwipe(currentSwipe))
            {
                target.transform.Rotate(-90, 0, 0, Space.World);
            }
        }
    }

    bool LeftSwipe(Vector2 swipe)
    {
        return swipe.x < 0 && swipe.y > -0.5f && swipe.y < 0.5f;
    }

    bool RightSwipe(Vector2 swipe)
    {
        return swipe.x > 0 && swipe.y > -0.5f && swipe.y < 0.5f;
    }
    bool UpLeftSwipe(Vector2 swipe)
    {
        return swipe.y > 0 && swipe.x < 0f;
    }
    bool UpRightSwipe(Vector2 swipe)
    {
        return swipe.y > 0 && swipe.x > 0f;
    }
    bool DownLeftSwipe(Vector2 swipe)
    {
        return swipe.y < 0 && swipe.x < 0f;
    }
    bool DownRightSwipe(Vector2 swipe)
    {
        return swipe.y < 0 && swipe.x > 0f;
    }
}
