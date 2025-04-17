using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveRotate : MonoBehaviour
{
    public InputActionProperty moveTrigger; 
    
    private Quaternion start;
    void Start(){
        start = transform.rotation;
    }
    //ray tracing is the goal.
    void Update()
    {
        Vector2 lookingPosition = Mouse.current.position.ReadValue();
    
        //ray tracing solution doesn't work
        Ray ray = Camera.main.ScreenPointToRay(lookingPosition);
        // Vector3 newPosition = ray.direction;
        Quaternion newRotation = Quaternion.LookRotation(ray.direction);
        // newRotation.x = newRotation.x * (-1f);
        transform.rotation = Quaternion.Euler(newRotation.eulerAngles * 6f);
        // transform.rotation = Quaternion.RotateTowards(start, newRotation, 90f);

        //Line from GPT-4o
        float triggerValue = moveTrigger.action.ReadValue<float>();
        //Line from GPT-4o
        if (triggerValue > 0.1f) 
        {
            //get forward direction and continue
            Vector3 forward = transform.forward;
            forward.y = 0f;
            forward.Normalize();
            transform.position +=  Time.deltaTime * forward * 500;
        }
        // transform.LookAt(newPosition);
    }


}