using System.Collections;
using System.Collections.Generic;
using RootMotion.Dynamics;
using UnityEngine;


[System.Serializable]
public class HumanoidParts : ActorParts
{

    protected Dictionary<HumanBodyBones, Muscle> bodyDic;
    private static HumanBodyBones[] neededBones = new HumanBodyBones[]{
        //head
        HumanBodyBones.Head,

        //body
        HumanBodyBones.Chest,
        HumanBodyBones.Hips,    

        // rightHand
        HumanBodyBones.RightUpperArm,
        HumanBodyBones.RightLowerArm,
        HumanBodyBones.RightHand,

        // leftHand
        HumanBodyBones.LeftUpperArm,
        HumanBodyBones.LeftLowerArm,
        HumanBodyBones.LeftHand,

        //RightFoot
        HumanBodyBones.RightUpperLeg,
        HumanBodyBones.RightLowerLeg,
        HumanBodyBones.RightFoot,

        //LeftFoot
        HumanBodyBones.LeftUpperLeg,
        HumanBodyBones.LeftLowerLeg,
        HumanBodyBones.LeftFoot,
    };
    protected virtual HumanBodyBones[] NeededBones
    {
        get { return neededBones; }
    }
    protected virtual Dictionary<Global.BodyAction, Global.MuscleGroupStateProps> NormalBodyStateProps
    {
        get { return normalBodyStateProps; }
    }
    protected virtual Dictionary<Global.HandAction, Global.MuscleGroupStateProps> NormalHandStateProps
    {
        get { return normalHandStateProps; }
    }
    protected virtual Skill[] LearnableSkills
    {
        get { return learnableSkills; }
    }


    public HumanoidParts(PuppetMaster puppet) : base(puppet)
    { }

    protected override void initBodyParts()
    {
        // if(aimPart.gameObject.GetComponent<targetTest>() == null)
        //     aimPart.gameObject.AddComponent<targetTest>();
        bodyDic = new Dictionary<HumanBodyBones, Muscle>();
        for (int i = 0; i < neededBones.Length; i++)
        {
            int j = puppet.GetMuscleIndex(neededBones[i]);
            if (j != -1)
            {
                bodyDic[neededBones[i]] = puppet.muscles[j];
            }
        }
        initMuscleGroup();
    }
    protected override void initPropHandlers()
    {
        // foreach(var ph in propHandlers){
        muscleGroups[HumanoidParts.MucleGroupIndex.Prop] = new MuscleGroup("RightProp", HumanoidParts.MucleGroupIndex.Prop, propHandlers[0].muscles);
        //}
    }


    private void initMuscleGroup()
    {
        muscleGroups = new MuscleGroup[7]{
            new MuscleGroup("Head",HumanoidParts.MucleGroupIndex.HEAD,
                bodyDic[HumanBodyBones.Head]
            ),
            new MuscleGroup("Body",HumanoidParts.MucleGroupIndex.BODY,
                bodyDic[HumanBodyBones.Chest],
                bodyDic[HumanBodyBones.Hips]
            ),
            new MuscleGroup("RightHand",HumanoidParts.MucleGroupIndex.RHAND,
                bodyDic[HumanBodyBones.RightUpperArm],
                bodyDic[HumanBodyBones.RightLowerArm],
                bodyDic[HumanBodyBones.RightHand]
            ),
            new MuscleGroup("LeftHand",HumanoidParts.MucleGroupIndex.LHAND,
                bodyDic[HumanBodyBones.LeftUpperArm],
                bodyDic[HumanBodyBones.LeftLowerArm],
                bodyDic[HumanBodyBones.LeftHand]
            ),
            new MuscleGroup("RightFoot",HumanoidParts.MucleGroupIndex.RFOOT,
                bodyDic[HumanBodyBones.RightUpperLeg],
                bodyDic[HumanBodyBones.RightLowerLeg],
                bodyDic[HumanBodyBones.RightFoot]
            ),
            new MuscleGroup("LeftFoot",HumanoidParts.MucleGroupIndex.LFOOT,
                bodyDic[HumanBodyBones.LeftUpperLeg],
                bodyDic[HumanBodyBones.LeftLowerLeg],
                bodyDic[HumanBodyBones.LeftFoot]
            ),null
        };

    }


