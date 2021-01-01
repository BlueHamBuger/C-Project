using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class Global
{
    public static PhysicMaterial normalPM;
    public static PhysicMaterial zeroPM;
    public static PhysicMaterial lowPM;


    public enum HandAction
    {
        SLASH,
        STAB,
        FETCH,  // 拾取某物物体 比如 拾取武器等等
        DEFENCE,
        FREE,
        NONE,
    }

    public enum BodyAction
    {
        FALLING,// Falling 表示身体已经失衡
        GETUP, // 正在起身
        AIR,// 表示 身体未失衡 但是 在空中
        GROUND,//表示正常姿态
        STUN,//落地等等姿态造成的 延迟
        DIE,
        SKILL, // 借助系统辅助 使用技能
        FETCH,
        NONE,
    }

    public enum MuscleActionState
    {
        SLASH,
        STAB,
        FETCH,
        DEFENCE,
        SKILL,
        NONE,
    }

    public class MuscleGroupStateProps
    {
        private Dictionary<short, StateController.PropsBase> groupProp;

        public MuscleGroupStateProps(Dictionary<short, StateController.PropsBase> g)
        {
            groupProp = g;
        }
        public StateController.PropsBase GetGroupProp(ActorParts.MuscleGroup mg)
        {

            return groupProp.ContainsKey(mg.GroupIndex) ? groupProp[mg.GroupIndex] : null;
        }
        public static MuscleGroupStateProps operator +(MuscleGroupStateProps m1, MuscleGroupStateProps m2)
        {

            Dictionary<short, StateController.PropsBase> tempDic = new Dictionary<short, StateController.PropsBase>();
            if (m1 != null)
            {
                foreach (var entry in m1.groupProp)
                {
                    tempDic[entry.Key] = new StateController.PropsBase(0, 0, 0, 0,0);
                    tempDic[entry.Key].SetValue(entry.Value);
                }
            }
            if (m2 != null)
            {
                foreach (var entry in m2.groupProp)
                {
                    tempDic[entry.Key].Accuracy += entry.Value.Accuracy;
                    tempDic[entry.Key].Agility += entry.Value.Agility;
                    tempDic[entry.Key].Power += entry.Value.Power;
                    tempDic[entry.Key].Toughness += entry.Value.Toughness;
                    tempDic[entry.Key].Tolerance += entry.Value.Tolerance;
                }
            }
            return new MuscleGroupStateProps(tempDic);
        }
    }

    // 通用委托
    public delegate void Del0();
    public delegate void TransformDel(Transform t);

    // Helps
    public static void Tranverse(Transform t,TransformDel callback){
        foreach(Transform t1 in t){
            callback(t1);
            Tranverse(t1,callback);
        }
    }



}
