using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion;
using RootMotion.Dynamics;

public class SubBattleManager : SubManagerBase
{
    public SubBattleManager(ActorManager am) : base(am){
        //am.state.BehaPuppet.OnCollisionImpulse = Damage;
    }

    public void Damage(MuscleCollision mc,float impluse){
        Muscle m = am.puppet.muscles[mc.muscleIndex];

        //目前击中 武器的话 不应该对自身造成伤害
        if(m.props.group == Muscle.Group.Prop) return;

        StateController.GroupProp gp = am.state.GetBaseProps(am.actorParts.GetMuscleGroup(mc.muscleIndex));

        //TODO 细化伤害公式
            //CollisionResistance 的考虑
        float damage = impluse * (1 - gp.curProp.Toughness);

        am.state.TryDoDamage(damage);
    }



}
