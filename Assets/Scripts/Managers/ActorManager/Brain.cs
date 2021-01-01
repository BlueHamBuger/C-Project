using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Brain : MonoBehaviour
{
    public ActorManager am;
    public EnemyController ac;
    NavMeshAgent nma;
    public Transform playerTransform;
    float armLenght = 0;
    Vector2 fomerVel = Vector2.zero;
    public EnemyFS fs = EnemyFS.APPROCHING;
    bool canAttack = true;
    bool canSkill = false;
    // Couroutines 
    Coroutine resetAttackC = null;
    Coroutine resetSkillC = null;
    float minDist;
    float maxDist;
    float distance; // 和玩家之间的距离
    bool sleep = true;




    private void Start()
    {
        nma = GetComponent<NavMeshAgent>();
        //playerTransform = GameManager.gameMng.controllingMngs[0].actorController.transform;
        nma.updatePosition = false;
        nma.updateRotation = false;
        armLenght = ac.GetActorParts().GetMuscleGroup(HumanoidParts.MucleGroupIndex.RHAND).muscleLength;
        nma.stoppingDistance = armLenght * 2.5f;
        minDist = 2 * armLenght;
        maxDist = 4 * armLenght;
        MessageManager.AddListener<Global.HandAction>("HandStateChange", OnHandStateChange, transform);
        fs = EnemyFS.READY;
        resetSkillC = StartCoroutine(resetSkill());
    }
    private void FixedUpdate()
    {
        if(sleep){
            distance = Vector3.Distance(playerTransform.position, transform.position);
            if(distance<= 4 * armLenght){
                sleep = false;
            }
            return;
        }

        if (am.state.actionState.CanInput)
            FSM();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="maxSpeed"></param>
    /// <returns> 返回是否需要进行转身 </returns>
    private bool FacePlayer(float maxSpeed = 0.07f)
    { // 持续朝向 玩家
        Vector3 dir = transform.forward;
        Vector3 tarDir = (playerTransform.position - transform.position).normalized;
        if (ac.rigid.velocity.magnitude <= 0.2f)
        {
            float angle = Vector3.SignedAngle(transform.forward, tarDir, transform.up);
            if ((fs != EnemyFS.ATTACK || fs != EnemyFS.SKILL) && Mathf.Abs(angle) > 60)
            {
                print(angle);
                ac.Turn(angle / 90f);
                return true;
            }
            else
            {
                ac.Turn(0);
                transform.forward = Vector3.MoveTowards(dir, tarDir, maxSpeed);
            }

        }
        else
        {
            ac.Turn(0);
            transform.forward = Vector3.MoveTowards(dir, tarDir, maxSpeed);
        }
        return false;
    }
    /// <summary>
    /// 由于判断 和玩家之间的 距离
    /// </summary>
    /// <returns>改變是否发生了改变</returns>
    private bool JudgeDist()
    {
        distance = Vector3.Distance(playerTransform.position, transform.position);
        EnemyFS former = fs;
        if (distance >= maxDist)
        {
            fs = EnemyFS.APPROCHING;
        }
        else if (distance <= minDist)
        {
            fs = EnemyFS.KEEPAWAY;
        }
        else
        {
            fs = EnemyFS.READY;
        }
        return (former != fs);
    }
    private void FSM()
    {
        switch (fs)
        {
            case EnemyFS.APPROCHING:
                Approch();
                break;
            case EnemyFS.ATTACK:
                Attack();
                break;
            case EnemyFS.SKILL:
                Skill();
                break;
            case EnemyFS.KEEPAWAY:
                KeepAway();
                break;
            case EnemyFS.READY:
                Ready();
                break;
        }
    }
    private void Ready()
    {
        if (FacePlayer() || JudgeDist())
        {
            nma.velocity = Vector3.zero;
            return;
        }
        else
        {
            fomerVel = Vector3.MoveTowards(fomerVel, Vector3.zero, 0.05f);
            ac.Move(fomerVel);
            if (canSkill)
            {
                fs = EnemyFS.SKILL;
            }
            else if (canAttack)
                fs = EnemyFS.ATTACK;
        }

    }
    private void Approch()
    {
        if (JudgeDist() || FacePlayer()) return;
        nma.destination = playerTransform.position;
        Vector3 transformedDir = transform.InverseTransformVector(nma.velocity);
        Vector3 worldDeltaPosition = nma.nextPosition - transform.position;
        fomerVel = new Vector2(transformedDir.x, transformedDir.z);
        if (worldDeltaPosition.magnitude > nma.radius)
            nma.nextPosition = transform.position + 0.9f * worldDeltaPosition;
        ac.Move(fomerVel);
    }
    private void Attack()
    {
        if (am.state.HandAction == Global.HandAction.FREE && canAttack) // 开始
        {
            FacePlayer();
            ac.AttackT(JudgeAttackType());
            canAttack = false;
        }
        else if (am.state.HandAction == Global.HandAction.SLASH)
        { // on
            FacePlayer(0.1f);
        }
        else
        { // 结束
            FacePlayer();
            fs = EnemyFS.READY;
        }
    }
    private int JudgeAttackType()
    {
        return Random.Range(1, 4);
    }
    private string JudgeSkillType()
    {
        if (distance >= maxDist - 0.5 * armLenght)
            return "DashAttack";
        else
            return "Combo3";
    }
    private void Skill()
    {
        if (am.state.BodyAction == Global.BodyAction.GROUND && canSkill) // 开始
        {
            FacePlayer();
            ac.skillName = JudgeSkillType();
            ac.Skill();
            canSkill = false;
            canAttack = false;
        }
        else if (am.state.BodyAction == Global.BodyAction.SKILL)
        { // on
            //FacePlayer(0.01f);
        }
        else
        { // 结束
            FacePlayer();
            fs = EnemyFS.READY;
        }
    }
    private void KeepAway()
    {
        if (JudgeDist() || FacePlayer()) return;
        nma.destination = playerTransform.position - (playerTransform.position - transform.position).normalized * armLenght * 5;
        Vector3 transformedDir = transform.InverseTransformVector(nma.velocity);
        Vector3 worldDeltaPosition = nma.nextPosition - transform.position;
        fomerVel = new Vector2(transformedDir.x, transformedDir.z);
        if (worldDeltaPosition.magnitude > nma.radius)
            nma.nextPosition = transform.position + 0.9f * worldDeltaPosition;
        ac.Move(fomerVel);

    }
    private void OnHandStateChange(Global.HandAction ha)
    {
        switch (ha)
        {
            case Global.HandAction.FREE:
                if (resetAttackC == null && canAttack == false)
                {
                    resetAttackC = StartCoroutine(resetAttack());
                }
                if (resetSkillC == null && canSkill == false)
                {
                    resetSkillC = StartCoroutine(resetSkill());
                }
                break;
            default:
                break;
        }
    }
    //CD 回调
    private IEnumerator resetAttack()
    {
        yield return new WaitForSeconds(0.8f);
        canAttack = true;
        resetAttackC = null;
    }
    private IEnumerator resetSkill()
    {
        yield return new WaitForSeconds(5f);
        canSkill = true;
        resetSkillC = null;
    }
}
public enum EnemyFS
{
    APPROCHING,
    ATTACK,
    SKILL,
    KEEPAWAY,
    READY,
}
