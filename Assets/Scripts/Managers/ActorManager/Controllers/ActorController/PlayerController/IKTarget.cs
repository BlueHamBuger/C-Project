using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKTarget : MonoBehaviour
{
    Rigidbody rigidbody;
    
    public Transform UParm;
    
    // 三维 的向量 
        // 作为 方向
        //判断其为 顺时针 还是 负
    public Vector3 CollisionDir{
        get;
        private set;
    }


    private void Start() {
        rigidbody = transform.GetComponent<Rigidbody>();
    }
    private void FixedUpdate() {
        updateRot();
    }

    private void OnCollisionStay(Collision other) {
        CollisionDir = other.GetContact(0).normal;
        MessageManager.Invoke("IKTargetCol",CollisionDir,transform);
    }

    private void OnCollisionExit(Collision other) {
        CollisionDir = Vector3.zero;
        MessageManager.Invoke("IKTargetCol",CollisionDir,transform);
    }

    public void updateRot(){
        Quaternion q = Quaternion.FromToRotation(transform.right,transform.position - UParm.position);
        rigidbody.MoveRotation(q*rigidbody.rotation);
    }



}