    /// <summary>
    /// object 应该是 HumanBodyBones 类型
    /// </summary>
    /// <param name="hbb"></param>
    /// <returns></returns>
    public override Muscle GetBodyPart(object hbb)
    {
        HumanBodyBones humanBodyBones = (HumanBodyBones)hbb;
        return bodyDic[humanBodyBones];
    }

    public override Global.MuscleGroupStateProps GetStateProp(StateController.ActionState action)
    {
        Global.MuscleGroupStateProps pb1 = NormalBodyStateProps.ContainsKey(action.bodyAction) ? NormalBodyStateProps[action.bodyAction] : null;
        Global.MuscleGroupStateProps pb2 = NormalHandStateProps.ContainsKey(action.handAction) ? NormalHandStateProps[action.handAction] : null;

        return pb1 + pb2;
    }
    public override Skill[] GetLearnableSkill()
    {
        return LearnableSkills;
    }

    public static class MucleGroupIndex
    {
        public const short HEAD = 0;
        public const short BODY = 1;
        public const short RHAND = 2;
        public const short LHAND = 3;
        public const short RFOOT = 4;
        public const short LFOOT = 5;
        public const short Prop = 6;
    }


    private static readonly Dictionary<Global.BodyAction, Global.MuscleGroupStateProps> normalBodyStateProps = new Dictionary<Global.BodyAction, Global.MuscleGroupStateProps>
    {
        [Global.BodyAction.GROUND] = new Global.MuscleGroupStateProps(
            new Dictionary<short, StateController.PropsBase>
            {
                [HumanoidParts.MucleGroupIndex.HEAD] = new StateController.PropsBase(0.8f, 0.3f, 0.8f, 0.8f, 0.1f),
                [HumanoidParts.MucleGroupIndex.BODY] = new StateController.PropsBase(0.8f, 0.3f, 0.8f, 0.8f, 2f),
                [HumanoidParts.MucleGroupIndex.RHAND] = new StateController.PropsBase(0.8f, 0.3f, 0.4f, 0.4f, 2f),
                [HumanoidParts.MucleGroupIndex.LHAND] = new StateController.PropsBase(0.8f, 0.3f, 0.4f, 0.4f, 2f),
                [HumanoidParts.MucleGroupIndex.LFOOT] = new StateController.PropsBase(0.8f, 0.3f, 1.0f, 1.0f, 2f),
                [HumanoidParts.MucleGroupIndex.RFOOT] = new StateController.PropsBase(0.8f, 0.3f, 1.0f, 1.0f, 2f),
                [HumanoidParts.MucleGroupIndex.Prop] = new StateController.PropsBase(1.2f, 1.0f, 1f, 1f, 10f),
            }
        ),
        [Global.BodyAction.GETUP] = new Global.MuscleGroupStateProps(
            new Dictionary<short, StateController.PropsBase>
            {
                [HumanoidParts.MucleGroupIndex.HEAD] = new StateController.PropsBase(1.0f, 0.8f, 1.0f, 1.0f, 0.1f),
                [HumanoidParts.MucleGroupIndex.BODY] = new StateController.PropsBase(1.0f, 0.8f, 1.0f, 1.0f, 1f),
                [HumanoidParts.MucleGroupIndex.RHAND] = new StateController.PropsBase(1.0f, 0.8f, 1.0f, 1.0f, 1f),
                [HumanoidParts.MucleGroupIndex.LHAND] = new StateController.PropsBase(1.0f, 0.8f, 1.0f, 1.0f, 1f),
                [HumanoidParts.MucleGroupIndex.LFOOT] = new StateController.PropsBase(1.0f, 0.8f, 1.0f, 1.0f, 1f),
                [HumanoidParts.MucleGroupIndex.RFOOT] = new StateController.PropsBase(1.0f, 0.8f, 1.0f, 1.0f, 1f),
                [HumanoidParts.MucleGroupIndex.Prop] = new StateController.PropsBase(1.2f, 0.8f, 1.0f, 1.0f, 10f),
            }
        ),
        [Global.BodyAction.FALLING] = new Global.MuscleGroupStateProps(
            new Dictionary<short, StateController.PropsBase>
            {
                [HumanoidParts.MucleGroupIndex.HEAD] = new StateController.PropsBase(0.5f, 0.7f, 0.0f, 0.0f, 0.1f),
                [HumanoidParts.MucleGroupIndex.BODY] = new StateController.PropsBase(0.5f, 0.7f, 0.0f, 0.0f, 1f),
                [HumanoidParts.MucleGroupIndex.RHAND] = new StateController.PropsBase(0.5f, 0.7f, 0.0f, 0.0f, 1f),
                [HumanoidParts.MucleGroupIndex.LHAND] = new StateController.PropsBase(0.5f, 0.7f, 0.0f, 0.0f, 1f),
                [HumanoidParts.MucleGroupIndex.LFOOT] = new StateController.PropsBase(0.5f, 0.7f, 0.0f, 0.0f, 1f),
                [HumanoidParts.MucleGroupIndex.RFOOT] = new StateController.PropsBase(0.5f, 0.7f, 0.0f, 0.0f, 1f),
                [HumanoidParts.MucleGroupIndex.Prop] = new StateController.PropsBase(0.8f, 0.7f, 1.0f, 1.0f, 10),
            }
        ),
        [Global.BodyAction.AIR] = new Global.MuscleGroupStateProps(
            new Dictionary<short, StateController.PropsBase>
            {
                [HumanoidParts.MucleGroupIndex.HEAD] = new StateController.PropsBase(1f, 0.5f, 0.8f, 0.8f, 1f),
                [HumanoidParts.MucleGroupIndex.BODY] = new StateController.PropsBase(1f, 0.5f, 0.8f, 0.8f, 3f),
                [HumanoidParts.MucleGroupIndex.RHAND] = new StateController.PropsBase(1f, 0.5f, 0.4f, 0.4f, 5f),
                [HumanoidParts.MucleGroupIndex.LHAND] = new StateController.PropsBase(1f, 0.5f, 0.4f, 0.4f, 5f),
                [HumanoidParts.MucleGroupIndex.LFOOT] = new StateController.PropsBase(1f, 0.5f, 0.8f, 1.0f, 2f),
                [HumanoidParts.MucleGroupIndex.RFOOT] = new StateController.PropsBase(1f, 0.5f, 0.8f, 1.0f, 2f),
                [HumanoidParts.MucleGroupIndex.Prop] = new StateController.PropsBase(1.2f, 0.8f, 1.0f, 1.0f, 10),
            }
        ),
        // SKILL 的额外数值 应该 从 SkillController 中进行获取 
        [Global.BodyAction.SKILL] = new Global.MuscleGroupStateProps(
            new Dictionary<short, StateController.PropsBase>
            {
                [HumanoidParts.MucleGroupIndex.HEAD] = new StateController.PropsBase(1.0f, 0.8f, 1.0f, 1.0f, 5f),
                [HumanoidParts.MucleGroupIndex.BODY] = new StateController.PropsBase(1.0f, 0.8f, 1.0f, 1.0f, 5f),
                [HumanoidParts.MucleGroupIndex.RHAND] = new StateController.PropsBase(1.0f, 0.8f, 1.0f, 1.0f, 10f),
                [HumanoidParts.MucleGroupIndex.LHAND] = new StateController.PropsBase(1.0f, 0.8f, 1.0f, 1.0f, 10f),
                [HumanoidParts.MucleGroupIndex.LFOOT] = new StateController.PropsBase(2.0f, 0.8f, 1.0f, 1.0f, 5f),
                [HumanoidParts.MucleGroupIndex.RFOOT] = new StateController.PropsBase(2.0f, 0.8f, 1.0f, 1.0f, 5f),
                [HumanoidParts.MucleGroupIndex.Prop] = new StateController.PropsBase(5f, 1.0f, 1.0f, 1.0f, 10),
            }
        ),
        [Global.BodyAction.FETCH] = new Global.MuscleGroupStateProps(
            new Dictionary<short, StateController.PropsBase>
            {
                [HumanoidParts.MucleGroupIndex.HEAD] = new StateController.PropsBase(0.8f, 0.2f, 1.0f, 1.0f, 10f),
                [HumanoidParts.MucleGroupIndex.BODY] = new StateController.PropsBase(0.8f, 0.3f, 1.0f, 1.0f, 10f),
                [HumanoidParts.MucleGroupIndex.RHAND] = new StateController.PropsBase(0.8f, 0.5f, 1.0f, 1.0f, 10f),
                [HumanoidParts.MucleGroupIndex.LHAND] = new StateController.PropsBase(0.8f, 0.5f, 1.0f, 1.0f, 10f),
                [HumanoidParts.MucleGroupIndex.LFOOT] = new StateController.PropsBase(0.8f, 0.5f, 1.0f, 1.0f, 10f),
                [HumanoidParts.MucleGroupIndex.RFOOT] = new StateController.PropsBase(0.8f, 0.5f, 1.0f, 1.0f, 10f),
                [HumanoidParts.MucleGroupIndex.Prop] = new StateController.PropsBase(1.0f, 0.8f, 1.0f, 1.0f, 10),
            }
        ),
    };
    private static readonly Dictionary<Global.HandAction, Global.MuscleGroupStateProps> normalHandStateProps = new Dictionary<Global.HandAction, Global.MuscleGroupStateProps>
    {
        [Global.HandAction.SLASH] = new Global.MuscleGroupStateProps(new Dictionary<short, StateController.PropsBase>
        {
            [HumanoidParts.MucleGroupIndex.HEAD] = new StateController.PropsBase(0.25f, 0.25f, 0.25f, 0.25f, 0.1f),
            [HumanoidParts.MucleGroupIndex.BODY] = new StateController.PropsBase(0.25f, 0.25f, 0.25f, 0.25f, 1),
            [HumanoidParts.MucleGroupIndex.RHAND] = new StateController.PropsBase(1f, 0.25f, 0.8f, 0.8f, 1),
            [HumanoidParts.MucleGroupIndex.LHAND] = new StateController.PropsBase(0.25f, 0.25f, 0.25f, 0.25f, 1),
            [HumanoidParts.MucleGroupIndex.LFOOT] = new StateController.PropsBase(0.25f, 0.25f, 0.25f, 0.25f, 1),
            [HumanoidParts.MucleGroupIndex.RFOOT] = new StateController.PropsBase(0.25f, 0.25f, 0.25f, 0.25f, 1),
            [HumanoidParts.MucleGroupIndex.Prop] = new StateController.PropsBase(0.25f, 0.1f, 1.0f, 1.0f, 1),
        }),
        [Global.HandAction.FREE] = null,
        [Global.HandAction.DEFENCE] = new Global.MuscleGroupStateProps(new Dictionary<short, StateController.PropsBase>
        {
            [HumanoidParts.MucleGroupIndex.RHAND] = new StateController.PropsBase(1f, 0.25f, 0.8f, 0.8f, 1),
        }),
        [Global.HandAction.FETCH] = new Global.MuscleGroupStateProps(new Dictionary<short, StateController.PropsBase>
        {
            [HumanoidParts.MucleGroupIndex.HEAD] = new StateController.PropsBase(0.8f, 0.5f, 1.0f, 1.0f, 1f),
            [HumanoidParts.MucleGroupIndex.BODY] = new StateController.PropsBase(0.8f, 0.5f, 1.0f, 1.0f, 1f),
            [HumanoidParts.MucleGroupIndex.RHAND] = new StateController.PropsBase(0.8f, 1f, 1.0f, 1.0f, 5f),
            [HumanoidParts.MucleGroupIndex.LHAND] = new StateController.PropsBase(0.8f, 1f, 1.0f, 1.0f, 5f),
            [HumanoidParts.MucleGroupIndex.LFOOT] = new StateController.PropsBase(0.8f, 0.5f, 1.0f, 1.0f, 1f),
            [HumanoidParts.MucleGroupIndex.RFOOT] = new StateController.PropsBase(0.8f, 0.5f, 1.0f, 1.0f, 1f),
            [HumanoidParts.MucleGroupIndex.Prop] = new StateController.PropsBase(1.0f, 0.8f, 1.0f, 1.0f, 0),
        }),
    };

    private static readonly Skill[] learnableSkills = {
        // 在这里 设置玩家可获得的技能
        new Skill("Combo3",2,null),
        new Skill("DashAttack",6,null),
        new Skill("SlashDouble",8,null),
    };

}
