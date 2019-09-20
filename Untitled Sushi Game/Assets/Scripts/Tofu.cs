using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Tofu : MonoBehaviour
{
    GameObject player;

    NavMeshAgent agent;
    public GameObject[] waypoints;
    int dest = 0;

    Quaternion rotation;

    RaycastHit wall;
    RaycastHit above;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.destination = waypoints[dest].transform.position;

        rotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = rotation;

        if (!agent.pathPending && agent.remainingDistance < 0.1f)
        {
            NextPoint();
        }
    }

    void NextPoint()
    {
        dest = (dest + 1) % waypoints.Length;
        agent.destination = waypoints[dest].transform.position;
    }
}
