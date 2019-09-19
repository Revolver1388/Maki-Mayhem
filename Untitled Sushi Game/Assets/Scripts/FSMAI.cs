using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMAI : MonoBehaviour
{
    public enum Behave { idle, chase, cooldown, wander };
    public Behave OBehave;
    float rotateSpeed = 4;
    Quaternion currentRotation;
    // Start is called before the first frame update
    void Start()
    {
        
        OBehave = Behave.idle;
    }

    // Update is called once per frame
    void Update()
    {
        currentRotation = transform.rotation;

        switch (OBehave)
        {
            case Behave.idle:
                idle();
                break;
            case Behave.chase:
                chase();
                break;
            case Behave.cooldown:
                break;
            case Behave.wander:
                break;
            default:
                break;
        }

    }

    void idle()
    {
        Vector3 rotateAngle = new Vector3(90, 0, 0);
        Quaternion angle = new Quaternion(0,ReverseAngle(rotateAngle.x), 0, 0);
       // Quaternion reverseAngle = new Quaternion(0, -rotateAngle.x, 0, 0);

        if (currentRotation != angle)
        {
        transform.rotation = Quaternion.Slerp(currentRotation,angle, rotateSpeed * Time.deltaTime);
        }
        if (transform.rotation.x == angle.x)
        {
            transform.rotation = Quaternion.Slerp(currentRotation, angle, rotateSpeed * Time.deltaTime);
        }
    }

    void chase()
    {
        
    }

    public float ReverseAngle(float x)
    {
        float y = -x;

        return y;
    }
    IEnumerator Switch(float t)
    {
        yield return new WaitForSeconds(t);

    }
}
