using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    public Camera firstCam;
    public Camera thirdCam;
    public Transform thirdCamTarget;
    public Vector3 thirdCamOffset;

    public float speed;
    public float mouseSensitivity;
    public float cameraRotationLimit;
    public float zoomSpeed;
    public float zoomMin;
    public float zoomMax;
    public float jumpForce;

    private Vector3 moveVelocity = Vector3.zero;
    private Vector3 rbodyRotation = Vector3.zero;
    private Vector3 jumpVector = Vector3.zero;
    private bool isSprinting = false;
    private bool isGrounded = true;
    private float camRotation = 0f;
    private float currentCamRotation = 0f;
    private float thirdCamTargetYRotation = 0f;
    private float zoom = -5f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //Calculate movement velocity as a 3D vector
        float xMove = Input.GetAxisRaw("Horizontal");
        float zMove = Input.GetAxisRaw("Vertical");

        Vector3 moveHorizontal = transform.right * xMove;
        Vector3 moveVertical = transform.forward * zMove;

        //Toggle Sprint
        if(Input.GetKey(KeyCode.LeftShift))
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }

        //Final movement vector
        if(!isSprinting)
        {
            moveVelocity = (moveHorizontal + moveVertical).normalized * speed;
        }
        else
        {
            moveVelocity = (moveHorizontal + moveVertical).normalized * (speed * 2);
        }

        //Calculate rotation as a 3D vector, turning character model and camera
        float yRotation = Input.GetAxisRaw("Mouse X");

        rbodyRotation = new Vector3(0f, yRotation, 0f) * mouseSensitivity;
        thirdCamTargetYRotation = yRotation * mouseSensitivity;

        //Calculate rotation as a 3D vector, turning camera only
        float xRotation = Input.GetAxisRaw("Mouse Y");

        camRotation = xRotation * mouseSensitivity;

        //Calculate thirdPersonCam zoom
        zoom += Input.GetAxisRaw("Mouse ScrollWheel") * zoomSpeed;

        //Calculate jump vector
        if(isGrounded && Input.GetButtonDown("Jump"))
        {
            jumpVector = Vector3.up * jumpForce;
        }
    }

    //Fixed update is called 50 times per second, used for physics calculations
    private void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
        ControlThirdPersonCamPosition();
    }

    void PerformMovement()
    {
        //Move the player
        if(moveVelocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
        }

        //Perform jump, and stop jumping
        if(jumpVector != Vector3.zero)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            jumpVector = Vector3.zero;
        }
    }

    void PerformRotation()
    {
        //Turn the player
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rbodyRotation));

        //Rotate the firstPersonCam to look up/down
        if(firstCam != null)
        {
            //Set cam rotation and clamp it
            currentCamRotation -= camRotation;
            currentCamRotation = Mathf.Clamp(currentCamRotation, -cameraRotationLimit, cameraRotationLimit);

            //Apply the cam rotation to the camera
            firstCam.transform.localEulerAngles = new Vector3(currentCamRotation, 0f, 0f);
        }

        //Rotate the thirdPersonCam to look up/down
        if(thirdCam != null)
        {
            //Set cam rotation and clamp it
            currentCamRotation -= camRotation;
            currentCamRotation = Mathf.Clamp(currentCamRotation, -cameraRotationLimit, cameraRotationLimit);

            //Apply the cam rotation to the third cam's target
            thirdCamTarget.transform.localEulerAngles = new Vector3(currentCamRotation, thirdCamTargetYRotation, 0f);

            //Ensure thirdCam is always looking at target
            thirdCam.transform.LookAt(thirdCamTarget);
        }
    }

    void ControlThirdPersonCamPosition()
    {
        //Clamp zoom, due to zoom being negative numbers Max & Min are "reversed"
        zoom = Mathf.Clamp(zoom, zoomMax, zoomMin);

        //Adjust Z transform of thirdCam's offset
        thirdCamOffset.z = zoom;

        //Apply thirdCamOffset to thirdCam
        thirdCam.transform.localPosition = thirdCamOffset;
    }


    //Used to determine if player is on the ground
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }
}
