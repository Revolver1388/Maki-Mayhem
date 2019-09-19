using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealthScript : MonoBehaviour
{
    public Transform spawnPoint; 
    public GameObject ricePrefab;
    public RiceGrain[] myRice = new RiceGrain[20];
    public Transform soyPoint;
    public LevelManager l_Manager;

    // Start is called before the first frame update
    void Start()
    {

        for (int i = 0; i < myRice.Length; i++)
        {
            GameObject go = Instantiate(ricePrefab);
            myRice[i] = go.GetComponent<RiceGrain>();
            myRice[i].player = this.transform;
            myRice[i].playerHealth = this; 
            myRice[i].DeActivate();
        }


        StartCoroutine(RiceTrail(0));

    }

    IEnumerator RiceTrail(int i)
    {
        yield return new WaitForSeconds(Random.Range(3,6));
        myRice[i].Activate(spawnPoint.position);
         l_Manager.r_Current -= 1; 
        if(i < myRice.Length - 1)
        {
            StartCoroutine(RiceTrail(i+1));
        }

    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
            Debug.Break();
    }
}
