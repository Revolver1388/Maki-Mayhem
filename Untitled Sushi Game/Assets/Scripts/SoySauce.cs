using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class SoySauce : MonoBehaviour
{
	public NavMeshAgent agent;


	public HealthScript player;
    public float speed = 2f;
    public float avoidenceTimer;



    float minRange = 1;
    float maxtange = 3; 



    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<HealthScript>();
     

        
        StartCoroutine(AnnoyPlayer());

    }


    IEnumerator AnnoyPlayer()
    {

        Vector3 away = player.soyPoint.transform.position - transform.position;

        while (away.sqrMagnitude > minRange * minRange && away.sqrMagnitude < maxtange * maxtange)
        {

			agent.destination = player.soyPoint.transform.position;

			//Vector3 newDir = Vector3.RotateTowards(transform.forward, away, step, 0.0f);
			//transform.rotation = Quaternion.LookRotation(newDir);

			
            away = player.soyPoint.transform.position - transform.position;
            yield return null;


        }
        yield return new WaitForFixedUpdate();

        

        StartCoroutine(AnnoyPlayer());

    }


}
