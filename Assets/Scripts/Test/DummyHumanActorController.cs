using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion;
using RootMotion.Dynamics;


public class DummyHumanActorController : ActorController
{

    public HumanoidParts humanoidParts;
    AnimationHashBase animationHash;
    protected override AnimationHashBase animationHashBase
    {
        get { return animationHash; }
    }
    //public override Animation
    public override void Init(ActorManager am)
    {
        base.Init(am);
        HumanoidParts temp = new HumanoidParts(am.puppet);
        temp.aimPart = humanoidParts.aimPart;
        temp.rigidRoot = humanoidParts.rigidRoot;
        humanoidParts = temp;
    }
    public override void LateInit() { }
    public override ActorParts GetActorParts()
    {
        return humanoidParts;
    }


    // 基本actions

    //public override void HandAction(Global.HandAction handAction, Vector2 viewXY){}
    public override void AdRange(float deltaRange) { }
    public override void Move(Vector2 moveXY) { }
    public override void Run(bool run) { }
    public override void Dodge() { }
    public override void Fall() { }
    public override void OnRegainBalance() { }
    public override void Jump(float val) { }
    public override void GetUp() { }

}
