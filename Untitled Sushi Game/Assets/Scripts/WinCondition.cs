using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    GameObject gameMan;


    void Start()
    {
        gameMan = GameObject.FindGameObjectWithTag("GameMan");
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            gameMan.GetComponent<GameManager>().gameWin = true;
        }
    }
}
