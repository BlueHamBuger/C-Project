using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showCenterMass : MonoBehaviour
{
    private Rigidbody rigidbody;
    public Vector3 centerOffSet;
    private void Awake() {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.centerOfMass += centerOffSet;
    }
    private void OnDrawGizmos() {
        if(rigidbody){
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(rigidbody.worldCenterOfMass,0.1f);
        }
    }

    private void OnCollisionEnter(Collision other) {
        // if(other.gameObject.layer == LayerMask.NameToLayer("ground")){
        //      print(Vector3.Dot(other.relativeVelocity,transform.right));    
        // }
    }
}
