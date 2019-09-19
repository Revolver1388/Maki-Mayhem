using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExcitedThings : MonoBehaviour
{
    public GameObject player;
    public Vector3 newPos;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        if(Vector3.Distance(transform.position, player.transform.position) < 2)
        {
            HappyToSeeYou();
        }
    }

    void HappyToSeeYou()
    {

    }

    float EaseIn(float t) {
        return t * t;
    }
}
