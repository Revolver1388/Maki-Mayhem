using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class Sake : MonoBehaviour
{
    public Rigidbody rB;
    GameObject player;
    public GameObject look;
    Quaternion rotation;

    public GameObject[] waypoints;
    public int dest = 0;
    public Vector3 x;
    public float speed = 3;

    public GameObject spawn;
    public GameObject proj;

    public bool idle;
    public bool attack;
    public bool coolDown;

    Vector3 moveDir;
    Vector3 startPos;

    bool shooting = false;
    int shots = 0;
    int maxShots = 3;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        idle = true;

        rotation = transform.rotation;
        startPos = transform.position;
    }

    void Update()
    {
        if (idle)
        {
            coolDown = false;
            transform.rotation = rotation;
            transform.position = new Vector3(transform.position.x, startPos.y, transform.position.z);

            transform.position = Vector3.MoveTowards(transform.position, waypoints[dest].transform.position, 1 * Time.deltaTime);

            if (Vector3.Distance(transform.position, waypoints[dest].transform.position) < 1)
            {
                dest = (dest + 1) % waypoints.Length;
            }
        }

        if (attack)
        {
            shooting = true;     

            if (shots >= maxShots)
            {
                attack = false;
                coolDown = true;
                shots = 0;
            }
        }
        else
        {
            shooting = false;
        }

        if (shooting)
        {
            var rotation = Quaternion.LookRotation(player.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 4 * Time.deltaTime);
            Shoot();
        }

        if (coolDown)
        {
            idle = false;
            transform.position = new Vector3(transform.position.x, startPos.y, transform.position.z);
            moveDir = transform.position - player.transform.position;
            StartCoroutine("CoolDown");
        }

        if (Vector3.Distance(transform.position, player.transform.position) < 2)
        {
            idle = false;
            attack = true;
        }
        else
        {
            idle = true;
            attack = false;
        }
    }

    public void KillEnemy()
    {
        gameObject.SetActive(false);
    }

    public void Shoot()
    {
        GetComponent<ProjectileShoot>().ShootProjectile();
        shots += 1;
    }

    IEnumerator CoolDown()
    {
        transform.Translate(moveDir.normalized * 1 * Time.deltaTime);
        yield return new WaitForSeconds(3);
        idle = true;
    }
}
