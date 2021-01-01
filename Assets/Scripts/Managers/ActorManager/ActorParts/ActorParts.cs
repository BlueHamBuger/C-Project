using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;


/// <summary>
/// ActorParts 应该被不同的 actorController 持有。
///     将会 对骨骼进行分组， 同组骨骼 将拥有 相同的 muscle 属性
///     其用于 识别 不同的 类型的 骨骼架构 所对应的  骨骼的位置，
///             以及应该暴露出来作为 属性的  骨骼
/// 
///     主要 应用于  动画的时候 对 一定的骨骼权重进行设置
///     此外 还用于 角色各个 肢体的 状态识别。
///             即 使用 MuscleGroup 来标识对应的 一组muscle
///     
/// </summary>`
[System.Serializable]
public abstract class ActorParts
{
    [HideInInspector]
    public PuppetMaster puppet;
    [HideInInspector]
    protected Muscle[] bodyParts;

    [HideInInspector]
    public Transform puppetRoot;
    [HideInInspector]
    public Transform animationRoot;

    [Tooltip("用来标识 角色物理位置的 root")]
    public Transform rigidRoot;

    //TODO 左右手皆可 瞄准
    [Tooltip("角色的瞄准的位置")]
    public Transform aimPart;

    protected MuscleGroup[] muscleGroups;
    public MuscleGroup[] MuscleGroups
    {
        get{
            return muscleGroups;
        }
    }

    public PropHandler[] propHandlers;
    


    public ActorParts(PuppetMaster puppet)
    {
        this.puppet = puppet;
        bodyParts = puppet.muscles;
        puppetRoot = puppet.transform;
        animationRoot = puppet.targetAnimator.transform;

        initBodyParts();
        // 自动设置 propHandler
        propHandlers = animationRoot.GetComponentsInChildren<PropHandler>();
        foreach(var ph in propHandlers){
            MuscleGroup  mg = GetMuscleGroup(puppet.GetMuscleIndex(ph.transform.parent));
            mg.propHandler = ph;
            ph.Init(this,mg);
        }
        initPropHandlers();
    }
    protected virtual void initPropHandlers(){}

    protected virtual void initBodyParts(){
    }

    //public abstract 

    //TODO get 各种 muscle 相关的属性 以供其它的 类进行 调用

    public abstract Muscle GetBodyPart(object obj);
    public abstract Skill[] GetLearnableSkill();

    /// <summary>
    /// 获取对应状态下 肌肉组的 属性值
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public abstract Global.MuscleGroupStateProps GetStateProp(StateController.ActionState action);

    public MuscleGroup GetMuscleGroup(string name){
        foreach (var muscleGroup in muscleGroups)
        {
            if (muscleGroup.groupName == name)
            {
                return muscleGroup;
            }
        }
        return null;
    }

    public MuscleGroup GetMuscleGroup(int muscleIndex){
        Muscle m = puppet.muscles[muscleIndex];
        foreach(var group in muscleGroups){
            foreach(var m1 in group.muscles)
                if(m == m1) return group;
        }
        return null;
    }


    // 通过 在 子类中 直接 定义 静态的 类
        // 来显示地使用 index 来获取到muscleGroup
    public MuscleGroup GetMuscleGroup(short index){
        return muscleGroups[index];
    }



    // 为 Muscle 直接进行分组
    // 使用 Mask
    /// <summary>
    ///  Muscle Group 应该要求 必须组内肌肉 父子关系为:
    ///     [0] -> [1] -> [2] ...
    /// </summary>
    public class MuscleGroup
    {
        public string groupName;
        private short groupIndex;
        public short GroupIndex{
            get{return groupIndex;}
        }
        public Muscle[] muscles;

        /// <summary>
        /// 决定 此muscleGroup 是否有 propHandler
        /// </summary>
        public PropHandler propHandler = null;
        public Global.MuscleActionState actionState = Global.MuscleActionState.NONE;
        public float muscleLength;



        // 进攻 使用的 部位的  transform
        // 为空 则代表 无
        //可以 是 肢体 或者 武器
        [HideInInspector]
        public Transform attackTran = null;
        public MuscleGroup(string name,short index, params Muscle[] muscles)
        {
            this.muscles = muscles;
            this.groupName = name;
            groupIndex = index;
            muscleLength = Vector3.Distance(muscles[0].target.position,muscles[muscles.Length-1].target.position);
        }
    }
}
