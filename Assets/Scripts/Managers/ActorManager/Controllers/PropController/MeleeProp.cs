using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;
using RootMotion;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(FixedJoint))]
public class MeleeProp : PropBase
{
    [LargeHeader("Melee")]

    [Tooltip("Switch to a CapsuleCollider when the prop is picked up so it behaves more smoothly when colliding with objects.")]
    public CapsuleCollider capsuleCollider;

    [Tooltip("The default BoxCollider used when this prop is not picked up.")]
    public BoxCollider boxCollider;

    public EdgeCol edgeCol{
        get;
        private set;
    }
    public override bool IsStabIn{ //判定是否处于插入状态
        get{
            return edgeCol.inbedding;
        }
    }

    [Tooltip("Temporarily increase the radius of the capsule collider when a hitting action is triggered, so it would not pass colliders too easily.")]
    public float actionColliderRadiusMlp = 1f;

    [Tooltip("Temporarily set (increase) the pin weight of the additional pin when a hitting action is triggered.")]
    [Range(0f, 1f)] public float actionAdditionalPinWeight = 1f;

    [Tooltip("Temporarily increase the mass of the Rigidbody when a hitting action is triggered.")]
    [Range(0.1f, 10f)] public float actionMassMlp = 1f;

    [Tooltip("Offset to the default center of mass of the Rigidbody (might improve prop handling).")]
    public Vector3 COMOffset;

    private float defaultColliderRadius;
    private float defaultMass;
    private float defaultAddMass;


    public void StartAction(float duration)
    {
        //StopAllCoroutines();
       // StartCoroutine(Action(duration));
    }

    private void FixedUpdate() {
        // 动态更新 collider 大小
        // if(propHandler != null){
        //     Rigidbody rigid = muscle.GetComponent<Rigidbody>();
        //     float factor = 1+rigid.velocity.sqrMagnitude/actionColliderRadiusMlp;
        //     capsuleCollider.radius = defaultColliderRadius * Mathf.Clamp(factor,1,1.6f);
        //     propHandler.muscles[1].props.pinWeight = propHandler.attachedMuscleGroup.muscles[0].props.pinWeight * Mathf.Clamp(factor,1,3.0f);
        // }

        // mass 的增加 可选

    }

    private void OnDrawGizmos() {

    }

    // public IEnumerator Action(float duration)
    // {
    //     capsuleCollider.radius = defaultColliderRadius * actionColliderRadiusMlp;
    //     r.mass = defaultMass * actionMassMlp;

    //     int additionalPinMuscleIndex = additionalPinTarget != null ? propRoot.puppetMaster.GetMuscleIndex(additionalPinTarget) : -1;
    //     if (additionalPinMuscleIndex != -1)
    //     {
    //         propRoot.puppetMaster.muscles[additionalPinMuscleIndex].props.pinWeight = actionAdditionalPinWeight;
    //     }

    //     yield return new WaitForSeconds(duration);

    //     capsuleCollider.radius = defaultColliderRadius;
    //     r.mass = defaultMass;
    //     if (additionalPinMuscleIndex != -1)
    //     {
    //         propRoot.puppetMaster.muscles[additionalPinMuscleIndex].props.pinWeight = additionalPinWeight;
    //     }
    // }

    protected override void OnStart()
    {
        edgeCol = additionalPinTarget.GetComponent<EdgeCol>();
        // Initiate stuff here.
        // defaultColliderRadius = capsuleCollider.radius;

        // r = muscle.GetComponent<Rigidbody>();
        // r.centerOfMass += COMOffset;
        // defaultMass = r.mass;
    }

    protected override void OnPickUp(PropHandler propHandler)
    {

        //propHandler.muscles[1].transform.gameObject.GetComponent<EdgeCol>().OnPickUp(edgeCol);
    }

    protected override void OnDrop()
    {
        // Called when the prop has been dropped.
        // capsuleCollider.radius = defaultColliderRadius;
        // r.mass = defaultMass;

        // if(boxCollider){
        //     capsuleCollider.enabled = false;
        //     boxCollider.enabled = true;
        // }
    }


}
