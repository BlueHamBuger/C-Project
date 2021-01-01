using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testPickUpWeapon : MonoBehaviour
{
    public PropBase propBase;
    private void OnTriggerEnter(Collider other) {
        ActorManager am = other.GetComponentInParent<ActorManager>();
        
        
        if(!am) return;
        PropHandler ph = am.GetComponentInChildren<PropHandler>();
        ph.currentProp =   propBase;
        gameObject.active = false;
    }
}
