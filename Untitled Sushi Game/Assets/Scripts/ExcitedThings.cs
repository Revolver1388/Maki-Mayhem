using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExcitedThings : MonoBehaviour
{
    public GameObject player;
    public Vector3 newPos;
    [Range(0, 3)]
    public float time;
    [Range(0, 3)]
    public float distance;


    private Transform start;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        start = transform;
    }
    private void Update()
    {
        if(Vector3.Distance(transform.position, player.transform.position) < 2)
        {
            HappyToSeeYou();
        }
        if (Input.GetKey(KeyCode.O))
        {
            HappyToSeeYou();
        }
    }

    void HappyToSeeYou()
    {
        //if(transform.position.y <= start.position.y)
        //{
        //transform.position = Vector3.Lerp(start.position, new Vector3(start.position.x,newPos.y,start.position.z), EaseIn(time / distance) * Time.deltaTime);
        //}
        //else if(transform.position.y >= newPos.y)
        //{
        //    transform.position = Vector3.Lerp(new Vector3(start.position.x, newPos.y, start.position.z),start.position, EaseOut(time / distance) * Time.deltaTime);

        //}

    }

    public static float EaseIn(float t) {
        return t * t;
    }

    public static float EaseOut(float t)
    {
        return (2 - t) * t;
    }

    public static float EaseInOut(float t)
    {
        return -t * t * (2 * t - 3);
    }
}
