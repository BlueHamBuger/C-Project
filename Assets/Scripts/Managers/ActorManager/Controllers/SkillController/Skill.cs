using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 关于 Skill 的数据结构物体
///     之后可能使用 AOV 结构 来实现 技能的 学习性
/// </summary>
public class Skill
{
    /// <summary>
    /// skillName 应该要和 动画的对应状态名保持一致
    /// </summary>
    public readonly string skillName;
    /// <summary>
    /// 决定 这个剑技应该摆放的位置
    /// </summary>
    public readonly short zoneType;
    private Global.MuscleGroupStateProps propCorrection;


    // new Global.MuscleGroupStateProps(
    //     new Dictionary<short, StateController.PropsBase>
    //     {
    //         [HumanoidParts.MucleGroupIndex.HEAD] = new StateController.PropsBase(1.0f, 1.0f, 1.0f, 1.0f),
    //         [HumanoidParts.MucleGroupIndex.BODY] = new StateController.PropsBase(1.0f, 1.0f, 1.0f, 1.0f),
    //         [HumanoidParts.MucleGroupIndex.RHAND] = new StateController.PropsBase(1.0f, 1.0f, 1.0f, 1.0f),
    //         [HumanoidParts.MucleGroupIndex.LHAND] = new StateController.PropsBase(1.0f, 1.0f, 1.0f, 1.0f),
    //         [HumanoidParts.MucleGroupIndex.LFOOT] = new StateController.PropsBase(1.0f, 1.0f, 1.0f, 1.0f),
    //         [HumanoidParts.MucleGroupIndex.RFOOT] = new StateController.PropsBase(1.0f, 1.0f, 1.0f, 1.0f),
    //     }
    // ),

    public Skill(string name, short zoneType ,Dictionary<short, StateController.PropsBase> SkillProp)
    {
        skillName = name;
        propCorrection = new Global.MuscleGroupStateProps(SkillProp);
        this.zoneType = zoneType;
    }
}
