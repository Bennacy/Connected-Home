using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlayerController : MonoBehaviour
{

    [Header("Camera")]
    public CameraController cameraController;
    public Transform cameraHolder;
    private Vector2 moveDirection;
    public Camera cam;
    public Vector2 cameraVerticalBounds;
    private Vector2 cameraRotate;
    private float currCameraAngle;
    public ILightDivision currentDivision;
    [Space(10)]

    private Rigidbody rb;

    public float camSensitivity;
    public float walkSpeed;
    private float moveSpeed;
    
    void Start()
    {
        currCameraAngle = 0;
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if(currentDivision != null){
            if(Input.GetKeyDown(KeyCode.L)){
                currentDivision.UpdateLightIntensity(1);
            }
            if(Input.GetKeyDown(KeyCode.P)){
                currentDivision.UpdateLightColor(1);
            }
            if(Input.GetKeyDown(KeyCode.O)){
                currentDivision.ToggleLights();
            }
        }
    }

    void FixedUpdate()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(moveX, moveY);
        Vector3 direction = moveDirection.y * transform.forward + moveDirection.x * transform.right;

        rb.AddForce(direction * walkSpeed * 10, ForceMode.Force);
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
        ILightDivision lightDivision = other.GetComponent<ILightDivision>();
        if(lightDivision){
            currentDivision = lightDivision;
        }
    }
    private void OnTriggerExit(Collider other) {
        ILightDivision lightDivision = other.GetComponent<ILightDivision>();
        if(currentDivision == lightDivision){
            currentDivision = null;
        }
    }
}
