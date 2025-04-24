using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;




public class MoveRotateBoat : MonoBehaviour
{
    public Transform cameraInfo; // XR Camera
    public float rotationSpeed = 50f;
    public float moveSpeed = 2f;
    public Vector3 positionOffset = new Vector3(0f, -3f, 0f); // Boat follows camera with vertical offset




    private Vector2 joystickInput;
    private float throttleInput = 0f;
    private InputAction throttleAction;




    void OnEnable()
    {
        var playerInput = GetComponent<PlayerInput>();
        throttleAction = playerInput.actions["Move Forward"];
        throttleAction.performed += OnThrottle;
        throttleAction.Enable();
    }




    void OnDisable()
    {
        if (throttleAction != null)
            throttleAction.performed -= OnThrottle;
    }




    void LateUpdate()
    {
        if (cameraInfo == null) return;
       
        // 1. Move camera forward with trigger
        if (throttleInput > 0.1f)
        {
            Debug.Log("HERE??");
            // Get horizontal movement direction (ignore vertical)
            Vector3 boatForward = transform.forward;
           
            // Move the camera forward
            Debug.Log("1");
            Debug.Log(cameraInfo.position);
            cameraInfo.position += boatForward * moveSpeed * throttleInput * Time.deltaTime;
            Debug.Log("2");
            Debug.Log(cameraInfo.position);
 
        }




        // 2. Rotate boat with joystick (camera stays independent)
        if (joystickInput != Vector2.zero)
        {
            float yaw = joystickInput.x * rotationSpeed * Time.deltaTime;
            transform.Rotate(0, yaw, 0, Space.World);
        }




        // 3. Boat follows camera position with vertical offset
        Vector3 targetBoatPosition = cameraInfo.position + positionOffset;
        transform.position = targetBoatPosition;
    }




    public void OnMove(InputAction.CallbackContext context)
    {
        joystickInput = context.ReadValue<Vector2>();
    }




    public void OnThrottle(InputAction.CallbackContext context)
    {
        throttleInput = context.ReadValue<float>();
        Debug.Log("Throttle: " + throttleInput);
    }
}



