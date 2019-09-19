using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiceGrainRayCast : MonoBehaviour
{
    public float groundSearchLength = 0.6f;

    public bool offGround = false;

    RiceGrain riceGrain;

    private void Awake()
    {
        riceGrain = GetComponentInParent<RiceGrain>();
    }

    private bool isOnGround()
    {
        Vector3 lineStart = transform.position;
        Vector3 vectorToSearch = new Vector3(lineStart.x, lineStart.y - groundSearchLength, lineStart.z);

        Debug.DrawLine(lineStart, vectorToSearch);

        return Physics.Linecast(lineStart, vectorToSearch);
    }

   
    private void FixedUpdate()
    {
        if (riceGrain.deactivated)
            return; 

        offGround = !isOnGround();
    }

  
}
