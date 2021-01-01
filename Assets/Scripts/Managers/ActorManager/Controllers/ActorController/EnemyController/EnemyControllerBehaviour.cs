using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class EnemyController : ActorController
{
    public class EnemyBehaviours : ActorBehavioursBase<EnemyController, EnemyBehaviours> // 模板自包含 帮助内部类
    {
        EnemyBodyBehaviours enemyBodyBehaviour;
        EnemyHandBehaviours[] enemyHandBehaviours;

        public EnemyBehaviours(EnemyController ac) : base(ac)
        {
            enemyBodyBehaviour = new EnemyBodyBehaviours(this);
            enemyHandBehaviours = new EnemyHandBehaviours[]{new EnemyHandBehaviours(this,ac.humanoidParts.propHandlers[0])};
        }

        protected override BodyBehaviours bodyBehaviour {
            get{return enemyBodyBehaviour;}
        }

        protected override HandBehaviours[] handBehaviours {
            get{return enemyHandBehaviours;}
        }

        public class EnemyHandBehaviours : HandBehaviours
        {
            public EnemyHandBehaviours(ActorBehavioursBase<EnemyController, EnemyBehaviours> abb, PropHandler ph) : base(abb, ph)
            {}
        }
        public class EnemyBodyBehaviours : BodyBehaviours
        {
            public EnemyBodyBehaviours(ActorBehavioursBase<EnemyController, EnemyBehaviours> abb) : base(abb)
            {}

            protected override void  OnSkill(){
                var hand = abb.enemyHandBehaviours[0];
                ac.am.OnSkill(Vector3.zero,1,hand.propHandler.currentProp);
            }
        }
    }
}