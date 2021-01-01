using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///  可能需要 SkillController 独自运行 来 实时显示 当前对应的可使用技能
/// </summary>
public class SkillController : MonoBehaviour
{
    // 已经学习到的技能
    Skill[] skills;
    Skill[] equipedSkill;
    // 
    SkillZone skillZone;
    Transform rigidTran;
    public void Init(ActorParts ap){
        rigidTran = ap.rigidRoot;
        //TODO 技能的学习
            skills = ap.GetLearnableSkill();


        if(equipedSkill ==null){
            equipedSkill = new Skill[9];
        }
        skillZone = new SkillZone(ap);
    }

    // 将指定技能装备到 对应的zone  区域上
    public void  equipSkill(Skill s){
        int i=0;
        for(; i< skills.Length;i++){
            if(s == skills[i]) break;
        }
        if(i == skills.Length){
            throw new System.Exception("Skill Not Learned");
        }
        
        // 装备上对应的 技能
        equipedSkill[s.zoneType] = s;
    }
    private void Update() {
        // Debug.DrawLine(rigidTran.position + rigidTran.TransformVector(new Vector3(skillZone.skillZones[0].x,skillZone.skillZones[0].y,0)), rigidTran.position + rigidTran.TransformVector(new Vector3(skillZone.skillZones[2].x + skillZone.sideLength,skillZone.skillZones[2].y,0)),Color.red);
        // Debug.DrawLine(rigidTran.position + rigidTran.TransformVector(new Vector3(skillZone.skillZones[0].x,skillZone.skillZones[0].y - skillZone.sideLength,0)), rigidTran.position + rigidTran.TransformVector(new Vector3(skillZone.skillZones[2].x + skillZone.sideLength,skillZone.skillZones[2].y - skillZone.sideLength,0)),Color.red);
        // Debug.DrawLine(rigidTran.position + rigidTran.TransformVector(new Vector3(skillZone.skillZones[3].x,skillZone.skillZones[3].y,0)), rigidTran.position + rigidTran.TransformVector(new Vector3(skillZone.skillZones[5].x+ skillZone.sideLength,skillZone.skillZones[5].y,0)),Color.red);
        // Debug.DrawLine(rigidTran.position + rigidTran.TransformVector(new Vector3(skillZone.skillZones[3].x,skillZone.skillZones[3].y - skillZone.sideLength,0)), rigidTran.position + rigidTran.TransformVector(new Vector3(skillZone.skillZones[5].x + skillZone.sideLength,skillZone.skillZones[5].y - skillZone.sideLength,0)),Color.red);
        // Debug.DrawLine(rigidTran.position + rigidTran.TransformVector(new Vector3(skillZone.skillZones[6].x,skillZone.skillZones[6].y,0)) , rigidTran.position +rigidTran.TransformVector( new Vector3(skillZone.skillZones[8].x+ skillZone.sideLength,skillZone.skillZones[8].y,0)),Color.red);
        // Debug.DrawLine(rigidTran.position + rigidTran.TransformVector(new Vector3(skillZone.skillZones[0].x,skillZone.skillZones[0].y - skillZone.sideLength,0)), rigidTran.position + rigidTran.TransformVector(new Vector3(skillZone.skillZones[2].x + skillZone.sideLength,skillZone.skillZones[2].y - skillZone.sideLength,0)),Color.red);


        // Debug.DrawLine(rigidTran.position + rigidTran.TransformVector(new Vector3(skillZone.skillZones[1].x,skillZone.skillZones[1].y,0)), rigidTran.position + rigidTran.TransformVector(new Vector3(skillZone.skillZones[7].x,skillZone.skillZones[7].y- skillZone.sideLength,0)),Color.red);
        // Debug.DrawLine(rigidTran.position + rigidTran.TransformVector(new Vector3(skillZone.skillZones[2].x,skillZone.skillZones[2].y,0)), rigidTran.position + rigidTran.TransformVector(new Vector3(skillZone.skillZones[8].x,skillZone.skillZones[8].y- skillZone.sideLength,0)),Color.red);
        // Debug.DrawLine(rigidTran.position + rigidTran.TransformVector(new Vector3(skillZone.skillZones[0].x,skillZone.skillZones[0].y,0)), rigidTran.position + rigidTran.TransformVector(new Vector3(skillZone.skillZones[6].x,skillZone.skillZones[6].y- skillZone.sideLength,0)),Color.red);

        // Debug.DrawLine(rigidTran.position + rigidTran.TransformVector(new Vector3(skillZone.skillZones[1].x+skillZone.sideLength,skillZone.skillZones[1].y,0)), rigidTran.position + rigidTran.TransformVector(new Vector3(skillZone.skillZones[7].x+skillZone.sideLength,skillZone.skillZones[7].y- skillZone.sideLength,0)),Color.red);
        // Debug.DrawLine(rigidTran.position + rigidTran.TransformVector(new Vector3(skillZone.skillZones[2].x+skillZone.sideLength,skillZone.skillZones[2].y,0)), rigidTran.position + rigidTran.TransformVector(new Vector3(skillZone.skillZones[8].x+skillZone.sideLength,skillZone.skillZones[8].y- skillZone.sideLength,0)),Color.red);
        // Debug.DrawLine(rigidTran.position + rigidTran.TransformVector(new Vector3(skillZone.skillZones[0].x+skillZone.sideLength,skillZone.skillZones[0].y,0)), rigidTran.position + rigidTran.TransformVector(new Vector3(skillZone.skillZones[6].x+skillZone.sideLength,skillZone.skillZones[6].y- skillZone.sideLength,0)),Color.red);
    }



    public Skill GetSkillByAction(Vector3 dir){
        int index = skillZone.JudgeZone(dir);
        if(index == -1) return null;
        return equipedSkill[index];
    } 




}
