﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    Scene currentScene;
    string sceneName;
    Renderer s_Render;
    public Material s_Original;
    public Material s_New;
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

    [Header("Player Movement Attributes")]
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
        [Header("Jump Attributes")]
    //float gravity = 4;
    [Range(0, 3.5f)]
    public float g_accel;
    float j_counter;
    float j_Stregnth = 0.5f;
    float j_time;
    public float j_maxTime = 0.2f;
    float h_time = 0.2f;
    [SerializeField]
    float j_height;
    bool j_on = false;
    bool j_release = true;
    bool f_on = false;
    float startOfJump;
    [Range(0, 5)]
    public float j_Time;
    [Range(0, 5)]
    public float j_Duration;
    [Range(0, 20)]
    public float x;
    [Range(0, 5)]
    public float f_Time;
    [Range(0, 20)]
    public float y;
    [Range(0, 20)]
    public float z;
    public Vector3 ending;
    public Vector3 start;
    #endregion

    [Header("Enemy and Life Attributes")]
    //Lives
    public List<GameObject> lives = new List<GameObject>();
    public float nbrLives = 3;
    public bool takeDmg = false;

    //Enemies
    GameObject sake;
    //Wasabi
    bool onFire = false;
    bool wasabi = false;
    [Range(0, 4)]
    public float w_Time;
    [Range(0, 4)]
    public float w_Duration;
    public float w_height;
    //Tofu Stuff
    public float t_Time;
    public float t_Duration;
    public float t_Distance;
    // Start is called before the first frame update
    void Start()
    {
        if (!cam)
        {
            cam = Camera.main;
        }
        if (!s_Render)
        {
            s_Render = GetComponent<Renderer>();
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

        if (wasabi)
        {
            s_Render.material.color = Color.Lerp(s_Original.color, Color.red, Mathf.PingPong(Time.time, .5f));
        }
        #region Wall Checks

        if (isGrounded())
        {
            if(ground.collider.gameObject.tag == "Tofu")
            {
                TofuBounceUp();
            }
            if(ground.collider.gameObject.tag == "sticky")
            {
                StartCoroutine(StickyTime());
            }
        }

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

            if (wall.collider.gameObject.tag == "Tofu")
            {
                TofuBounceBack();
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
        if (isGrounded() && !j_on || !isGrounded() && j_on)
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
        float g_Step =  EaseIn(f_Time / g_accel);
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
        ending = new Vector3(rb.position.x + (movement.x/4), y + t, rb.position.z + (movement.z/2));
        transform.position = Vector3.Slerp(transform.position, ending, EaseIn(j_Time / j_Duration) * Time.deltaTime);
    }
    public static float EaseIn(float t)
    {
        return t * t;
    }

    public void TofuBounceBack()
    {
        Vector3 b_End = new Vector3(rb.position.x, rb.position.y + t_Distance / 2, rb.position.z - t_Distance);
        transform.position = Vector3.Slerp(transform.position, b_End, EaseIn(t_Time / t_Duration) * Time.deltaTime);

        //In editor, set t_Time to 1.7 and t_Duration to 0.4
    }

    public void TofuBounceUp()
    {
        ending = new Vector3(rb.position.x + (movement.x / 4), 6.0f + rb.position.y, rb.position.z + (movement.z / 4));
        transform.position = Vector3.Slerp(transform.position, ending, EaseIn(1.0f / 0.4f) * Time.deltaTime);
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

    // Add bounce to player hop!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    IEnumerator UpSpeed()
    {
        wasabi = true;
        //ending = new Vector3(rb.position.x + (movement.x / 4), w_height + rb.position.y, rb.position.z - (w_height/ 2));
        //transform.position = Vector3.Slerp(transform.position, ending, EaseIn(w_Time / w_Duration) * Time.deltaTime);
        p_Speed = 4.5f;
        yield return new WaitForSeconds(3);
        p_Speed = 2;
        wasabi = false;
        s_Render.material = s_Original;
    }

    IEnumerator StickyTime()
    {
        p_Speed = 1;
        yield return new WaitForSeconds(2);
        p_Speed = 2;
    }
}

