using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeDetector : MonoBehaviour
{
    //
    PropHandler propHandler;
    Vector3 velBeforeHit;
    Vector3 rightBeforeHit;
    Rigidbody bladeRigid; // 刃部当前绑定的 rigidbody
    EdgeCol edgeCol;
    bool coled; // 是否正在就行插入操作？

    public void init(PropHandler proph) // 需要传入 edgeCol 其是 武器嵌入 操作 的入口
    {
        this.bladeRigid = GetComponent<Rigidbody>();
        propHandler = proph;

        // 事件监听
        //MessageManager.AddListener("OnStabOver",OnStabOver);
    }

    // 使用 后
    IEnumerator postFixedUpdate()
    {
        while (true)
        {
            velBeforeHit = bladeRigid.velocity;
            rightBeforeHit = transform.right;
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnCollisionEnter(Collision other)
    {

        if (coled) return;
        if(other.gameObject.layer != LayerMask.NameToLayer("ground")) return;
        if(other.transform.root == transform.root) return; // 同属于 一个 AM 下
        Vector3 speedOnEdge = Vector3.Project(velBeforeHit, rightBeforeHit);
        float dotResult = Vector3.Dot(speedOnEdge, rightBeforeHit);
        if (dotResult >= 25.0f)
        {
            coled = true;
            StopCoroutine("GetIn");
            MessageManager.Invoke("OnStabIn",transform);
            StartCoroutine("GetIn", other);
        }
    }

    IEnumerator GetIn(Collision other)
    {
        Vector3 edgeVel = Vector3.Project(velBeforeHit,rightBeforeHit);
        yield return new WaitForFixedUpdate();
        edgeCol.PreGetIn(other, edgeVel,OnStabOver,transform);
        //yield return new WaitUntil()
    }
    public void OnEnableDetector(EdgeCol e){
        edgeCol =  e;
        coled = false;
    }

    private void OnDisable()
    {
        StopCoroutine("postFixedUpdate");
    }
    private void OnEnable() {
        if(propHandler)
            StartCoroutine("postFixedUpdate");
    }
    //外部回调
    private void OnStabOver(){
        coled = false;
    }
}
