using Unity.Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    CharacterController controller;
    [SerializeField] CinemachineCamera virtualCamera;
    [SerializeField] Transform lookAtTarget;

    [Tooltip("Minimum vertical angle")] public float minPitch = -30f;
    [Tooltip("Maximum vertical angle")] public float maxPitch = 30f; 
    [Tooltip("Minimum horizontal angle")] public float minYaw = -45f;
    [Tooltip("Maximum horizontal angle")] public float maxYaw = 45f;

    [Header("Input")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float mouseSensitivity = 150f;
    [SerializeField] float zoomSpeed = 2f;

    float moveInput;
    float turnInput;
    float mouseX;
    float mouseY;
    bool mouseR;
    float pitch;
    float yaw;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Confine the cursor to the game window
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        HandleMovement();
    }

    void HandleMovement()
    {
        //GroundMovement();
        HandleTurn();
        HandleZoom();
    }

    void HandleTurn()
    {
        mouseX *= mouseSensitivity * Time.deltaTime;
        mouseY *= mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        yaw = Mathf.Clamp(yaw, minYaw, maxYaw);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0);
    }

    void HandleZoom()
    {
        float min = 2f;
        float max = 3.5f;
        var cur = lookAtTarget.transform.localPosition;

        float zoom = cur.z + (zoomSpeed * Time.deltaTime * (mouseR ? 1 : -1));

        zoom = Mathf.Clamp(zoom, min, max);
        cur.z = zoom;
        lookAtTarget.localPosition = cur;
    }

    void GroundMovement()
    {
        var move = new Vector3(turnInput, 0, moveInput);

        move *= moveSpeed;

        controller.Move(move * Time.deltaTime);
    }

    
    void HandleInput()
    {
        moveInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");

        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        mouseR = Input.GetButton("Fire2");
    }
}
