using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using RootMotion.Dynamics;

public partial class PlayerController : ActorController
{
    public class PlayerBehaviours : ActorBehavioursBase<PlayerController, PlayerBehaviours> // 模板自包含 帮助内部类
    {
        PlayerHandBehaviours[] playerHandBehaviours;
        PlayerBodyBehaviours playerBodyBehaviour;

        protected override BodyBehaviours bodyBehaviour
        {
            get
            {
                return playerBodyBehaviour;
            }
        }
        protected override HandBehaviours[] handBehaviours
        {
            get
            {
                return playerHandBehaviours;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ac"></param>
        /// <param name="IKchains">要提供与propHandler数量相同的chain</param>
        /// <returns></returns>
        public PlayerBehaviours(PlayerController ac, FBIKChain[] iKchains,IKEffector[] iKEffectors):base(ac)
        {
            this.ac = ac;
            this.playerBodyBehaviour = new PlayerBodyBehaviours(this);
            PropHandler[] phs = ac.GetActorParts().propHandlers;
            this.playerHandBehaviours = new PlayerHandBehaviours[phs.Length];
            for (int i = 0; i < phs.Length; i++)
            {
                this.playerHandBehaviours[i] = new PlayerHandBehaviours(this, phs[i], iKchains[i],iKEffectors[i]);
            }
        }
        //TODO 改良 PreIK
        public override void OnPreIK(){
            // Transform rhand = humanoidParts.GetBodyPart(HumanBodyBones.RightHand).target;    
            // playerBodyBehaviours.RHandSkillPos = rhand.position;
            playerBodyBehaviour.RHandSkillPos = playerHandBehaviours[0].handMuscle.target.position;
        }

        private void SetAllHandWeight(float weight, float maxDelta = 0.0f)
        {
            if (maxDelta != 0.0f)
            {
                foreach (var hb in playerHandBehaviours)
                {
                    hb.IKWeight = weight;
                }
            }
            else
            {
                foreach (var hb in playerHandBehaviours)
                {
                    hb.IKWeight = Mathf.MoveTowards(hb.IKWeight, weight, maxDelta);
                }

            }
        }




        public class PlayerHandBehaviours : HandBehaviours
        {
            public float IKWeight = 0;
            FBIKChain handChain; // playerController 还使用了 ik 内容
            IKEffector handEffector;
            HumanoidParts hp;
            float curVer = 0.0f;
            Vector3 formerIKDir;
            internal bool isPicking = false;

            public PlayerHandBehaviours(PlayerBehaviours abb, PropHandler ph, FBIKChain hc,IKEffector iKEffector) : base(abb, ph)
            {
                handChain = hc;
                hp = ac.humanoidParts;
                handEffector = iKEffector;
                //Muscle handMuscle = ph.attachedMuscleGroup.muscles[ph.attachedMuscleGroup.muscles.Length - 1];// 最后一个位置即为 最末尾的 hand 的 muscle
            }
            // 外部回调
            // public void OnPreIK(){
            //     Rhan
            // }

            protected override void OnAttack()
            {
                IKWeight = 1f;
                Rigidbody r = hp.aimPart.GetComponent<Rigidbody>();
                Vector2 assumeDirRaw = ac.viewXY.normalized;
                Vector3 dir = CalDir(ref ac.viewXY);
                Vector3 vel = (hp.rigidRoot.position + dir * ac.atdeRange - hp.aimPart.position) * 30;
                r.velocity = Vector3.Lerp(r.velocity, vel, 0.8f);


                // 防止 time 的快速变换造成抖动
                //if (viewXY.magnitude >= 5f)
                // 如果挥动速度很快 则 不会在 挥动的 中途进行 方向的变更
                // 而不是直接使用 viewXY 来进行方向变更
                if (r.velocity.sqrMagnitude > 200f)
                //if (viewXY.magnitude >= 5f )
                {
                    //if(Vector3.Distance)
                    //humanoidParts.aimPart.GetComponent<Rigidbody>().AddForce(humanoidParts.rigidRoot.position + dir * atdeRange - humanoidParts.aimPart.position)/;
                    //humanoidParts.aimPart.rotation = RHandRot;
                    float x = ac.animationHash.slashSpeedX;
                    float y = ac.animationHash.slashSpeedY;
                    ac.animationHash.slashSpeedX = Mathf.Lerp(ac.animationHash.slashSpeedX, assumeDirRaw.x, 0.05F);
                    ac.animationHash.slashSpeedY = Mathf.Lerp(ac.animationHash.slashSpeedY, assumeDirRaw.y, 0.05F);
                    // ac.animationHash.slashSpeedX = slashXY.x;
                    // ac.animationHash.slashSpeedY = slashXY.y;
                    // 判断 前后 的 方向之间的 夹角 
                    // 如果 大于一定值 则 要求进行 过渡 动作 
                    if (assumeDirRaw.x * x + assumeDirRaw.y * y < 0.05f)
                        ac.animator.SetTrigger("attTri");

                    Vector2 slashOrigin = new Vector2(hp.rigidRoot.position.x, hp.rigidRoot.position.y) - 3.0f * assumeDirRaw;
                    Vector2 slashPos = new Vector2(hp.rigidRoot.position.x + dir.x, hp.rigidRoot.position.y + dir.y);
                    float slashTime = Vector2.Distance(slashOrigin, slashPos) / 4f;
                    ac.animationHash.time = slashTime;
                    //ac.animationHash.time = Mathf.MoveTowards(ac.animationHash.time,slashTime,0.1f);
                    //float vel1 = humanoidParts.aimPart.GetComponent<Rigidbody>().velocity.magnitude/100;
                }
                else
                { //如果鼠标移动速度很慢 将代表 只是切换姿势 而不是攻击
                    Vector3 localDir = hp.rigidRoot.InverseTransformVector(dir);
                    ac.animationHash.slashSpeedX = Mathf.MoveTowards(ac.animationHash.slashSpeedX, -localDir.normalized.x, 0.5f);
                    ac.animationHash.slashSpeedY = Mathf.MoveTowards(ac.animationHash.slashSpeedY, -localDir.normalized.y, 0.5f);
                    //ac.animationHash.slashSpeedY = -localDir.normalized.y;
                    //ac.animationHash.time = Mathf.MoveTowards(ac.animationHash.time,0.0f,0.3f);
                    ac.animationHash.time = 0.0f;
                }
                // Mouse ScrollWheel 用于 控制 攻击的 距离
                //humanoidParts.aimPart.position = Vector3.Lerp(humanoidParts.aimPart.position,humanoidParts.rigidRoot.position + dir * atdeRange,Time.deltaTime*10);
            }


            protected override void OnDefence()
            {
                IKWeight = 0.6f;
                // raw 即 直接 表示 鼠标的 移动方向
                Vector2 assumeDirRaw = ac.viewXY.normalized;
                Vector3 dir = CalDir(ref ac.viewXY);
                //humanoidParts.aimPart.position = humanoidParts.rigidRoot.position + dir * 2.0f;
                hp.aimPart.GetComponent<Rigidbody>().MovePosition(hp.rigidRoot.position + dir * 2.0f);

                if (ac.viewXY.magnitude >= 0.5f)
                {
                    ac.animationHash.slashSpeedX = assumeDirRaw.x;
                    ac.animationHash.slashSpeedY = assumeDirRaw.y;
                    ac.animationHash.time = 0.0f;
                }
            }
            protected override void OnFetch()
            {
                if (!propHandler.holdingProp)
                {
                    Transform handlerTran = propHandler.muscles[0].transform;
                    RaycastHit raycastHit;
                    if (Physics.SphereCast(hp.rigidRoot.position, 0.5f, ac.am.cameraHandler.transform.forward, out raycastHit, 2.0f, LayerMask.GetMask("weapon"), QueryTriggerInteraction.Ignore))
                    {
                        ac.am.LockOn(raycastHit.collider.transform);// 视角的局部锁定
                        float distance = Vector3.Distance(hp.rigidRoot.position, raycastHit.collider.transform.position);
                        handChain.reach = Mathf.MoveTowards(handChain.reach, 0.375f * distance, 0.05f);
                        handChain.pull = Mathf.MoveTowards(handChain.pull, 0.375f * distance, 0.05f);
                        IKWeight = 1.0f;
                        //humanoidParts.aimPart.position = Vector3.MoveTowards(humanoidParts.aimPart.position, raycastHit.collider.attachedRigidbody.position, 0.1f);
                        hp.aimPart.GetComponent<Rigidbody>().MovePosition(Vector3.MoveTowards(hp.aimPart.position, raycastHit.collider.attachedRigidbody.position, 0.05f));
                        //humanoidParts.GetBodyPart(HumanBodyBones.RightHand).rigidbody.isKinematic = true;


                        float d = Vector3.Distance(handlerTran.position, raycastHit.collider.attachedRigidbody.position);
                        if (d <= 0.5f && propHandler.currentProp == null)
                        {
                            //TEST  
                            //通过使用  IK target 来作为 spring joint 的连接起点 去连接武器手柄
                            //am.targetTest.transform.position = raycastHit.collider.attachedRigidbody.position;
                            ac.IgnoreHWCollision(raycastHit.collider, true);
                            Rigidbody r = hp.aimPart.GetComponent<Rigidbody>();
                            r.MovePosition(raycastHit.collider.attachedRigidbody.position);
                            r.velocity = Vector3.zero;
                            r.angularVelocity = Vector3.zero;
                            r.GetComponent<IKTarget>().updateRot();
                            propHandler.holdingProp = raycastHit.collider.gameObject.GetComponentInParent<PropBase>();
                            if (!propHandler.holdingProp.IsStabIn)// 武器不处于插入状态
                            {
                                propHandler.currentProp = propHandler.holdingProp;
                                handAction = Global.HandAction.FREE;
                                handChain.reach = 0.0f;
                                handChain.pull = 0.0f;
                                isPicking = false; propHandler.holdingProp = null;
                                // 解除视角的局部锁定
                                ac.am.unLock();
                                return;
                            }
                            return;
                        }
                    }
                    else
                    {
                        handChain.reach = 0.0f;
                        handChain.pull = 0.0f;
                        return;
                    }
                }
                else
                {
                    Collider weaponCollider = propHandler.holdingProp.GetComponentInChildren<Collider>();
                    ac.IgnoreHWCollision(weaponCollider, true);
                    ac.am.LockOn(weaponCollider.transform);
                    float distance = Vector3.Distance(hp.rigidRoot.position, propHandler.holdingProp.transform.position);
                    if (!propHandler.holdingProp.IsStabIn)
                    {
                        ac.IgnoreHWCollision(weaponCollider,false);
                        hp.propHandlers[0].currentProp = propHandler.holdingProp;
                        if(handAction == Global.HandAction.FETCH)
                            handAction = Global.HandAction.FREE;
                        handChain.reach = 0.0f;
                        handChain.pull = 0.0f;
                        propHandler.holdingProp = null;
                        // 解除视角的局部锁定
                        ac.am.unLock();
                        Vector3 force = hp.rigidRoot.InverseTransformVector(ac.viewXY);
                        hp.GetBodyPart(HumanBodyBones.RightHand).rigidbody.AddForce(force * 10, ForceMode.Impulse);
                        return;
                    }
                    else if (distance >= hp.GetMuscleGroup(HumanoidParts.MucleGroupIndex.RHAND).muscleLength * 1.8f) // 如果距离大于1.5倍的单臂长
                    {
                        ac.am.unLock();
                        propHandler.holdingProp = null;
                        propHandler.currentProp = null;
                        handAction = Global.HandAction.FREE;
                        handChain.reach = 0.0f;
                        handChain.pull = 0.0f;
                        return;
                    }
                    handChain.reach = Mathf.MoveTowards(handChain.reach, 0.375f * distance, 0.05f);
                    handChain.pull = Mathf.MoveTowards(handChain.pull, 0.375f * distance, 0.05f);
                    hp.aimPart.GetComponent<Rigidbody>().MovePosition(propHandler.holdingProp.r.position);
                    keepHandState = true; //不更新state的手部动作   
                    if (handAction == Global.HandAction.SLASH)
                    {
                        Vector3 force = hp.rigidRoot.InverseTransformVector(ac.viewXY);
                        //施加力量 来提起武器
                        propHandler.holdingProp.r.AddForce(force * 400, ForceMode.Force);
                    }
                }
            }

            protected override void OnNone()
            {
                formerIKDir = hp.aimPart.position - hp.rigidRoot.position;
                hp.aimPart.position = Vector3.Lerp(hp.aimPart.position, hp.GetBodyPart(HumanBodyBones.RightHand).transform.position, Time.deltaTime * 10.0f);
                hp.aimPart.GetComponent<Rigidbody>().MovePosition(Vector3.Lerp(hp.aimPart.position, hp.GetBodyPart(HumanBodyBones.RightHand).transform.position, Time.deltaTime * 10.0f));
                IKWeight = Mathf.SmoothDamp(IKWeight, 0.0f, ref curVer, 0.2f);
                if (ac.animationHash.slashSpeedX != 0.0f) ac.animationHash.slashSpeedX = Mathf.Lerp(ac.animationHash.slashSpeedX, 0.0f, Time.deltaTime * 3);
                if (ac.animationHash.slashSpeedY != 0.0f) ac.animationHash.slashSpeedY = Mathf.Lerp(ac.animationHash.slashSpeedY, 0.0f, Time.deltaTime * 3);
                if (ac.animationHash.time != 0.0f) ac.animationHash.time = Mathf.Lerp(ac.animationHash.time, 0.0f, Time.deltaTime * 3);
            }
            protected override void PostHandUpdate()
            {
                handEffector.positionWeight = IKWeight; 
            }
            protected override void PreFetch(){
                // if(propHandler.holdingProp!=null && (ac.formerHandAction == Global.HandAction.DEFENCE||ac.formerHandAction == Global.HandAction.SLASH)){
                //     handAction = ac.formerHandAction;// 保持之前的状态
                // }
            }

            /// <summary>
            /// 用于计算 手部动作的 指向的方向
            /// </summary>
            /// <param name="vecXY"></param>
            /// <returns></returns>

            private Vector3 CalDir(ref Vector2 vecXY)
            {
                Vector3 deltaVector = (hp.rigidRoot.forward + hp.rigidRoot.right * vecXY.x + hp.rigidRoot.up * vecXY.y).normalized;
                //  决定 挥动的速度
                Vector3 assumeDir = Vector3.SlerpUnclamped(hp.rigidRoot.forward, deltaVector, Mathf.Clamp(vecXY.magnitude / 20f, 0.0f, 3.0f));
                Quaternion qua = Quaternion.FromToRotation(hp.rigidRoot.forward, assumeDir);
                Vector3 dir = (qua * formerIKDir).normalized;
                //float angle = Vector3.Angle(rigidTran.forward, dir);


                // 限制 可以旋转的角度
                float angle = Vector3.SignedAngle(hp.rigidRoot.forward, dir, hp.rigidRoot.up);
                if (angle > 80.0f)
                {
                    dir = Vector3.Slerp(hp.rigidRoot.forward, dir, 80 / angle);
                }
                else if (angle < -70.0f)
                {
                    dir = Vector3.Slerp(hp.rigidRoot.forward, dir, 70 / -angle);
                }
                formerIKDir = dir;
                return dir;
            }
            
        }

        /// <summary>
        /// 每帧调用，根据当前身体状态进行格式参数变化 动画的播放 
        /// 并且根据其余参数来进行 特定状态的转态
        /// </summary>
        public class PlayerBodyBehaviours : BodyBehaviours
        {
            public float velY;
            float curVer = 0.0f;
            HumanoidParts hp;
            /// <summary>
            /// 主要用于 剑技的释放的 IK 要先 得到 原位置
            /// </summary>
            protected Vector3 rhandSkillPos;
            protected Vector3 formerRhandSkillPos;

            public Vector3 RHandSkillPos
            {
                get { return rhandSkillPos; }
                set { formerRhandSkillPos = rhandSkillPos; rhandSkillPos = value; }
            }

            /// <summary>
            /// 表示
            /// </summary>
            Vector2 SkillOffsetVec = Vector2.zero;
            public float curBias = 1.0f;

            public PlayerBodyBehaviours(PlayerBehaviours abb) : base(abb)
            { hp = abb.ac.humanoidParts; }

            protected override void PostBodyUpdate()
            {
                velY = ac.rigid.velocity.y;
                if (Mathf.Abs(velY) > 0.5f)
                {
                    ac.Air();
                }
            }
            protected override void OnCantInput()
            {
                abb.SetAllHandAction(Global.HandAction.FREE);
                abb.SetAllHandWeight(0.0f, 0.2f);

                ac.animationHash.SetLayerWeight(1, 0.0f);
            }
            /// <summary>
            /// 剑技 手部动作更新函数
            /// </summary>
            protected override void OnSkill()
            {
                //主参数设置
                abb.IgnoreHandUpdate();
                abb.SetAllHandWeight(1);
                ac.animationHash.SetLayerWeight(1, 0.0f);
                ac.animationHash.defence = false;
                ac.animationHash.attack = false;
                // 点击了鼠标左键 代表期望 进行 手动操纵
                if (abb.playerHandBehaviours[0].handAction== Global.HandAction.SLASH)//TODO 解决耦合
                {
                    SkillOffsetVec.Set(SkillOffsetVec.x + ac.viewXY.x / 2, SkillOffsetVec.y - ac.viewXY.y / 2);
                }
                else
                {
                    abb.SetAllHandAction(Global.HandAction.FREE);
                    SkillOffsetVec = Vector2.Lerp(SkillOffsetVec, Vector2.zero, Time.deltaTime * 3);
                }
                Vector3 formerAimPos = hp.aimPart.position;
                Vector3 Rid2Hand = RHandSkillPos - hp.rigidRoot.position;
                Quaternion quat = Quaternion.AngleAxis(SkillOffsetVec.x, hp.rigidRoot.up);
                quat *= Quaternion.AngleAxis(SkillOffsetVec.y, hp.rigidRoot.right);
                //hp.aimPart.position = hp.rigidRoot.position + quat * Rid2Hand;
                Vector3 targetPos = hp.rigidRoot.position + quat * Rid2Hand;
                hp.aimPart.GetComponent<Rigidbody>().MovePosition(targetPos);

                // 计算偏离值

                Vector3 slashSkillDir = (RHandSkillPos - formerRhandSkillPos) / Time.deltaTime;
                Vector3 slashDir = (targetPos - formerAimPos) / Time.deltaTime;
                float bias = Vector3.Dot(slashSkillDir, slashDir);
                bias = Mathf.Atan(bias / 50) * 2 / Mathf.PI; //通过 函数 将正负无穷映射到 -1 到 1 之间
                float lastBias = curBias;
                //  bias 很大 或 小于负数 很小时候 要 加快变化的步伐
                if (bias > 0.8f || bias <= -0.0f)
                {
                    curBias = Mathf.MoveTowards(curBias, bias, 0.1f);
                }
                else
                {
                    curBias = Mathf.MoveTowards(curBias, bias, 0.01f);
                }
                // //TODO 当bias 小于等于某个值的 时候 要求中止 剑技，
                //     // 并通过 cross 来 影响 动画过渡时间 以来达到 延迟硬直效果
                // if(curBias < -0.01f && lastBias < -0.01f){
                //     print(curBias +" " + lastBias);
                //     animator.CrossFadeInFixedTime("Ground",3.0f,0);
                //     animator.SetFloat("skillspeed",0.1f);
                //     curBias = 1.0f;
                // }
                // 偏离值将会 影响到 剑技的释放速度
                ac.animator.SetFloat("skillspeed", 0.5f + curBias * 0.7f);
                ac.am.OnSkill(RHandSkillPos, 0.2f + curBias,abb.playerHandBehaviours[0].propHandler.currentProp);
            }

            /// <summary>
            /// 空中动作update函数
            /// </summary>
            protected override void OnAir()
            {
                velY = ac.rigid.velocity.y;
                ac.animationHash.Jump = velY;
                if (velY < -15.0f)
                {
                    ac.Fall();
                }
                else if (velY < -10.0f)
                {
                    ac.animationHash.fallStun = true;
                }
            }
            protected override void PreSkill(){
                abb.NoDrop(true);
            }
            protected override void PreGround(){
                abb.NoDrop(false);
                ac.am.puppet.angularLimits = true;
                // 刷新
                SkillOffsetVec.Set(0,0);
                curBias = 0;
            }
        }
    }
}