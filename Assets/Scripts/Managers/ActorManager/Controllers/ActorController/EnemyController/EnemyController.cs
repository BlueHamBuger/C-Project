using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class EnemyController : ActorController
{
    AnimationHashBase animationHash;
    public BearMonsterParts humanoidParts;
    protected override AnimationHashBase animationHashBase
    {
        get { return animationHash; }
    }
    //TODO JUST FOR TEST
    public string skillName;

    public override void Init(ActorManager am)
    {
        base.Init(am);
        // 变量初始化
        BearMonsterParts temp = new BearMonsterParts(am.puppet);
        temp.aimPart = humanoidParts.aimPart;
        temp.rigidRoot = humanoidParts.rigidRoot;
        humanoidParts = temp;
        animationHash = new AnimationHashBase(animator);
        actorBehaviours = new EnemyBehaviours(this);

        // 取消 武器和 地面的碰撞
        //Physics.IgnoreCollision(humanoidParts.propHandlers[0].currentProp.edgeCollider,)
        //Physics.IgnoreLayerCollision()
    }

    public override ActorParts GetActorParts()
    {
        return humanoidParts;
    }

    public void AttackT(int type)
    {
        animationHashBase.handType = type;
        Attack(true);
    }
    public override void Attack(bool isAttack)
    {
        animator.SetTrigger("attack");
    }

    public override void Fall()
    {
        // 如果 不是 被回调 而是 controller 类内调用
        if (curBodyAction != Global.BodyAction.FALLING)
        {
            am.state.State2Fall();
            //curBodyAction = Global.BodyAction.FALLING;
            ///humanoidParts.propHandlers[0].currentProp = null;
        }
        animationHash.air = false;
        animationHash.fallStun = false;
        curBodyAction = Global.BodyAction.FALLING;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
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
    public override void Ground()
    {
        animationHash.air = false;
        rigid.velocity = Vector3.zero;
        // StartCoroutine(TurnToFont(2.0f));
        // CancelInvoke("resetJump");
        // Invoke("resetJump", 0.2f);
    }
    public override void Skill()
    {
        //Vector3 dir = humanoidParts.aimPart.position - humanoidParts.rigidRoot.position;
        //Skill skill = am.skillController.GetSkillByAction(humanoidParts.rigidRoot.InverseTransformVector(dir));
        //if (skill == null) return; // TODO 诺 此位置不存在剑技 则需要 进行 ui 的提示
        am.puppet.angularLimits = false;
        animator.CrossFade(skillName, 0.05f, 0, 0.0f);
    }
    private string testSkill()
    {
        int r = Random.Range(0, 2);
        switch (r)
        {
            case 0:
                return "Combo3";
            case 1:
                return "DashAttack";
            case 2:
                return "Casting";
        }
        return "Combo3";
    }
    public override void Jump(float val) { }

    // puppet 回调函数

    public override void GetUp() { }
    public override void OnRegainBalance()
    {
    }

}
