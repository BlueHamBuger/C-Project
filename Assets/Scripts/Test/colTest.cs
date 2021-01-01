using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colTest : MonoBehaviour
{
    Rigidbody r;
    public PropHandler propHandler;

    bool coled = false;
    Vector3 velBeforeHit;
    Collider tarCol;
    public EdgeCol edgeCol;

    private void Start()
    {
        r = GetComponent<Rigidbody>();
        StartCoroutine("postFixedUpdate");
    }
    IEnumerator postFixedUpdate()
    {
        while (true)
        {
            velBeforeHit = r.velocity;
            yield return new WaitForFixedUpdate();
        }
    }


    private void OnCollisionEnter(Collision other)
    {
        if (coled) return;
        Vector3 speedOnEdge = Vector3.Project(velBeforeHit, transform.right);
        float dotResult = Vector3.Dot(speedOnEdge, transform.right);
        if (dotResult >= 20.0f)
        {
            edgeCol = propHandler.currentProp.transform.GetComponentInChildren<EdgeCol>();
            propHandler.DropImmediate();
            //UnityEditor.EditorApplication.isPaused = true;
            StopCoroutine("GetIn");
            StartCoroutine("GetIn", other);
            coled = false;
        }
    }


    IEnumerator GetIn(Collision other)
    {
        Vector3 edgeVel = Vector3.Project(velBeforeHit, transform.right);
        yield return new WaitForFixedUpdate();
        edgeCol.PreGetIn(other, edgeVel);
    }
}