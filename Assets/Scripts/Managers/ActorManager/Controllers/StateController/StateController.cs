using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;

public partial class StateController : MonoBehaviour
{
    // 默认属性值
    ActorManager am;

    // StateController  持有 behaviourBase  用于状态的控制
    // behaviourPuppet 即 为 正常状态
    // behaviourFall 为 失衡状态
    // BehaviourPuppet behaviourPuppet = null;
    // BehaviourFall behaviourFall = null;


    ConsciousBehaviour consciousBeha;

    UnConsciousBehaviour unConsciousBeha;

    public ConsciousBehaviour BehaPuppet
    {
        get
        {
            return consciousBeha;
        }
    }
    public UnConsciousBehaviour BehaFall
    {
        get
        {
            return unConsciousBeha;
        }
    }


    // 描述 角色当前的 动作状态
    public ActionState actionState = new ActionState
    {
        handAction = Global.HandAction.FREE,
        bodyAction = Global.BodyAction.GROUND,
        formerBodyAction = Global.BodyAction.NONE,
        formerHandAction = Global.HandAction.NONE,
    };

    public Global.HandAction HandAction
    {
        get
        {
            return actionState.handAction;
        }
        set
        {
            actionState.formerHandAction = actionState.handAction;
            actionState.handAction = value;
            if (actionState.formerHandAction != value)
            {
                MessageManager.Invoke<Global.HandAction>("HandStateChange", value, transform);
                UpdateStateProps();
            }
        }
    }
    public Global.HandAction formerHandAction
    {
        get { return actionState.formerHandAction; }
    }

    public Global.BodyAction BodyAction
    {
        get
        {
            // if (actionState.bodyAction!= Global.BodyAction.FALLING&&BehaPuppet.state == ConsciousBehaviour.State.Unpinned)
            // {
            //     BodyAction = Global.BodyAction.FALLING;
            // }
            return actionState.bodyAction;
        }
        set
        {
            actionState.formerBodyAction = actionState.bodyAction;
            actionState.bodyAction = value;
            if (actionState.formerBodyAction != value)
            {
                MessageManager.Invoke<Global.BodyAction>("BodyStateChange", value, transform);
                UpdateStateProps();
            }
        }
    }
    public Global.BodyAction formerBodyAction
    {
        get { return actionState.formerBodyAction; }
    }











    public void Init(ActorManager am)
    {
        this.am = am;

        foreach (var behaivour in am.puppet.behaviours)
        {
            if (behaivour is ConsciousBehaviour)
            {
                consciousBeha = behaivour as ConsciousBehaviour;
            }
            else if (behaivour is UnConsciousBehaviour)
            {
                unConsciousBeha = behaivour as UnConsciousBehaviour;
            }
        }
        //behaviourFall.OnPreActivate += am.actorController.Fall;
        BehaPuppet.onRegainBalance.unityEvent.AddListener(am.actorController.OnRegainBalance);
        BehaPuppet.onLoseBalance.unityEvent.AddListener(am.actorController.Fall);

        BehaPuppet.onGetUpProne.unityEvent.AddListener(am.actorController.GetUp);
        BehaPuppet.onGetUpSupine.unityEvent.AddListener(am.actorController.GetUp);



        // 初始化 muscleGroup
        updateEntry = UpdateDelegate;
        ActorParts.MuscleGroup[] muscleGroup = am.actorParts.MuscleGroups;
        baseProps = new GroupProp[muscleGroup.Length];
        for (int i = 0; i < muscleGroup.Length; i++)
        {
            baseProps[i] = new GroupProp(muscleGroup[i], updateEntry);
        }

        BehaPuppet.InitMuscleGroup(baseProps);
        InitProps();
    }

    private void Update()
    {
        //TODO 统一 在数值发生 变化的时候 进行 属性的 update
        UpdateProps();
        // if(am.frozen){
        //     am.puppet.Kill();
        //     // foreach(var m in am.puppet.muscles){

        //     //     var d = new JointDrive();
        //     //     m.joint.xMotion = ConfigurableJointMotion.Free;
        //     //     m.joint.yMotion = ConfigurableJointMotion.Free;
        //     //     m.joint.zMotion = ConfigurableJointMotion.Free;

        //     //     d.positionSpring =1000f;
        //     //     d.maximumForce = 1000f;
        //     //     m.joint.xDrive = d;
        //     //     m.joint.yDrive = d;
        //     //     m.joint.zDrive = d;
        //     //     m.joint.targetPosition = m.transform.localPosition;
        //     //     m.joint.slerpDrive = d;
        //     //     m.rigidbody.constraints = RigidbodyConstraints.FreezePosition; 
        //     // }
        //     am.frozen = false;

        // }

    }

    private void setDefault()
    {
        curHP = maxHP;
        curSP = maxSP;
    }


    public void AddExtraProps(ActorParts.MuscleGroup muscleGroup)
    {
        GroupProp g = new GroupProp(muscleGroup, updateEntry);
        if (extraProps == null)
        {
            extraProps = new GroupProp[1] { g };
            return;
        }
        System.Array.Resize(ref extraProps, extraProps.Length + 1);
        extraProps[extraProps.Length - 1] = g;
    }


    // 状态变换调用函数 在 bodyAction 的setter 中进行调用。
    public void State2Fall()
    {
        BehaPuppet.SetState(ConsciousBehaviour.State.Unpinned);
    }
    private void State2Die()
    {
        //TODO 死亡相关
        am.puppet.Kill();
        // 使用 kill 来改变 puppet 状态到 die
        // 可以使用 puppetMaster 的ondeath 回调 来调用一些 方法


        //TODO 其它如 游戏ui 游戏保存等等内容的调用
    }










    // 内部类/struct

    [System.Serializable]
    public struct ActionState
    {
        //TODO 左右手 分别使用对应等等 HandAction
        public Global.HandAction handAction;
        public Global.BodyAction bodyAction;
        public Global.HandAction formerHandAction;
        public Global.BodyAction formerBodyAction;

        //TODO 可能 有 更多的 决定 是否可以输入的状态变量
        public bool CanInput
        {
            get
            {
                switch (bodyAction)
                {
                    case Global.BodyAction.FALLING:
                    case Global.BodyAction.STUN:
                    case Global.BodyAction.GETUP:
                        return false;
                    default:
                        return true;
                }
            }
        }

    };

}
