using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTest : MonoBehaviour
{
    public Transform addTran;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("ground"))
        {
            Vector3 rigidVel = addTran.GetComponent<Rigidbody>().velocity.normalized;
            float dotReuslt = Vector3.Dot(rigidVel,addTran.right);
            float dotReuslt2 = Vector3.Dot(rigidVel,-other.GetContact(0).normal);
        }
    }
}
