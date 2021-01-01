using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController : ActorController
{
    // 提供快速的hash化 动画查找
    private class AnimationHash:AnimationHashBase
    {
        //hashCodes 
        static int slashSpeedXHashCode = Animator.StringToHash("slashSpeedX");
        static int slashSpeedYHashCode = Animator.StringToHash("slashSpeedY");
        public float slashSpeedX
        {
            get
            {
                return animator.GetFloat(slashSpeedXHashCode);
            }
            set
            {
                animator.SetFloat(slashSpeedXHashCode, value);
            }
        }
        public float slashSpeedY
        {
            get
            {
                return animator.GetFloat(slashSpeedYHashCode);
            }
            set
            {
                animator.SetFloat(slashSpeedYHashCode, value);
            }
        }

        public AnimationHash(Animator animator):base(animator){}
    }

}
