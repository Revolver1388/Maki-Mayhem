using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshTest : MonoBehaviour
{
    NavMeshAgent A;
    public Transform testTarget; 

    // Start is called before the first frame update
    void Start()
    {
        A = GetComponent<NavMeshAgent>();

        A.SetDestination(testTarget.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
