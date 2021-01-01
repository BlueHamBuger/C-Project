using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract partial class ActorController : MonoBehaviour
{

    public interface IActorBehaviour
    {
        void AddListeners();
        void Update();
        void OnPreIK();
        void SetAllHandAction(Global.HandAction ha);
        void SetBodyAction(Global.BodyAction ba);

        /// <summary>
        /// 判断指定的ph 是否处于 pick状态
        /// </summary>
        /// <param name="ph"></param>
        /// <returns></returns>
    }

    public abstract class ActorBehavioursBase<ACType, ABB> : IActorBehaviour where ACType : ActorController 
    where ABB : ActorBehavioursBase<ACType, ABB>
    {
        public ACType ac
        {
            get; protected set;
        }
        protected abstract BodyBehaviours bodyBehaviour { get; }
        protected abstract HandBehaviours[] handBehaviours { get; }
        protected ActorBehavioursBase(ACType ac)
        {
            this.ac = ac;
        }
        public virtual void AddListeners() { }
        public virtual void Update()
        {
            bodyBehaviour.Update();
            foreach (var hb in handBehaviours)
            {
                hb.Update();
            }
        }
        // 状态设置
        public void SetAllHandAction(Global.HandAction ha)
        {
            foreach (var hb in handBehaviours)
            {
                hb.handAction = ha;
            }
        }
        public void SetBodyAction(Global.BodyAction ba)
        {
            bodyBehaviour.bodyAction = ba;
        }
        public virtual void OnPreIK() { }

        protected HandBehaviours GetHandBehaviours(PropHandler ph)
        {
            foreach (var hb in handBehaviours)
            {
                if (hb.propHandler == ph)
                    return hb;
            }
            return null;
        }
        protected void IgnoreHandUpdate()
        {
            foreach (var hb in handBehaviours)
                hb.ignoreHandUpdate = true;
        }
        protected void KeepHandState()
        {
            foreach (var hb in handBehaviours)
                hb.keepHandState = true;
        }


        protected void NoDrop(bool t)
        {
            if (t)
                foreach (var ph in handBehaviours)
                {
                    ph.propHandler.DisableDetector(); // 静止武器嵌入功能
                }
            else
            {
                foreach (var ph in handBehaviours)
                {
                    ph.propHandler.EnableDetector(); // 静止武器嵌入功能
                }
            }
        }

        protected void DropProps()
        {
            foreach (var ph in handBehaviours)
            {
                ph.propHandler.currentProp = null; // 静止武器嵌入功能
            }
        }

        // 定义的内部类

        public abstract class BodyBehaviours
        {
            public Global.BodyAction bodyAction = Global.BodyAction.GROUND; // 在内部处理完最后的 bodyAction 直到帧的结束的时候
                                                                            //才将此 action上传给state ， 一方面减少 state 在状态泛化的时候进行的无用的数值修改
                                                                            // 另一方面 可以知晓前一次的actionstate
            protected ABB abb;
            protected ACType ac;
            public BodyBehaviours(ActorBehavioursBase<ACType, ABB> abb)
            {
                this.abb = (ABB)abb;
                this.ac = abb.ac;
            }

            public void Update()
            {
                // 过滤 BodyAction
                if (!ac.CanInput())
                {
                    OnCantInput();
                }
                bool changed = (bodyAction != ac.curBodyAction);
                switch (bodyAction)
                {
                    case Global.BodyAction.SKILL:
                        if (changed) PreSkill();
                        OnSkill();
                        break;
                    case Global.BodyAction.AIR:
                        if (changed) PreAir();
                        OnAir();
                        break;
                    case Global.BodyAction.FALLING:
                        if (changed) PreFall();
                        OnFalling();
                        break;
                    case Global.BodyAction.GETUP:
                        if (changed) PreGetUp();
                        OnGetUp();
                        break;
                    case Global.BodyAction.GROUND:
                        if (changed) PreGround();
                        PostBodyUpdate();
                        break;
                }
                ac.curBodyAction = bodyAction;
            }
            //动作函数
            protected virtual void OnSkill() { }
            protected virtual void OnAir() { }
            protected virtual void OnFalling() { }
            protected virtual void OnGetUp() { }
            protected virtual void OnCantInput() { }
            protected virtual void PostBodyUpdate() { }

            protected virtual void PreFall() { }
            protected virtual void PreAir() { }
            protected virtual void PreGround() { }
            protected virtual void PreSkill() { }
            protected virtual void PreGetUp() { }

        }

        public abstract class HandBehaviours
        {
            public PropHandler propHandler
            {
                get; private set;
            }
            public Global.HandAction handAction = Global.HandAction.FREE;
            public bool ignoreHandUpdate = false; // 跳过手部的更新操作 
            public bool keepHandState = false;  // 不用 handAction 来更新stateController 适用于一些子操作
            protected ABB abb;
            protected ACType ac;
            protected ActorParts.MuscleGroup muscleGroup;
            public RootMotion.Dynamics.Muscle handMuscle;

            public HandBehaviours(ActorBehavioursBase<ACType, ABB> abb, PropHandler ph)
            {
                propHandler = ph;
                this.abb = (ABB)abb;
                this.ac = abb.ac;
                muscleGroup = ph.attachedMuscleGroup;
                handMuscle = muscleGroup.muscles[muscleGroup.muscles.Length - 1];
            }

            public void Update()
            {
                if (ignoreHandUpdate) { ignoreHandUpdate = false; ac.curHandAction = handAction; }
                else
                {
                    Global.HandAction ha = handAction;
                    if (keepHandState)
                    {
                        keepHandState = false;
                        ha = ac.curHandAction;
                    }
                    else if (handAction != ac.curHandAction)
                    {
                        ac.curHandAction = handAction;
                        PreUpdates(handAction);
                    }
                    Updates(ha);
                }
                PostHandUpdate();
            }
            private void Updates(Global.HandAction ha)
            {
                switch (ha)
                {
                    case Global.HandAction.SLASH:
                        OnAttack();
                        break;
                    case Global.HandAction.DEFENCE:
                        OnDefence();
                        break;
                    case Global.HandAction.FREE:
                        OnNone();
                        break;
                    case Global.HandAction.FETCH:
                        OnFetch();
                        break;
                    default:
                        break;
                }
            }
            private void PreUpdates(Global.HandAction ha)
            {
                switch (ha)
                {
                    case Global.HandAction.SLASH:
                        PreAttack();
                        break;
                    case Global.HandAction.DEFENCE:
                        PreDefence();
                        break;
                    case Global.HandAction.FREE:
                        PreNone();
                        break;
                    case Global.HandAction.FETCH:
                        PreFetch();
                        break;
                    default:
                        break;
                }
            }
            protected virtual void OnAttack() { }
            protected virtual void OnDefence() { }
            protected virtual void OnNone() { }
            protected virtual void OnFetch() { }
            protected virtual void PostHandUpdate() { }
            protected virtual void PreAttack() { }
            protected virtual void PreDefence() { }
            protected virtual void PreNone() { }
            protected virtual void PreFetch() { }
        }
    }

    protected class AnimationHashBase
    {
        protected Animator animator;
        //hashCodes 
        static int speedXHashCode = Animator.StringToHash("speedX");
        static int speedYHashCode = Animator.StringToHash("speedY");
        static int dodgeHashCode = Animator.StringToHash("dodge");
        static int IKweightHashCode = Animator.StringToHash("IKweight");
        static int timeHashCode = Animator.StringToHash("time");
        static int fallBlendCode = Animator.StringToHash("FallBlend");
        static int attackHashCode = Animator.StringToHash("attack");
        static int defenceHashCode = Animator.StringToHash("defence");
        static int airHashCode = Animator.StringToHash("air");
        static int rollHashCode = Animator.StringToHash("roll");
        static int fallStunCode = Animator.StringToHash("FallStun");
        static int jumpCode = Animator.StringToHash("Jump");
        static int jumpLegCode = Animator.StringToHash("JumpLeg");
        static int turnCode = Animator.StringToHash("turn");
        static int handTypeCode = Animator.StringToHash("handType");




        public float speedX
        {
            get
            {
                return animator.GetFloat(speedXHashCode);
            }
            set
            {
                animator.SetFloat(speedXHashCode, value);
            }
        }
        public float speedY
        {
            get
            {
                return animator.GetFloat(speedYHashCode);
            }
            set
            {
                animator.SetFloat(speedYHashCode, value);
            }
        }
        public float turn
        {
            get
            {
                return animator.GetFloat(turnCode);
            }
            set
            {
                animator.SetFloat(turnCode, value);
            }
        }
        public float IKweight
        {
            get
            {
                return animator.GetFloat(IKweightHashCode);
            }
            set
            {
                animator.SetFloat(IKweightHashCode, value);
            }
        }
        public float time
        {
            get
            {
                return animator.GetFloat(timeHashCode);
            }
            set
            {
                animator.SetFloat(timeHashCode, value);
            }
        }


        public float fallBlend
        {
            get
            {
                return animator.GetFloat(fallBlendCode);
            }
            set
            {
                animator.SetFloat(fallBlendCode, value);
            }
        }

        public float Jump
        {
            get
            {
                return animator.GetFloat(jumpCode);
            }
            set
            {
                animator.SetFloat(jumpCode, value);
            }
        }
        public float JumpLeg
        {
            get
            {
                return animator.GetFloat(jumpLegCode);
            }
            set
            {
                animator.SetFloat(jumpLegCode, value);
            }
        }
        // int
        public int handType
        {
            get
            {
                return animator.GetInteger(handTypeCode);
            }
            set
            {
                animator.SetInteger(handTypeCode, value);
            }
        }


        // trigger and bool
        public bool dodge
        {
            set
            {
                if (value)
                    animator.SetTrigger(dodgeHashCode);
                else
                    animator.ResetTrigger(dodgeHashCode);
            }
        }
        public bool roll
        {
            set
            {
                if (value)
                    animator.SetTrigger(rollHashCode);
                else
                    animator.ResetTrigger(rollHashCode);
            }
        }
        public bool attack
        {
            get
            {
                return animator.GetBool(attackHashCode);
            }
            set
            {
                animator.SetBool(attackHashCode, value);
            }
        }
        public bool defence
        {
            get
            {
                return animator.GetBool(defenceHashCode);
            }
            set
            {
                animator.SetBool(defenceHashCode, value);
            }
        }

        public bool air
        {
            get
            {
                return animator.GetBool(airHashCode);
            }
            set
            {
                animator.SetBool(airHashCode, value);
            }
        }

        public bool fallStun
        {
            set
            {
                if (value)
                    animator.SetTrigger(fallStunCode);
                else
                    animator.ResetTrigger(fallStunCode);
            }
        }

        public AnimationHashBase(Animator animator)
        {
            this.animator = animator;
        }

        public void SetLayerWeight(int layerIndex, float weight)
        {
            animator.SetLayerWeight(layerIndex, weight);
        }

        public IEnumerator SetLayerWeightGrd(int layerIndex, float weight, float time)
        {
            float oriWeight = animator.GetLayerWeight(layerIndex);
            float curWeight = oriWeight;
            float delta = 0;

            while (delta < 1)
            {
                curWeight = Mathf.Lerp(oriWeight, weight, delta);
                animator.SetLayerWeight(layerIndex, curWeight);
                delta += Time.deltaTime / time;
                yield return new WaitForEndOfFrame();
            }
            animator.SetLayerWeight(layerIndex, 1);
        }

        // 检查 是否处于对应的 状态
        bool CheckStateAndTransition(string name, int layerIndex = 0)
        {
            AnimatorStateInfo animatorState = animator.GetCurrentAnimatorStateInfo(layerIndex);
            AnimatorTransitionInfo animatorTransition = animator.GetAnimatorTransitionInfo(layerIndex);
            return animatorState.IsTag(name) || animatorTransition.IsUserName(name);
        }
    }



}
