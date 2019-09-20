using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Object : MonoBehaviour
{
    Collider player;
    PlayerMovement pm;
    RaycastHit ground;
    float groundCheckDist = 0.3f;
    float gravity = 0.57f;



    Rigidbody body;
    private void Awake()
    {
        pm = FindObjectOfType<PlayerMovement>();
        player = pm.GetComponent<Collider>();
        body = GetComponent<Rigidbody>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other == player)
        {
            Vector3 heading = transform.position - player.transform.position;
            float distance = heading.magnitude;
            Vector3 direction = heading / distance;

            StartCoroutine(KnockBack(direction));


        }
    }

    private void FixedUpdate()
    {
        if (!isGrounded())
            Fall();
    }

    public bool isGrounded()
    {
        Vector3 lineStart = transform.position;
        Vector3 vectorToSearch = new Vector3(lineStart.x, lineStart.y - groundCheckDist, lineStart.z);

        Debug.DrawLine(lineStart, vectorToSearch);

        return Physics.Linecast(lineStart, vectorToSearch, out ground);
    }
    public static float Easin(float t)
    {
        return t * t;
    }

    void Fall()
    {
        float yPos = transform.position.y;
        float g_Step = Easin(gravity / 0.69f);
        transform.position = Vector3.Slerp(transform.position, new Vector3(transform.position.x, -(yPos + g_Step), transform.position.z), 1 * Time.deltaTime);
        // Vector3.MovePosition(rb.position + movement * Time.deltaTime);
    }

    IEnumerator KnockBack(Vector3 direction)
    {
        float timer = 0;

        while (timer < pm.p_Speed)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + direction / 100, pm.p_Speed);
            timer += Time.deltaTime;
            yield return null;
        }



    }
}
