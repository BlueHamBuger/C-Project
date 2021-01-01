using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract partial class ActorController : MonoBehaviour
{
    public ActorManager am;
    public Rigidbody rigid;
    protected Animator animator;

    // 角色视角的移动
    protected Vector2 viewXY;
    /// <summary>
    /// 主要用于 剑技的释放的 IK 要先 得到 原位置
    /// </summary>
    // 跑步加速系数
    protected float speedMul = 1.0f;

    protected Global.BodyAction curBodyAction
    {
        get
        {
            return am.state.BodyAction;
        }
        set
        {
            am.state.BodyAction = value;
        }
    }
    protected Global.BodyAction formerBodyAction
    {
        get
        {
            return am.state.actionState.formerBodyAction;
        }
    }

    protected Global.HandAction curHandAction
    {
        get
        {
            return am.state.actionState.handAction;
        }
        set
        {
            am.state.HandAction = value;
        }
    }
    protected Global.HandAction formerHandAction
    {
        get
        {
            return am.state.actionState.formerHandAction;
        }
    }
    protected IActorBehaviour actorBehaviours;
    protected abstract AnimationHashBase animationHashBase
    {
        get;
    }

    public virtual void FixedUpdate()
    {
        actorBehaviours.Update();
    }
    private void OnAnimatorMove()
    {
        rigid.MovePosition(rigid.position + animator.deltaPosition);
        rigid.MoveRotation(animator.deltaRotation * rigid.rotation);
    }



    public virtual IEnumerator SetBodyAction(Global.BodyAction ba, float time)
    {
        yield return new WaitForSeconds(time);
        SetBodyAction(ba);
    }

    public virtual void SetBodyActionInTime(Global.BodyAction ba, float time)
    {
        StartCoroutine(SetBodyAction(ba, time));
    }
    public virtual void SetHandAction(Global.HandAction ha)
    {
        //curHandAction = ha;
        actorBehaviours.SetAllHandAction(ha);
    }
    public virtual void SetBodyAction(Global.BodyAction ba)
    {
        actorBehaviours.SetBodyAction(ba);
        //curBodyAction = ba;
    }
    protected bool CanInput()
    {
        return am.state.actionState.CanInput;
    }



    public virtual void Init(ActorManager am)
    {
        this.am = am;
        rigid = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }
    public virtual void LateInit() { }
    public abstract ActorParts GetActorParts();



    // 基本actions

    ///public abstract void HandAction(Global.HandAction handAction, Vector2 viewXY);
    public virtual void Attack(bool isAttack) { }
    public virtual void Defence(bool isDefence) { }

    public virtual void AdRange(float deltaRange) { }
    public virtual void Move(Vector2 moveXY)
    {
        animationHashBase.speedX = moveXY.x * speedMul;
        animationHashBase.speedY = moveXY.y * speedMul;
    }
    public virtual void Run(bool run)
    {
        speedMul = run ? Mathf.Lerp(speedMul, 2, Time.deltaTime) : (speedMul > 1 ? Mathf.Lerp(speedMul, 1, Time.deltaTime) : 1);
    }
    public virtual void Dodge()
    {
        animationHashBase.dodge = true;
    }


    public abstract void Jump(float val);
    public abstract void Fall();

    public virtual void Air()
    {
        animationHashBase.air = true;
    }

    public virtual void Turn(float turn){
        animationHashBase.turn = turn;
    }
    public virtual void Ground() { }
    public virtual void Skill() { }
    public abstract void GetUp();
    public abstract void OnRegainBalance();

    public virtual void PickDrop(bool v) { }
    public virtual void View(Vector2 viewXY)
    {
        this.viewXY = viewXY;
    }

    // 对外委托
    public void PreIK()
    {
        actorBehaviours.OnPreIK();
    }


}
