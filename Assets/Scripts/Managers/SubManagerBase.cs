using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubManagerBase : MonoBehaviour
{
    public ActorManager am;
    public SubManagerBase(ActorManager am){
        this.am = am;
    }

    public virtual void Init(){}

}
