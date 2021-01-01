using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;

public class PropHandler : MonoBehaviour
{

    [Tooltip("Reference to the PuppetMaster component.")]
    /// <summary>
    /// Reference to the PuppetMaster component.
    /// </summary>
    public ActorParts actorParts;



    [Tooltip("Is there a Prop connected to this PropRoot? Simply assign this value to connect, replace or drop props.")]
    /// <summary>
    /// Is there a Prop connected to this PropRoot? Simply assign this value to connect, replace or drop props.
    /// </summary>
    public PropBase currentProp;


    /// <summary>
    /// [0]存储的是 武器的把柄 muscleindex
    ///     [1] 存储的则是 武器的攻击部位的 muscleindex
    /// </summary>
    public Muscle[] muscles;
    public BladeDetector bladeDetector; //挂载在 武器的攻击部位对应的 muscle处，

    public ActorParts.MuscleGroup attachedMuscleGroup;

    public PropBase holdingProp; // 当前没有自由握起，处于僵持状态的prop
    private PropBase lastProp;
    private bool fixedUpdateCalled;
    private Vector3 defaultLocalPos;
    private Vector3 defaultLocalEular;

    [Tooltip("If a prop is connected, what will it's joint be connected to?")]
    /// <summary>
    /// If a prop is connected, what will it's joint be connected to?
    /// </summary>
    public Rigidbody connectTo;

    /// <summary>
    /// Dropping/Picking up normally works in the fixed update cycle where joints can be properly connected. Use this to drop a prop immediatelly.
    /// </summary>
    public void DropImmediate()
    {
        if (lastProp == null) return;
        //actorParts.puppet.RemoveMuscleRecursive(lastProp.muscle, true, false, MuscleRemoveMode.Sever);
        if(lastProp.propHandler)// 没有drop过
            lastProp.Drop();
        actorParts.puppet.RemoveMuscle(muscles[0]);
        currentProp = null;
        lastProp = null;
    }

    public void Init(ActorParts actorParts, ActorParts.MuscleGroup muscleGroup){
        this.actorParts = actorParts;
        attachedMuscleGroup = muscleGroup;
        // If currentProp has been assigned, it will be picked up AS IS, presuming it is already linked with the joints and held in the right position.
        // To pick up the prop from ground, assign it after Awake, for example in Start.
        if (currentProp != null) currentProp.StartPickedUp(this);

        muscles = new Muscle[] { actorParts.puppet.muscles[15], actorParts.puppet.muscles[16] };
        System.Array.Resize(ref actorParts.puppet.muscles, actorParts.puppet.muscles.Length - 2);
        defaultLocalPos = muscles[0].target.localPosition;
        defaultLocalEular = muscles[0].target.localEulerAngles;
        bladeDetector = muscles[1].transform.gameObject.AddComponent<BladeDetector>();
        bladeDetector.enabled = false;bladeDetector.init(this);
        foreach (var m in muscles){
            m.transform.gameObject.SetActive(false);
        }

        // 事件绑定
        MessageManager.AddListener("OnStabIn",OnStabIn,transform);
    }

    void Update()
    {
        if (!fixedUpdateCalled) return;

        //If dropped by another script or PuppetMaster behaviour
        // if (currentProp != null && lastProp == currentProp )
        // {
        //     currentProp.Drop();
        //     currentProp = null;
        //     lastProp = null;
        // }
    }

    void FixedUpdate()
    {
        fixedUpdateCalled = true;
        // if(holdingProp&&muscles[0].transform.gameObject.activeSelf)
        //     actorParts.puppet.RemoveMuscle(muscles[0]); // 当前为 holding状态 并且 muscle没有被 remove ,则进行 remove
        if (currentProp == lastProp) return;

        // Dropping current prop
        if (currentProp == null)
        {
            DropImmediate();
        }

        // Picking up to an empty slot
        else if (lastProp == null)
        {
            MeleeProp m = currentProp as MeleeProp;
            if(m!=null){ // 是近战武器
                if(m.edgeCol) EnableDetector(m.edgeCol); // 有刀刃
            }
            AttachProp(currentProp);
        }

        // Switching props
        else if (lastProp != null && currentProp != null)
        {
            // actorParts.puppet.RemoveMuscleRecursive(lastProp.muscle, true, false, MuscleRemoveMode.Sever);
            // AttachProp(currentProp);
        }
        lastProp = currentProp;
    }


    private void AttachProp(PropBase prop)
    {
        // 确保 肌肉 的位置和 target的位置一致
        muscles[0].target.localPosition = defaultLocalPos;
        muscles[0].target.localEulerAngles = defaultLocalEular;
        muscles[0].transform.position = muscles[0].target.position;
        muscles[0].transform.rotation = muscles[0].target.rotation;

        prop.PickUp(this);
        actorParts.puppet.AddMuscle(muscles[0]);
        if (prop.additionalPinTarget != null)
        {
            actorParts.puppet.AddMuscle(muscles[1]);
        }
    }

    public void EnableDetector(EdgeCol e = null){
        bladeDetector.enabled = true;
        if(e) // 是注册新的 edgeCol
            bladeDetector.OnEnableDetector(e);
    }
    public void DisableDetector(){
        bladeDetector.enabled = false;
    }

    // 外部回调函数
    private void OnStabIn(){ //之所以不使用 dropImmediate 是因为 drop 之后 muscle被置为 disable 将无法触发之后的 Couroutine
        holdingProp = currentProp; // 武器嵌入场景中
        //actorParts.puppet.RemoveMuscleRecursive(lastProp.muscle, true, false, MuscleRemoveMode.Sever);
        lastProp.Drop();    
        currentProp = null;
        DisableDetector();
    }


}
