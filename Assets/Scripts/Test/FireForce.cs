using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireForce : MonoBehaviour
{
    public float speed;
    public bool fire =true;
    void Update()
    {
        if(fire){
            //GetComponent<Rigidbody>().AddForce(force);
            GetComponent<Rigidbody>().velocity = transform.right*speed;
            fire = false;
        }
    }
}
