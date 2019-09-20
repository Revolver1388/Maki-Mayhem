using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    Scene currentScene;
    string sceneName;

    GameObject gameMan;

    //RigidBody Conditions
    Rigidbody rb;
    Vector3 playerSize;
    float heightOffGround = -0.1f;

    //Raycast Parametres
    RaycastHit ground;
    float groundCheckDist = 0.3f;
    RaycastHit wall;
    float wallCheckDist = 0.1f;
    RaycastHit above;
    float aboveCheckDist = .13f;

    float distWall = 0.1f;

    //Player Movement Parametres
    Camera cam;
    public Vector3 movement;
    public float m_FB;
    public float m_LR;
    float m_Step;
    float rotateSpeed = 8;
    public float p_Speed = 2;

    //sprint 
    float runTime = 0;
    float maxRunTime = 1.3f;

    //Jumping Parametres

    #region jump stuff
    float gravity = 4;
    [Range(0, 3.5f)]
    public float g_accel;
    float j_counter;
    float j_Stregnth = 0.5f;
    float j_time;
    float j_maxTime = 0.2f;
    float h_time = 0.2f;
    [SerializeField]
    float j_height;
    bool j_on = false;
    bool j_release = true;
    bool f_on = false;
    float startOfJump;
    [Range(0, 5)]
    public float time;
    [Range(0, 5)]
    public float duration;
    [Range(0, 20)]
    public float x;
    [Range(0, 20)]
    public float y;
    [Range(0, 20)]
    public float z;

    public Vector3 ending;
    public Vector3 start;
    #endregion

    //Lives
    public List<GameObject> lives = new List<GameObject>();
    public float nbrLives = 3;
    public bool takeDmg = false;

    //Enemies
    GameObject sake;
    bool wasabi = false;


    // Start is called before the first frame update
    void Start()
    {
        if (!cam)
        {
            cam = Camera.main;
        }
        rb = GetComponent<Rigidbody>();
        playerSize = GetComponent<Collider>().bounds.size;

        currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;

        sake = GameObject.FindGameObjectWithTag("Sake");
        gameMan = GameObject.FindGameObjectWithTag("GameMan");
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGrounded() && !j_on || j_release && !isGrounded() || j_time >= j_maxTime && !isGrounded())
        {
            f_on = true;
        }
        if (isGrounded())
        {
            startOfJump = transform.position.y;
            f_on = false;
            j_time = 0.0f;
        }
        if (Input.GetButtonDown("Jump") && j_time < j_maxTime)
        {
            j_on = true;
            j_release = false;
        }
        else if (Input.GetButtonDown("Jump") && j_time >= j_maxTime)
        {          
            j_on = false;
            j_release = true;
        }
        if (Input.GetButtonUp("Jump") || j_time >= j_maxTime)
        {
            j_on = false;
            j_release = true;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            runTime += 1 * Time.deltaTime;
            if (runTime < maxRunTime)
            {
                p_Speed = 3;
            }
            else if (runTime >= maxRunTime)
            {
                p_Speed = 2;
            }
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) && !wasabi)
        {
            runTime = 0;
            p_Speed = 2;
        }

        #region Wall Checks

        if (isBlockedForward())
        {
            float hitWall = wall.point.z + (playerSize.z/2 - distWall);
            rb.position = new Vector3(rb.position.x + movement.x * m_Step, rb.position.y + movement.y * m_Step, hitWall);

            if (wall.collider.gameObject.tag == "Sake" && !takeDmg)
            {
                StartCoroutine(DamageTimer());
                nbrLives -= 1;
            }

            if (wall.collider.gameObject.tag == "Wasabi")
            {
                StartCoroutine("UpSpeed");
            }
        }

        if (isBlockedBackward())
        {
            float hitWall = wall.point.z + (playerSize.z/2 + distWall);
            rb.position = new Vector3(rb.position.x + movement.x * m_Step, rb.position.y + movement.y * m_Step, hitWall);

            if (wall.collider.gameObject.tag == "Sake" && !takeDmg)
            {
                StartCoroutine(DamageTimer());
                nbrLives -= 1;
            }

            if (wall.collider.gameObject.tag == "Wasabi")
            {
                StartCoroutine("UpSpeed");
            }
        }

        if (isBlockedLeft())
        {
            float hitWall = wall.point.x + (playerSize.x/2 + distWall);
            rb.position = new Vector3(hitWall, rb.position.y + movement.y * m_Step, rb.position.z + movement.z * m_Step);

            if (wall.collider.gameObject.tag == "Sake" && !takeDmg)
            {
                StartCoroutine(DamageTimer());
                nbrLives -= 1;
                
            }

            if (wall.collider.gameObject.tag == "Wasabi")
            {
                StartCoroutine("UpSpeed");
            }
        }

        if (isBlockedRight())
        {
            float hitWall = wall.point.x - (playerSize.x/2 - distWall);
            rb.position = new Vector3(hitWall, rb.position.y + movement.y * m_Step, rb.position.z + movement.z * m_Step);

            if (wall.collider.gameObject.tag == "Sake" && !takeDmg)
            {
                StartCoroutine(DamageTimer());
                nbrLives -= 1;
            }

            if (wall.collider.gameObject.tag == "Wasabi")
            {
                StartCoroutine("UpSpeed");
            }
        }

        if (isBlockedUp())
        {
            j_on = false;
            j_release = true;
            f_on = true;
        }
        #endregion  

        for (int i = 0; i < lives.Count; i++)
        {
            lives[i].SetActive(i < nbrLives);
        }

        if (rb.position.y <= -0.1f)
        {
            nbrLives = 0;
        }

        if (nbrLives <= 0)
        {
            StartCoroutine("GameOver");
        }
    }
    private void FixedUpdate()
    {
        #region Player Movement
        if (isGrounded() && !j_on)
        {
            m_Step = p_Speed * Time.deltaTime;
            m_LR = Input.GetAxis("Horizontal");
            m_FB = Input.GetAxis("Vertical");
            movement = cam.transform.forward * m_FB;
            movement += transform.right * m_LR;
            movement.y = 0.0f;
            Quaternion rotation = cam.transform.rotation;
            rotation.x = 0.0f;
            rotation.z = 0.0f;
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, rotation, rotateSpeed * Time.deltaTime));
            rb.MovePosition(rb.position + movement * m_Step *.25f);
        }
        #endregion

        #region Jump Stuff
        if (j_on && !j_release)
        {
           
            StartCoroutine("WaitforInput", h_time);
        }
        if (f_on)
        {
            Fall();
        }
        if (isGrounded() && !j_on)
        {
            float grounded = ground.point.y + (playerSize.y + heightOffGround);
            rb.position = new Vector3(rb.position.x + movement.x * m_Step, grounded, rb.position.z + movement.z * m_Step);
        }
        #endregion
    }

    #region BoundaryChecks

    public bool isGrounded()
    {
        Vector3 lineStart = transform.position;
        Vector3 vectorToSearch = new Vector3(lineStart.x, lineStart.y - groundCheckDist, lineStart.z);

        Debug.DrawLine(lineStart, vectorToSearch);

        return Physics.Linecast(lineStart, vectorToSearch, out ground);
    }

    public bool isBlockedForward()
    {
        int layerMask = 1 << 2;
        layerMask = ~layerMask;
        Vector3 lineStart = transform.position;
        Vector3 vectorToSearch = new Vector3(lineStart.x, lineStart.y, lineStart.z + wallCheckDist);

        Debug.DrawLine(lineStart, vectorToSearch, Color.red);

        return Physics.Linecast(lineStart, vectorToSearch, out wall, layerMask);
    }

    public bool isBlockedBackward()
    {
        int layerMask = 1 << 2;
        layerMask = ~layerMask;
        Vector3 lineStart = transform.position;
        Vector3 vectorToSearch = new Vector3(lineStart.x, lineStart.y, lineStart.z - wallCheckDist);

        Debug.DrawLine(lineStart, vectorToSearch, Color.blue);

        return Physics.Linecast(lineStart, vectorToSearch, out wall, layerMask);
    }

    public bool isBlockedRight()
    {
        int layerMask = 1 << 2;
        layerMask = ~layerMask;
        Vector3 lineStart = transform.position;
        Vector3 vectorToSearch = new Vector3(lineStart.x + wallCheckDist, lineStart.y, lineStart.z);

        Debug.DrawLine(lineStart, vectorToSearch, Color.green);

        return Physics.Linecast(lineStart, vectorToSearch, out wall, layerMask);
    }

    public bool isBlockedLeft()
    {
        int layerMask = 1 << 2;
        layerMask = ~layerMask;
        Vector3 lineStart = transform.position;
        Vector3 vectorToSearch = new Vector3(lineStart.x - wallCheckDist, lineStart.y, lineStart.z);

        Debug.DrawLine(lineStart, vectorToSearch, Color.magenta);

        return Physics.Linecast(lineStart, vectorToSearch, out wall, layerMask);
    }

    public bool isBlockedUp()
    {
        int layerMask = 1 << 9;
        Vector3 lineStart = transform.position;
        Vector3 vectorToSearch = new Vector3(lineStart.x, lineStart.y + aboveCheckDist, lineStart.z);

        Debug.DrawLine(lineStart, vectorToSearch, Color.grey);

        return Physics.Linecast(lineStart, vectorToSearch, out above, layerMask);
    }
    #endregion

    void Fall()
    {
        movement.y = 0.0f;
        float yPos = rb.position.y;
        float g_Step = gravity;
        movement.y = -(yPos + g_Step);
        rb.MovePosition(rb.position + movement * Time.deltaTime);
    }

    void Jump(float t)
    {
        //*****************************OLD JUMP CODE **********************************************************
        //float yPos = rb.position.y;
        //float yVel = j_Stregnth + g_accel * Time.deltaTime;
        //float jumpHeight = yPos + yVel;
        //movement.y = yPos + yVel;
        //rb.MovePosition(rb.position + movement * Time.deltaTime);
        //*****************************************************************************************************

        j_time += Time.deltaTime;
        ending = new Vector3(rb.position.x + (movement.x/4), y + t, rb.position.z + (movement.z/4));
        transform.position = Vector3.Slerp(transform.position, ending, EaseIn(time / duration) * Time.deltaTime);
    }



    IEnumerator WaitforInput(float t)
    {
        if (j_time < j_maxTime)
        {
            Jump(startOfJump);
        }
        yield return new WaitForSeconds(t);
    }

    IEnumerator GameOver()
    {
        gameMan.GetComponent<GameManager>().gameOver = true;
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator DamageTimer()
    {
        takeDmg = true;
        yield return new WaitForSeconds(.3f);
        takeDmg = false;
    }
    public static float EaseIn(float t)
    {
        return t * t;
    }

    // Add bounce to player hop!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    IEnumerator UpSpeed()
    {
        wasabi = true;
        ending = new Vector3(rb.position.x + (movement.x / 4), 2.0f + rb.position.y, rb.position.z + (movement.z / 4));
        transform.position = Vector3.Slerp(transform.position, ending, EaseIn(1.0f / 0.4f) * Time.deltaTime);
        p_Speed = 4.5f;
        yield return new WaitForSeconds(3);
        p_Speed = 2;
        wasabi = false;
    }
}

