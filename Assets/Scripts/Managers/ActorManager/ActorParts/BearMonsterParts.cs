using System.Collections;
using System.Collections.Generic;
using RootMotion.Dynamics;
using UnityEngine;

[System.Serializable]
public class BearMonsterParts : HumanoidParts
{
    public BearMonsterParts(PuppetMaster puppet) : base(puppet)
    { }

    protected override Dictionary<Global.BodyAction, Global.MuscleGroupStateProps> NormalBodyStateProps{
        get{return normalBodyStateProps;}
    }

    protected override Dictionary<Global.HandAction, Global.MuscleGroupStateProps> NormalHandStateProps{
        get{return normalHandStateProps;}
    }

    protected override Skill[] LearnableSkills {
        get{return learnableSkills;}
    }

    private static readonly Dictionary<Global.BodyAction, Global.MuscleGroupStateProps> normalBodyStateProps = new Dictionary<Global.BodyAction, Global.MuscleGroupStateProps>
    {
        [Global.BodyAction.GROUND] = new Global.MuscleGroupStateProps(
        new Dictionary<short, StateController.PropsBase>
        {
            [HumanoidParts.MucleGroupIndex.HEAD] = new StateController.PropsBase(0.8f, 0.3f, 0.9f, 0.9f,2),
            [HumanoidParts.MucleGroupIndex.BODY] = new StateController.PropsBase(0.8f, 0.3f, 0.9f, 0.9f,2),
            [HumanoidParts.MucleGroupIndex.RHAND] = new StateController.PropsBase(0.8f, 0.8f, 0.8f, 0.8f,8),
            [HumanoidParts.MucleGroupIndex.LHAND] = new StateController.PropsBase(0.8f, 0.8f, 0.8f, 0.8f,8),
            [HumanoidParts.MucleGroupIndex.LFOOT] = new StateController.PropsBase(0.8f, 0.7f, 1.0f, 1.0f,2),
            [HumanoidParts.MucleGroupIndex.RFOOT] = new StateController.PropsBase(0.8f, 0.7f, 1.0f, 1.0f,2),
            [HumanoidParts.MucleGroupIndex.Prop] = new StateController.PropsBase(2f, 1.0f, 1f, 1f,10),
        }
    ),
        [Global.BodyAction.GETUP] = new Global.MuscleGroupStateProps(
        new Dictionary<short, StateController.PropsBase>
        {
            [HumanoidParts.MucleGroupIndex.HEAD] = new StateController.PropsBase(1.0f, 1.0f, 1.0f, 1.0f,5),
            [HumanoidParts.MucleGroupIndex.BODY] = new StateController.PropsBase(1.0f, 1.0f, 1.0f, 1.0f,5),
            [HumanoidParts.MucleGroupIndex.RHAND] = new StateController.PropsBase(1.0f, 1.0f, 1.0f, 1.0f,5),
            [HumanoidParts.MucleGroupIndex.LHAND] = new StateController.PropsBase(1.0f, 1.0f, 1.0f, 1.0f,5),
            [HumanoidParts.MucleGroupIndex.LFOOT] = new StateController.PropsBase(1.0f, 1.0f, 1.0f, 1.0f,5),
            [HumanoidParts.MucleGroupIndex.RFOOT] = new StateController.PropsBase(1.0f, 1.0f, 1.0f, 1.0f,5),
            [HumanoidParts.MucleGroupIndex.Prop] = new StateController.PropsBase(2f, 1.0f, 1f, 1f,10),
        }
    ),
        [Global.BodyAction.FALLING] = new Global.MuscleGroupStateProps(
        new Dictionary<short, StateController.PropsBase>
        {
            [HumanoidParts.MucleGroupIndex.HEAD] = new StateController.PropsBase(0.5f, 0.5f, 0.0f, 0.0f,5),
            [HumanoidParts.MucleGroupIndex.BODY] = new StateController.PropsBase(0.5f, 0.5f, 0.0f, 0.0f,5),
            [HumanoidParts.MucleGroupIndex.RHAND] = new StateController.PropsBase(0.5f, 0.5f, 0.0f, 0.0f,5),
            [HumanoidParts.MucleGroupIndex.LHAND] = new StateController.PropsBase(0.5f, 0.5f, 0.0f, 0.0f,5),
            [HumanoidParts.MucleGroupIndex.LFOOT] = new StateController.PropsBase(0.5f, 0.5f, 0.0f, 0.0f,5),
            [HumanoidParts.MucleGroupIndex.RFOOT] = new StateController.PropsBase(0.5f, 0.5f, 0.0f, 0.0f,5),
            [HumanoidParts.MucleGroupIndex.Prop] = new StateController.PropsBase(2f, 1.0f, 1f, 1f,10),
        }
    ),
        [Global.BodyAction.AIR] = new Global.MuscleGroupStateProps(
        new Dictionary<short, StateController.PropsBase>
        {
            [HumanoidParts.MucleGroupIndex.HEAD] = new StateController.PropsBase(1f, 0.8f, 0.8f, 0.8f,5),
            [HumanoidParts.MucleGroupIndex.BODY] = new StateController.PropsBase(1f, 0.8f, 0.8f, 0.8f,5),
            [HumanoidParts.MucleGroupIndex.RHAND] = new StateController.PropsBase(1f, 0.8f, 0.8f, 0.8f,10),
            [HumanoidParts.MucleGroupIndex.LHAND] = new StateController.PropsBase(1f, 0.8f, 0.8f, 0.8f,10),
            [HumanoidParts.MucleGroupIndex.LFOOT] = new StateController.PropsBase(1f, 0.8f, 0.8f, 1.0f,5),
            [HumanoidParts.MucleGroupIndex.RFOOT] = new StateController.PropsBase(1f, 0.8f, 0.8f, 1.0f,5),
            [HumanoidParts.MucleGroupIndex.Prop] = new StateController.PropsBase(2f, 1.0f, 1f, 1f,10),
        }
    ),
        // SKILL 的额外数值 应该 从 SkillController 中进行获取 
        [Global.BodyAction.SKILL] = new Global.MuscleGroupStateProps(
        new Dictionary<short, StateController.PropsBase>
        {
            [HumanoidParts.MucleGroupIndex.HEAD] = new StateController.PropsBase(1.0f, 0.5f, 1.0f, 1.0f,2),
            [HumanoidParts.MucleGroupIndex.BODY] = new StateController.PropsBase(1.0f, 0.5f, 1.0f, 1.0f,2),
            [HumanoidParts.MucleGroupIndex.RHAND] = new StateController.PropsBase(1.0f, 1.0f, 1.0f, 1.0f,10),
            [HumanoidParts.MucleGroupIndex.LHAND] = new StateController.PropsBase(1.0f, 1.0f, 1.0f, 1.0f,10),
            [HumanoidParts.MucleGroupIndex.LFOOT] = new StateController.PropsBase(1.0f, 0.6f, 1.0f, 1.0f,3),
            [HumanoidParts.MucleGroupIndex.RFOOT] = new StateController.PropsBase(1.0f, 0.6f, 1.0f, 1.0f,3),
            [HumanoidParts.MucleGroupIndex.Prop] = new StateController.PropsBase(4f, 1.0f, 1f, 1f,20),
        }
    ),
        [Global.BodyAction.FETCH] = new Global.MuscleGroupStateProps(
        new Dictionary<short, StateController.PropsBase>
        {
            [HumanoidParts.MucleGroupIndex.HEAD] = new StateController.PropsBase(0.8f, 0.8f, 1.0f, 1.0f,5),
            [HumanoidParts.MucleGroupIndex.BODY] = new StateController.PropsBase(0.8f, 0.8f, 1.0f, 1.0f,5),
            [HumanoidParts.MucleGroupIndex.RHAND] = new StateController.PropsBase(0.8f, 0.8f, 1.0f, 1.0f,5),
            [HumanoidParts.MucleGroupIndex.LHAND] = new StateController.PropsBase(0.8f, 0.8f, 1.0f, 1.0f,5),
            [HumanoidParts.MucleGroupIndex.LFOOT] = new StateController.PropsBase(0.8f, 0.8f, 1.0f, 1.0f,5),
            [HumanoidParts.MucleGroupIndex.RFOOT] = new StateController.PropsBase(0.8f, 0.8f, 1.0f, 1.0f,5),
            [HumanoidParts.MucleGroupIndex.Prop] = new StateController.PropsBase(1.2f, 1.0f, 1f, 1f,10),
        }
    ),
    };
    private static readonly Dictionary<Global.HandAction, Global.MuscleGroupStateProps> normalHandStateProps = new Dictionary<Global.HandAction, Global.MuscleGroupStateProps>
    {
        [Global.HandAction.SLASH] = new Global.MuscleGroupStateProps(new Dictionary<short, StateController.PropsBase>
        {
            [HumanoidParts.MucleGroupIndex.HEAD] = new StateController.PropsBase(0.25f, 0f, 0.25f, 0.25f,0),
            [HumanoidParts.MucleGroupIndex.BODY] = new StateController.PropsBase(0.25f, 0f, 0.25f, 0.25f,0),
            [HumanoidParts.MucleGroupIndex.RHAND] = new StateController.PropsBase(1f, 0.1f, 0.8f, 0.8f,1),
            [HumanoidParts.MucleGroupIndex.LHAND] = new StateController.PropsBase(0.25f, 0.1f, 0.25f, 0.25f,1),
            [HumanoidParts.MucleGroupIndex.LFOOT] = new StateController.PropsBase(0.25f, 0f, 0.25f, 0.25f,0),
            [HumanoidParts.MucleGroupIndex.RFOOT] = new StateController.PropsBase(0.25f, 0f, 0.25f, 0.25f,0),
            [HumanoidParts.MucleGroupIndex.Prop] = new StateController.PropsBase(1.0f, 1.0f, 1f, 1f,1),
        }),
        [Global.HandAction.FREE] = null,
        [Global.HandAction.DEFENCE] = new Global.MuscleGroupStateProps(new Dictionary<short, StateController.PropsBase>
        {
            [HumanoidParts.MucleGroupIndex.RHAND] = new StateController.PropsBase(0.1f, 1f, 0.25f, 0.25f,5),
        }),
        [Global.HandAction.FETCH] = new Global.MuscleGroupStateProps(new Dictionary<short, StateController.PropsBase>
        {
            [HumanoidParts.MucleGroupIndex.RHAND] = new StateController.PropsBase(0, 0, 1, 1,1),
        }),
    };

    private static readonly Skill[] learnableSkills = {
        // 在这里 设置玩家可获得的技能
        new Skill("Combo3",2,null),
        new Skill("DashAttack",6,null),
        new Skill("SlashDouble",8,null),
    };
}
