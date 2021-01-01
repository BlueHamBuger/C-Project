using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 玩家技能的触发区域判定类
/// </summary>
public class SkillZone{

    /// <summary>
    /// 对应 玩家 剑技的 触发方块
    ///     位置 为 每一个zone 的 左上角
    /// /// </summary>
    public Vector2[] skillZones;
    public float sideLength;
    // 技能范围之间的 间距 
        // 防止 临界抖动等等
    public float padding;

    // 需要玩家的 ActorParts
    public SkillZone(ActorParts ap){
        sideLength = ap.GetMuscleGroup("RightHand").muscleLength;
        skillZones = new Vector2[9];
        padding = 0.2f*sideLength;
        float xFactor = -2.5f;
        float yFactor = 2.5f;
        for(int i=0;i<3;i++){
            yFactor-=1;
            for(int j=0;j<3;j++){
                xFactor+=1;
                Vector2 orgPoint = new Vector2(xFactor*sideLength,yFactor*sideLength);
                int offsetx =j - 1;int offsety = 1 - i;
                skillZones[i*3+j] = new Vector2(orgPoint.x + offsetx*padding , orgPoint.y + offsety * padding);
            }   
            xFactor = -2.5f;
        }
    }

    /// <summary>
    /// 根据 给与的 IK的 dir 来判定其 处于哪一个zone
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public int JudgeZone(Vector3 dir){
        // 只需要两个方向值
        Vector2 dir2 = dir;
        //判断 raw
        int raw;
        if(dir2.y<=skillZones[6].y && dir2.y>=skillZones[6].y-sideLength){
            raw = 2;
        }else if(dir2.y <= skillZones[3].y && dir2.y>=skillZones[3].y-sideLength){
            raw = 1;
        }else if(dir2.y <= skillZones[0].y && dir2.y>=skillZones[0].y-sideLength){
            raw =0;
        }else{
            return -1;
        }

        // 判断 col
        if(dir2.x >= skillZones[2].x && dir2.x <= skillZones[2].x + sideLength){
            return raw*3 + 2;
        }else if(dir2.x >= skillZones[1].x && dir2.x <= skillZones[1].x + sideLength){
            return raw*3 + 1;
        }else if(dir2.x >= skillZones[0].x && dir2.x <= skillZones[0].x + sideLength){
            return raw*3;
        }
        return -1;
    }

}
