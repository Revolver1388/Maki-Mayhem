using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wasabi : MonoBehaviour
{
    GameObject player;

    public float range = 2;
    public bool hit = false;
    Transform target;
    NavMeshAgent agent;
    Vector3 newPos;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");

        newPos = Random.insideUnitSphere * 2;
        agent.destination = newPos;
    }

    void Update()
    {
        
        if (!agent.pathPending && agent.remainingDistance < 0.1f)
        {
            FindPath();
        }

        if (Vector3.Distance(transform.position, player.transform.position) <= range)
        {
            ChasePlayer();
        }
        else
        {
            FindPath();
        }
    }

    void FindPath()
    {
        newPos = RandomNavSphere(transform.position, 3, -1);
        agent.destination = newPos;
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randomDir = Random.insideUnitSphere * dist;
        randomDir += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDir, out navHit, dist, layermask);
        return navHit.position;
    }

    void ChasePlayer()
    {
        
        if (!hit)
        {
            agent.destination = player.transform.position;
            hit = true;
        }
        else if(hit)
            StartCoroutine(StopSmashing());
    }
    IEnumerator StopSmashing()
    {
        yield return new WaitForSeconds(1.5f);
        hit = false;

    }
}
