using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeCol : MonoBehaviour
{
    bool coled = false;
    public float width; // 武器刃部的宽度
    Vector3 velBeforeHit;
    Rigidbody r;
    Rigidbody pr;
    BoxCollider b;
    Rigidbody tarRigid;
    Collider tarCol;
    Quaternion rotationBeforHit;
    public bool inbedding = false; // 是否正处于 嵌入中或是 嵌入状态
    Coroutine inbedingCour = null;
    // 



    private void Awake()
    {
        r = GetComponent<Rigidbody>();
        pr = transform.parent.GetComponent<Rigidbody>();
        b = GetComponentInChildren<BoxCollider>();

        if (b)
        {
            // 注意 bounds.extents 的 对应的 长度似乎是
            // 世界坐标系空间下的 长度
            width = b.bounds.size.x;
        }

        // 通过使用Coroutine 来使用 waitForUpdate 
        //这是因为 其调用时间是物理模拟的最末位置
        //这样 可以在 进入 Collision  之前获取到上一帧的 transform信息
        StartCoroutine("PostFixedUpdate");
    }
    public void OnPickUp(EdgeCol e)
    {
        width = e.width;
        b = e.b;
    }
    private IEnumerator PostFixedUpdate()
    {
        while (true)
        {
            velBeforeHit = r.velocity;
            rotationBeforHit = r.transform.rotation;
            yield return new WaitForFixedUpdate();
        }
    }
    public void OnCollisionEnter(Collision other)
    {
        // coled = true;
        // UnityEditor.EditorApplication.isPaused = true;
        // Debug.DrawRay(r.position, r.transform.right * 100);
        if (coled&&inbedding) return;
        Vector3 speedOnEdge = Vector3.Project(velBeforeHit, transform.right);
        float dotResult = Vector3.Dot(speedOnEdge, transform.right);
        if (dotResult >= 10.0f)
        {
            PreGetIn(other,speedOnEdge);
        }
    }
    public void PreGetIn(Collision other,Vector3 speedOnEdge,Global.Del0 callback=null,Transform sender = null)
    {
        r.isKinematic = true;
        pr.isKinematic = true;
        Physics.IgnoreCollision(b, other.collider, true);

        tarCol = other.collider;
        coled = true;
        tarRigid = other.rigidbody;
        r.transform.rotation = rotationBeforHit;
        if(inbedingCour!= null)StopCoroutine(inbedingCour);
        inbedingCour = StartCoroutine(GetIn(speedOnEdge,callback,sender));
    }


    IEnumerator GetIn(Vector3 speedOnEdge,Global.Del0 callback,Transform sender)
    {
        //UnityEditor.EditorApplication.isPaused = true;
        // 在开始就行 插入逻辑之前
        //先将碰撞体 旋转量设置到 碰撞前的状态
        //并通过  ComputePenetration 来计算出 碰撞体 和目标碰撞体恰好接触的位置
        // Vector3 speedOnEdge = Vector3.Project(velBeforeHit, transform.right);
        Vector3 outdir; float outdist;
        bool penetrated = Physics.ComputePenetration(b, transform.position, transform.rotation, tarCol, tarCol.transform.position, tarCol.transform.rotation, out outdir, out outdist);
        if(!penetrated) {
            OnJointBreak(0);
            yield break;
        }
        float cos = Mathf.Abs(Vector3.Dot(outdir.normalized, speedOnEdge.normalized));
        r.transform.position = r.transform.position - speedOnEdge.normalized * (outdist / cos);
        inbedding = true;

        // 开始进行 插入逻辑
        yield return new WaitForFixedUpdate();
        r.isKinematic = false;
        pr.isKinematic = false;
        r.useGravity = false;
        pr.useGravity = false; 
        Vector3 startVel = Vector3.ClampMagnitude(speedOnEdge / 10,2.5f);
        r.velocity = pr.velocity =startVel;
        // 需要保持之前的 rotation 因为
        // 结果物理引擎的 作用之后 rotation 可能会发生变化
        //r.transform.rotation = rotationBeforHit;
        Vector3 inPoint = r.position;
        float dist = Vector3.Distance(inPoint, r.position);
        for (int i =1;i<4 && dist <= width / 2 && penetrated;i++)
        {
            dist = Vector3.Distance(inPoint, r.position);
            r.velocity -= startVel/4;
            pr.velocity -= startVel/4;
            yield return new WaitForFixedUpdate();
            penetrated = Physics.ComputePenetration(b, transform.position, transform.rotation, tarCol, tarCol.transform.position, tarCol.transform.rotation, out outdir, out outdist);
        }
        r.velocity = Vector3.zero;
        pr.velocity = Vector3.zero;
        pr.angularVelocity = Vector3.zero;

        if (!penetrated){
            if(callback!=null)callback();
            OnJointBreak(0); //手动break；
            yield break;
        } 
        // 当 插入之后

        HingeJoint hinge = gameObject.AddComponent<HingeJoint>();
        if(tarRigid){
            hinge.connectedBody = tarRigid;
        }
        hinge.axis = Vector3.up;
        hinge.useLimits = true;
        JointLimits jointLimits = new JointLimits();
        jointLimits.min = -5f;
        jointLimits.max = 5f;
        hinge.limits = jointLimits;
        hinge.breakForce = 5000f*dist/width;
        r.drag = 0;

        // 设置 父物体
        pr.constraints = ~(RigidbodyConstraints.FreezeRotationY);
        pr.mass = 1;
        pr.drag = 0.5f;
        coled = false;
        if(callback!=null)callback();
        // 触发事件
        //MessageManager.Invoke("OnStabOver",sender);
    }

    private void OnJointBreak(float breakForce)
    {
        pr.mass = 1.0f;
        pr.drag = 0.0f;
        pr.constraints = RigidbodyConstraints.None;
        // 去除
        //pr.isKinematic = true;
        pr.isKinematic = false;
        r.isKinematic = false;
        Physics.IgnoreCollision(b, tarCol, false);

        r.useGravity = true;
        pr.useGravity = true;
        inbedding = false;
    }


}
