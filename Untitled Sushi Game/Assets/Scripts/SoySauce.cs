using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class SoySauce : MonoBehaviour
{
	public NavMeshAgent agent;

    public Transform rayCastPoint; 
	public HealthScript player;
    public float speed = 2f;
    public float avoidenceTimer;
    public float groundSearchLength = 0.6f;

    public Transform soyPrefabSpawn; 
    public GameObject soyPrefab;

    float minRange = 1;
    float maxtange = 3;
    public Animator anim;

    
    private void Awake()
    {
        anim = GetComponent<Animator>();
        //anim.enabled = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<HealthScript>();

        agent.Warp(transform.position);
        
        StartCoroutine(AnnoyPlayer());

    }


    private bool isOnGround()
    {
        Vector3 lineStart = transform.position;
        Vector3 vectorToSearch = new Vector3(lineStart.x, lineStart.y - groundSearchLength, lineStart.z);

        Debug.DrawLine(lineStart, vectorToSearch);

        return Physics.Linecast(lineStart, vectorToSearch);
    }

    private void Update()
    {
        if (!isOnGround())
            Debug.Log("Not over table");


        
     
    }

    IEnumerator Puke()
    {

        anim.SetTrigger("Puke");
        //yield return new WaitForSeconds(1f);
        Instantiate(soyPrefab, soyPrefabSpawn.position, transform.rotation);
        StartCoroutine(SwitchOffAnimator());
        yield break; 
    }

    IEnumerator SwitchOffAnimator()
    {
        yield return new WaitForSeconds(1f);
        anim.ResetTrigger("Puke");
        StartCoroutine(AnnoyPlayer());

    }

    IEnumerator AnnoyPlayer()
    {

        Vector3 away = player.soyPoint.transform.position - transform.position;
        //transform.LookAt(player.transform);
        while (away.sqrMagnitude > minRange * minRange && away.sqrMagnitude < maxtange * maxtange)
        {

			agent.destination = player.soyPoint.transform.position;

			//Vector3 newDir = Vector3.RotateTowards(transform.forward, away, step, 0.0f);
			//transform.rotation = Quaternion.LookRotation(newDir);
              if(away.sqrMagnitude < (maxtange * maxtange) / 2)
               {
                StartCoroutine(Puke());
                yield break; 
               }
			
            away = player.soyPoint.transform.position - transform.position;
            yield return null;


        }
        yield return new WaitForFixedUpdate();

        

        StartCoroutine(AnnoyPlayer());

    }

    //IEnumerator SoySlick()
    //{
     //   agent.isStopped = true; 


  //  }


}
