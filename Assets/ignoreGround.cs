using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ignoreGround : MonoBehaviour
{
    public Collider a;
    public Collider b;
    public Collider ground;
    public RootMotion.Dynamics.PuppetMaster Puppet;

    // Start is called before the first frame update
    void Start()
    {
        //Physics.IgnoreCollision(a,ground,true);
        Collider[] cs = a.gameObject.GetComponents<Collider>();
        foreach(var c in cs){
            Physics.IgnoreCollision(c,ground,true);
        //     // IgnoreCol(c);
        }
        Physics.IgnoreCollision(b,ground,true);
        //IgnoreCol(b);
        //Puppet.internalCollisions = false;
    }
    void IgnoreCol(Collider c){
        foreach(var m in Puppet.muscles){
            Collider[] css = m.transform.GetComponents<Collider>();
            foreach(var col in css){
                Physics.IgnoreCollision(c,col,true);
            }
        }
    }
}
