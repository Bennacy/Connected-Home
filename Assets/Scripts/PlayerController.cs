using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Video;

public class PlayerController : MonoBehaviour
{

    [Header("Camera")]
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Transform cameraHolder;
    private Vector2 moveDirection;
    [SerializeField] private Camera cam;
    [SerializeField] private Vector2 cameraVerticalBounds = new Vector2(80, -80);
    private Vector2 cameraRotate;
    private float currCameraAngle;
    [Space(10)]

    [Header("Lights")]
    [SerializeField] private LightZone currentDivision;
    private bool changingSecondary = false;
    [Space(10)]

    [Header("Movement")]
    [SerializeField] private float walkSpeed;
    private Rigidbody rb;
    
    void Start()
    {
        currCameraAngle = 0;
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        //! Not needed anymore, here for reference and in case of emergency
        //! Controls used while implementing the light zones
        if(Input.GetKeyDown(KeyCode.Alpha1) && currentDivision != null){
            currentDivision.ToggleLights(changingSecondary);
            changingSecondary = false;
        }
        if(Input.GetKeyDown(KeyCode.Alpha2) && currentDivision != null){
            currentDivision.UpdateLightColor(changingSecondary, 1);
            changingSecondary = false;
        }
        if(Input.GetKeyDown(KeyCode.Alpha3) && currentDivision != null){
            currentDivision.UpdateLightIntensity(changingSecondary);
            changingSecondary = false;
        }
        // if(Input.GetKeyDown(KeyCode.F) && currentDivision != null){
        //     changingSecondary = !changingSecondary;
        // }

        if(Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    void FixedUpdate()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(moveX, moveY);
        Vector3 direction = moveDirection.y * transform.forward + moveDirection.x * transform.right;

        rb.AddForce(direction * walkSpeed * 10, ForceMode.Force);
    }

    public void ToggleLights(bool on){
        if(on){
            currentDivision.EnableLights();
        }else{
            currentDivision.DisableLights();
        }
    }
     
    public void CycleLightColor(int direction){
        currentDivision.UpdateLightColor(false, direction);
    }


    void LateUpdate()
    {
        RotateCamera();
    }

    private void RotateCamera(){
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");
        cameraRotate = new Vector2(mouseX, mouseY);

        Vector3 currPlayerRotation = transform.localRotation.eulerAngles;
        Vector3 currCamRotation = cameraController.transform.rotation.eulerAngles;

        float sens = cameraController.sens;
        Vector2 localRotate = cameraRotate;
        
        currPlayerRotation.y += localRotate.x * sens;
        currCameraAngle -= localRotate.y * sens;
        currCamRotation.y = 0;
        
        currCameraAngle = Mathf.Clamp(currCameraAngle, cameraVerticalBounds.x, cameraVerticalBounds.y);
        currCamRotation.x = currCameraAngle;

        cameraController.transform.localRotation = Quaternion.Euler(currCamRotation);
        transform.rotation = Quaternion.Euler(currPlayerRotation);
    }

    void OnTriggerEnter(Collider other)
    {
        LightZone lightZone = other.GetComponent<LightZone>();
        if(lightZone){
            currentDivision = lightZone;
        }
    }
    private void OnTriggerExit(Collider other) {
        LightZone lightZone = other.GetComponent<LightZone>();
        if(currentDivision == lightZone){
            currentDivision = null;
        }
    }
}
