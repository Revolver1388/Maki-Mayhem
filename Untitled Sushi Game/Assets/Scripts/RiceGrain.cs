using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class RiceGrain : MonoBehaviour
{

	public NavMeshAgent agent;
	public bool isPlayerOnNavMesh;


    public Transform player;
    public float speed  = 2;
    public float avoidenceTimer;

    public HealthScript playerHealth; 

    public RiceGrainRayCast forward;
    public RiceGrainRayCast left;
    public RiceGrainRayCast right;

    public bool deactivated = true; 

    public Vector3 avoidenceDirection;

    public MeshRenderer mesh;

    public float maxRange = 0.2f;

    public GameObject l_Manager;

    RiceGrainRayCast[] riceGrainsRays = new RiceGrainRayCast[2]; 

    public void Activate(Vector3 pos)
    {


		//transform.position = pos;
		//transform.rotation = player.rotation;
		agent.enabled = true;
		deactivated = false;
		agent.Warp(pos);
		mesh.enabled = true;
		//StartCoroutine(Follow());

		//agent.SetDestination(player.transform.position);


		StartCoroutine(CheckIfPlayerIsOnNavMesh());


	}

	public void DeActivate()
    {
		agent.enabled = false;
        deactivated = true; 
        mesh.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
		agent.enabled = false; 
        l_Manager = GameObject.FindGameObjectWithTag("l_Manager");
		//agent = GetComponent<NavMeshAgent>();
        riceGrainsRays[0] = left;
        riceGrainsRays[1] = right;
        mesh.enabled = false; 
        
    }

	// Update is called once per frame


	private void FixedUpdate()
	{



		if (agent.enabled)
		{
			
			agent.destination = player.position;

		
			Vector3 towards = player.position - transform.position;
			if (towards.sqrMagnitude < maxRange * maxRange)
			{
				DeActivate();
				Debug.Log("I got there!");
				l_Manager.GetComponent<LevelManager>().r_Current += 1;

			}



		}
	}


		float onMeshThreshold = 3;

	public bool IsAgentOnNavMesh()
	{
		Vector3 agentPosition = player.transform.position;
		NavMeshHit hit;

		// Check for nearest point on navmesh to agent, within onMeshThreshold
		if (NavMesh.SamplePosition(agentPosition, out hit, onMeshThreshold, NavMesh.AllAreas))
		{
			// Check if the positions are vertically aligned
			if (Mathf.Approximately(agentPosition.x, hit.position.x)
				&& Mathf.Approximately(agentPosition.z, hit.position.z))
			{
				// Lastly, check if object is below navmesh
				//playerPositionOnNav = hit.position; 
				return agentPosition.y >= hit.position.y;
			}
		}

		return false;
	}


    IEnumerator CheckIfPlayerIsOnNavMesh()
	{
		yield return new WaitForFixedUpdate();
		isPlayerOnNavMesh = IsAgentOnNavMesh();

	}



	#region old non-NavMesh


	IEnumerator Follow()
    {
      
        Vector3 towards = player.position - transform.position;
        
        while (towards.sqrMagnitude > maxRange * maxRange)
        {
            float step = speed * Time.deltaTime;


            if (HeadingOffGround())
            {
                StartCoroutine(HeadInNewDirection(ChangeDirection(), 0));
                yield break;
            }

            //if (isInFront())
            //    Debug.Log("Something's in front of me");

            Vector3 newDir = Vector3.RotateTowards(transform.forward, towards, step, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);


            yield return new WaitForFixedUpdate();
            transform.position += towards.normalized * step;
            towards = player.position - transform.position;

           

            yield return null;


        }

        l_Manager.GetComponent<LevelManager>().r_Current += 1;
        DeActivate();
        Debug.Log("I got there!");
    }



    IEnumerator HeadInNewDirection(Vector3 directionChange, float time)
    {

        Debug.Log(directionChange);
        float step = speed * Time.deltaTime;

        while (time < avoidenceTimer)
        {
            transform.position += directionChange * step;
            Debug.Log(time);
            time += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Done");
        StartCoroutine(Follow());

    }

    bool HeadingOffGround()
    {
        for (int i = 0; i < riceGrainsRays.Length; i++)
        {
            if (riceGrainsRays[i].offGround)
            {
                Debug.Log(riceGrainsRays[i].name + " is off");
                return true;
            }
        }

        return false; 

    }

    Vector3 ChangeDirection()
    {
      
        if(left.offGround)
            return transform.forward + transform.right / 2; 
        
        
            return transform.forward + -transform.right / 2;


    }


    bool isInFront()
    {
        return Physics.Linecast(forward.transform.position, forward.transform.forward);

    }
	#endregion

}
