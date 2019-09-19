using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Turn on/off the mouse cursor")]
    public bool lockCursor = true;

    [Header("Invert Camera Y (set to off for mouse control)")]
    public bool invY;

    [Header("Player")]
    public GameObject Player;
    [SerializeField]
    float playerSpeed;
    [SerializeField]
    Vector3 playerFront;
    [Header("Camera Sensitivity and rotation smoothing(Suggested sensitivity = 4)")]
    [Range(1, 10)]
    public float sensitivity = 5;
    [Range(0,1)]
    public float rotationsmoothTime = 0.351f;
    float yAngle = 90;

    [Header("Distance from Player, Suggested Distance 2-3")]
    [Range(1, 10)]
    public float distFromPlayer = 3;
    float properDistance = 3;
    Vector2 pitchMinMax; //set to -15 and 90 respectively in editor

    [Header("Debug current camera rotations and eulers")]
    [SerializeField]
    Vector3 currentRotation;
    Vector3 eul;
    Vector3 smoothingVelocity;
    Vector3 temp;
    float yaw;
    float pitch;
    bool looking = false;

    [Header("Camera Field of View")]
    float i_FOV = 60;
    public float z_FOV = 30;
    float c_FOV;
    float smooth = 30;

    [Header("CameraRayCast")]
    public Camera main;
    public GameObject aimer;
    LayerMask layerMask;
    public float cameraOffsetX = .2f;
    void Start()
    {
        main = Camera.main;
        i_FOV = main.fieldOfView;
        properDistance = distFromPlayer;
        pitchMinMax = new Vector2(10,20);      
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    private void Update()
    {
        c_FOV = main.fieldOfView;
    }
    private void LateUpdate()
    {
        CamMovement();
    }
    void CamMovement()
    {
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
        yaw += Input.GetAxis("CamX") * sensitivity /*+ Input.GetAxis("MouseX") * sensitivity*/;
        if (invY)
        {
            invertY();
        }
        if (!invY)
        {
            nonInvertY();
        }
       // pitchMinMax = new Vector2(0, 12);
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
        eul = transform.eulerAngles;
        eul.x = 0;
        transform.eulerAngles = currentRotation;
        transform.position = Player.transform.position - (transform.forward - (transform.right * cameraOffsetX)) * CameraRaycast(distFromPlayer); 
    }

    void invertY()
    {
        pitch += Input.GetAxis("CamY") * sensitivity /*+ Input.GetAxis("MouseY") * sensitivity*/;
        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref smoothingVelocity, rotationsmoothTime);

    }
    void nonInvertY()
    {
        pitch -= Input.GetAxis("CamY") * sensitivity /*+ Input.GetAxis("MouseY") * sensitivity*/;
        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref smoothingVelocity, rotationsmoothTime);
    }

    public void switchInverseY()
    {
        invY = !invY;
    }

    float CameraRaycast(float x)
    {
        int layerMask = 1 << 2;
        layerMask = ~layerMask;
        RaycastHit hit;
        aimer.transform.LookAt(main.transform.position);
        if (Physics.Raycast(aimer.transform.position, aimer.transform.TransformDirection(Vector3.forward), out hit,10, layerMask))
        {
            Debug.DrawRay(aimer.transform.position, aimer.transform.TransformDirection(Vector3.forward), Color.blue, distFromPlayer);
            float dist = hit.distance;
            if (dist < distFromPlayer)
            {
                x = dist;
            }
            else 
            {
                x = properDistance;
            }
        }
        return x;
    }
}