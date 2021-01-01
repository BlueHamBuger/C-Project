using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;
using RootMotion.FinalIK;
public partial class PlayerController : ActorController
{
    private AnimationHash animationHash;
    public Transform attackPlane;


    // IK
    public FullBodyBipedIK fullBodyBipedIK;

    public HumanoidParts humanoidParts;


    /// <summary>
    /// 用来保存前一帧 rigidtrans 的position 这是为了 解决 移动中rigidTran 移动了 而 瞄准的ik 没有移动导致的IK 漂移问题
    /// </summary>
    Vector3 formerIKDir;



    /// <summary>
    /// 表示
    /// </summary>
    Vector2 SkillOffsetVec = Vector2.zero;
    public float curBias = 1.0f;

    // 状态改变 必须 设置 former




    // 移动 速度的 系数

    float atdeRange = 1.0f;

    // 是否可以 jump
    private bool canJump = true;
    // 跳过当前 手部状态更新
    protected override AnimationHashBase animationHashBase
    {
        get { return animationHash; }
    }





    // 消息函数
    public override void Init(ActorManager am)
    {
        base.Init(am);
        // 变量初始化
        HumanoidParts temp = new HumanoidParts(am.puppet);
        temp.aimPart = humanoidParts.aimPart;
        temp.rigidRoot = humanoidParts.rigidRoot;
        humanoidParts = temp;
        animationHash = new AnimationHash(animator);
        actorBehaviours = new PlayerBehaviours(this, new FBIKChain[] { fullBodyBipedIK.solver.rightArmChain }, new IKEffector[] { fullBodyBipedIK.solver.rightHandEffector });

        // 事件的绑定:
        MessageManager.AddListener("OnStabIn", OnStabIn, transform);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    // 将 动画的  位移和旋转直接作用到 角色的碰撞体上


    // 控制状态
    // 判断是否在地面
    private void OnCollisionEnter(Collision other)
    {
        int layerMask = 1 << other.gameObject.layer;
        if ((layerMask & am.state.BehaFall.raycastLayers.value) != 0 && curBodyAction != Global.BodyAction.FALLING && curBodyAction != Global.BodyAction.GETUP)
        {
            if (Vector3.Dot(other.GetContact(0).normal, transform.up) > 0.8f)
                Ground();
        }
    }
    private void OnCollisionStay(Collision other)
    {
        if (canJump)
        {
            int layerMask = 1 << other.gameObject.layer;
            if ((layerMask & am.state.BehaFall.raycastLayers.value) != 0 && curBodyAction != Global.BodyAction.FALLING && curBodyAction != Global.BodyAction.GETUP)
            {
                if (Vector3.Dot(other.GetContact(0).normal, transform.up) > 0.8f)
                    Ground();
            }
        }
    }



    // Getter And Setter

    public override ActorParts GetActorParts()
    {
        return this.humanoidParts;
    }

    // 状态设置函数


    // 回到地面状态时 调用
    public override void Ground()
    {
        animationHash.air = false;
        rigid.velocity = Vector3.zero;
        StartCoroutine(TurnToFont(2.0f));
        CancelInvoke("resetJump");
        Invoke("resetJump", 0.2f);
    }

    public override void Attack(bool isAttack)
    {
        if (isAttack)
        {
            if (curHandAction == Global.HandAction.FREE)
            {
                animationHash.SetLayerWeight(1, 1.0f);
                animationHash.attack = true;
            }
            actorBehaviours.SetAllHandAction(Global.HandAction.SLASH);
        }
        else
        {
            HandNone(curHandAction == Global.HandAction.SLASH);
        }
    }
    public override void Defence(bool isDefence)
    {
        if (isDefence)
        {
            if (curHandAction == Global.HandAction.FREE)
            {
                animationHash.SetLayerWeight(1, 1.0f);
                animationHash.defence = true;
            }
            actorBehaviours.SetAllHandAction(Global.HandAction.DEFENCE);
        }
        else
        {
            HandNone(curHandAction == Global.HandAction.DEFENCE);
        }
    }
    private void HandNone(bool changeState = true)
    {
        animationHash.SetLayerWeight(1, 0.0f);
        animationHash.defence = false;
        animationHash.attack = false;
        if (changeState)
            actorBehaviours.SetAllHandAction(Global.HandAction.FREE);
    }




    public override void AdRange(float deltaRange)
    {
        atdeRange = Mathf.Clamp(atdeRange + deltaRange, 1.4f, 1.7f);
        Transform temp = attackPlane.parent;
        attackPlane.parent = null;
        attackPlane.localScale = Vector3.one * atdeRange;
        attackPlane.parent = temp;
    }

    public override void Dodge()
    {
        animationHash.dodge = true;
    }

    public override void Jump(float jumpVal)
    {
        if (curBodyAction == Global.BodyAction.GROUND && canJump)
        {
            // 设置 物理属性
            rigid.velocity = animationHash.speedX * transform.right * 2.5f + animationHash.speedY * transform.forward * 2.5f + Vector3.up * 6.0f * Mathf.Lerp(0.5f, 1.0f, jumpVal);
            animationHash.JumpLeg = -animationHash.speedX;
            animationHash.Jump = rigid.velocity.y;
            canJump = false;
        }
    }


    // 失衡
    public override void Fall()
    {
        // 如果 不是 被回调 而是 controller 类内调用
        if (curBodyAction != Global.BodyAction.FALLING)
        {
            am.state.State2Fall();
            //curBodyAction = Global.BodyAction.FALLING;
        }
        animationHash.air = false;
        animationHash.fallStun = false;
        humanoidParts.propHandlers[0].currentProp = null;
        curBodyAction = Global.BodyAction.FALLING;
    }

    public override void GetUp()
    {
        //主要是在 设置 玩家  起身时候 的 pin 和 muscleweight   
        // 使得 起身的 时候 保证 动画的 正确进行

    }

    public override void OnRegainBalance()
    {
        Ground();
    }

    //TODO 不同手部 不同ph
    public override void PickDrop(bool picking)
    {
        if (curBodyAction == Global.BodyAction.SKILL) return; // 在skill 中途不允许进行其它操作
        //actorBehaviours.SetAllHandAction(Global.HandAction.FETCH);
        PropHandler ph = humanoidParts.propHandlers[0];
        if (ph.holdingProp && picking)
        {
            ph.holdingProp = null;
            am.unLock();
            //curHandAction = Global.HandAction.FREE;
            actorBehaviours.SetAllHandAction(Global.HandAction.FREE);
            return;
        }
        // 武器的丢弃（投掷）
        if (picking && humanoidParts.propHandlers[0].currentProp)
        {
            humanoidParts.propHandlers[0].currentProp = null;
            return;
        }

        if (!picking && !ph.holdingProp)
        {
            fullBodyBipedIK.solver.rightArmChain.reach = 0.0f;
            fullBodyBipedIK.solver.rightArmChain.pull = 0.0f;
            //curHandAction = Global.HandAction.FREE;
            actorBehaviours.SetAllHandAction(Global.HandAction.FREE);
            return;
        }
        //curHandAction = Global.HandAction.FETCH;
        actorBehaviours.SetAllHandAction(Global.HandAction.FETCH);
        //playerHandBehaviours[0].handAction = Global.HandAction.FETCH;
    }


    // 技能触发函数
    public override void Skill()
    {
        Vector3 dir = humanoidParts.aimPart.position - humanoidParts.rigidRoot.position;
        Skill skill = am.skillController.GetSkillByAction(humanoidParts.rigidRoot.InverseTransformVector(dir));
        if (skill == null) return; // TODO 诺 此位置不存在剑技 则需要 进行 ui 的提示
        am.puppet.angularLimits = false;
        animator.CrossFade(skill.skillName, 0.02f, 0, 0.0f);
        SkillOffsetVec.Set(0, 0);
        curBias = 0.2f;
    }





    // 外部回调
    // 作为回调 在IK 之前获取  动画的位置信息
    // public override void PreIK()
    // {
    //     Transform rhand = humanoidParts.GetBodyPart(HumanBodyBones.RightHand).target;
    //     //RHandSkillPos = rhand.position;
    //     playerBodyBehaviours.RHandSkillPos = rhand.position;

    // }
    // 武器插入物体中的时候 直接设置为 Fetch
    public void OnStabIn()
    {
        actorBehaviours.SetAllHandAction(Global.HandAction.FETCH);
        //playerHandBehaviours[0].handAction = 
        Rigidbody IKRigid = humanoidParts.aimPart.GetComponent<Rigidbody>();
        IKRigid.velocity = Vector3.zero; IKRigid.angularVelocity = Vector3.zero;
        IKRigid.position = humanoidParts.propHandlers[0].holdingProp.transform.position;
    }




    // 体动作辅助函数

    /// <summary>
    /// 让 玩家 起身之后 的 forwar slerp 到 相机的正方向。
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator TurnToFont(float time)
    {
        float delta = 0;
        while (delta < 1)
        {
            humanoidParts.animationRoot.forward = Vector3.Slerp(humanoidParts.animationRoot.forward, am.cameraHandler.handler.parent.forward, delta);
            humanoidParts.rigidRoot.forward = humanoidParts.animationRoot.forward;
            delta += Time.deltaTime / time;
            yield return new WaitForEndOfFrame();
        }
        humanoidParts.animationRoot.forward = Vector3.Slerp(humanoidParts.animationRoot.forward, am.cameraHandler.handler.parent.forward, 1);
    }





    private void resetJump()
    {
        canJump = true;
    }

    // 忽略 武器和 人物部分的碰撞
    private void IgnoreHWCollision(Collider weaponCollider, bool ignore)
    {
        Physics.IgnoreCollision(am.targetTest.GetComponent<Collider>(), weaponCollider, ignore);
        Physics.IgnoreCollision(humanoidParts.GetBodyPart(HumanBodyBones.RightHand).colliders[0], weaponCollider, ignore);
        Physics.IgnoreCollision(humanoidParts.GetBodyPart(HumanBodyBones.RightLowerArm).colliders[0], weaponCollider, ignore);
    }




}
