using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveRotateBoat : MonoBehaviour
{
    public Transform cameraInfo;
    //ray tracing is the goal.
    void Update()
    {
        
        transform.rotation = Quaternion.Euler(-58.13f, -90.41f + cameraInfo.rotation.eulerAngles.y , -90f);
        Vector3 newPosition = cameraInfo.position;
        newPosition.y -= 10;
        transform.position = newPosition;
    }


}


