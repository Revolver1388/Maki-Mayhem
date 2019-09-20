using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soy_Liquid : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Disappear());
    }

   IEnumerator Disappear()
    {

        yield return new WaitForSeconds(5f);
        this.gameObject.SetActive(false);
    }
}
