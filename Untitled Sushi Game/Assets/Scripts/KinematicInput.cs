using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicInput : MonoBehaviour
{
    // Horizontal movement parameters
    public float speed = 10.0f;
    public float runTime = 0;
    public float maxRunTime = 2;
    // Jump and Fall parameters
    public int maxJumps = 2;
    public int jumps = 0;
    public float maxJumpSpeed = 1.5f;
    public float maxFallSpeed = -2.2f;
    public float timeToMaxJumpSpeed = 0.2f;
    public float deccelerationDuration = 0.0f;
    public float maxJumpDuration = 1.2f;
    
    // Jump and Fall helpers
    bool jumpStartRequest = false;
    bool jumpRelease = false;
    bool isMovingUp = false;
    bool isFalling = false;
    float currentJumpDuration = 0.0f;
    float gravityAcceleration = -9.8f;
    float jumpHeight = 0.0f;
    public float groundSearchLength = 0.6f;
    RaycastHit currentGroundHit;
    RaycastHit platformAbove;
    RaycastHit wall;
    // Rotation Parameters
    float angleDifferenceForward = 0.0f;

    // Components and helpers
    Rigidbody rigidBody;
    Vector2 input;
    Vector3 playerSize;
    public float height;

    public Camera cammy;
    private float vertical;
    private float horizontal;
    public float rotateSpeed = 30;

    // Debug configuration
    public GUIStyle myGUIStyle;

    Color colorStartMajorDamage = Color.red;
    Color colorNoDamage = Color.blue;
    Color colorStartMediumDamage = Color.yellow;
    float damageDisplayDuration = 0.8f;
    Renderer rend;
    float currentMajorDamageTimer = 0.0f;
    float currentMediumDamageTimer = 0.0f;
    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        playerSize = GetComponent<Collider>().bounds.size;
        
    }

    void Start()
    {
        jumpStartRequest = false;
        jumpRelease = false;
        isMovingUp = false;
        isFalling = false;
        rend = GetComponent<Renderer>();
        colorNoDamage = rend.material.color;
        if (!cammy)
        {
            cammy = Camera.main;
        }
    }

    void Update()
    {
         horizontal = Input.GetAxis("Horizontal");
         vertical = Input.GetAxis("Vertical");

        input = new Vector2();
        input.x = horizontal;
        input.y = vertical;

        if (Input.GetButtonDown("Jump") && jumps == 0)
        {
            jumpStartRequest = true;
            jumpRelease = false;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            jumpRelease = true;
            jumpStartRequest = false;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            runTime += 1 * Time.deltaTime;
            if (runTime < maxRunTime)
            {
                speed = 15;
            }
            else if(runTime >= maxRunTime)
            {
                speed = 10;
            }
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            runTime = 0;
            speed = 10;
        }
        Movement();
    }

    void FixedUpdate()
    {

        //Raycasts
        if (Physics.Raycast(transform.position, transform.up, out platformAbove, groundSearchLength))
        {
            if (platformAbove.collider.tag == "redPlat")
            {
                Debug.Log("hitting plat");
                StartFalling();
            }
        }   
    }

    private void Movement()
    {
        Vector3 cammyRight = cammy.transform.TransformDirection(Vector3.right);
        Vector3 cammyFront = cammy.transform.TransformDirection(Vector3.forward);
        cammyRight.y = 0;
        cammyFront.y = 0;
        cammyRight.Normalize();
        cammyFront.Normalize();

        Vector3 movement = transform.right * input.x * speed * Time.deltaTime;
        movement += transform.forward * input.y * speed * Time.deltaTime;
        movement.y = 0.0f;
        Vector3 targetPosition = rigidBody.position + movement;

        float targetHeight = 0.0f;

        if (!isMovingUp && jumpStartRequest && isOnGround())
        {
            isMovingUp = true;
            jumpStartRequest = false;
            currentJumpDuration = 0.0f;
        }

        if (isMovingUp)
        {
            if (jumpRelease || currentJumpDuration >= maxJumpDuration)
            {
                StartCoroutine("WaitforInput", .2f);

            }
            else
            {
                if (jumps == 0)
                {
                    float currentYpos = rigidBody.position.y;
                    float newVerticalVelocity = maxJumpSpeed + gravityAcceleration * Time.deltaTime;
                    targetHeight = currentYpos + (newVerticalVelocity * Time.deltaTime) + (0.5f * maxJumpSpeed * Time.deltaTime * Time.deltaTime);
                    jumpHeight = targetHeight;
                    currentJumpDuration += Time.deltaTime;
                    jumps += 1;
                }
                if (jumps == 1)
                {
                    float currentYpos = rigidBody.position.y;
                    float newVerticalVelocity = maxJumpSpeed + gravityAcceleration * Time.deltaTime;
                    targetHeight = currentYpos + (newVerticalVelocity * Time.deltaTime) + (0.2f * maxJumpSpeed * Time.deltaTime * Time.deltaTime);
                    currentJumpDuration += Time.deltaTime;
                }
            }
        }
        else if (!isOnGround())
        {
            StartFalling();
        }

        if (isFalling)
        {
            if (isOnGround())
            {
                // End of falling state. No more height adjustments required, just snap to the new ground position
                jumps = 0;
                isFalling = false;
                targetHeight = currentGroundHit.point.y + (height + playerSize.y);
            }
            else
            {
                float currentYpos = rigidBody.position.y;
                float currentYvelocity = rigidBody.velocity.y;

                float newVerticalVelocity = maxFallSpeed + gravityAcceleration * Time.deltaTime;
                targetHeight = currentYpos + (newVerticalVelocity * Time.deltaTime) + (0.5f * maxFallSpeed * Time.deltaTime * Time.deltaTime);
            }
        }

        if (targetHeight > Mathf.Epsilon)
        {
            // Only required if we actually need to adjust height
            targetPosition.y = targetHeight;
        }

        Vector3 movementDirection = targetPosition - rigidBody.position;
        movementDirection.y = 0.0f;
        movementDirection.Normalize();

        Vector3 currentFacingXZ = transform.forward;
        currentFacingXZ.y = 0.0f;

        angleDifferenceForward = Vector3.SignedAngle(movementDirection, currentFacingXZ, Vector3.up);
        Vector3 targetAngularVelocity = Vector3.zero;
        targetAngularVelocity.y = angleDifferenceForward * Mathf.Deg2Rad;

        Vector3 currentRotation = transform.forward;

        transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, cammyFront * vertical + cammyRight * horizontal, rotateSpeed * Time.fixedDeltaTime, 0.0f));
        Quaternion syncRotation = transform.rotation;
        rigidBody.MovePosition(targetPosition);

        if (movement.sqrMagnitude > Mathf.Epsilon)
        {
            rigidBody.MoveRotation(transform.rotation);
        }
    }
    private bool isOnGround()
    {
        Vector3 lineStart = transform.position;
        Vector3 vectorToSearch = new Vector3(lineStart.x, lineStart.y - groundSearchLength, lineStart.z);

        Debug.DrawLine(lineStart, vectorToSearch);

        return Physics.Linecast(lineStart, vectorToSearch, out currentGroundHit);
    }

    private void OnDrawGizmos()
    {
        // Debug Draw last ground collision, helps visualize errors when landing from a jump
        if (currentGroundHit.collider != null)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(currentGroundHit.point, 0.05f);
        }
    }

    void StartFalling()
    {
        isMovingUp = false;
        isFalling = true;
        currentJumpDuration = 0.0f;
        jumpRelease = false;
    }

    IEnumerator WaitforInput(float t)
    {
        int x = 0;
        if (Input.GetButtonDown("Jump"))
        {
            x += 1;
            jumpStartRequest = true;
            jumpRelease = false;
        }
        else if (x == 0)
        yield return new WaitForSeconds(t);
        x = 0;
        StartCoroutine("Hover", .1f);
    }
    IEnumerator Hover(float t)
    {
        yield return new WaitForSeconds(t);

        StartFalling();
    }
}
