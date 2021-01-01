using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;

public class ActorManager : ManagerBase
{
    // 控制器
    public ActorController actorController;
    public SkillController skillController;
    public CameraController cameraHandler;

    //StateController 持有
    public StateController state;

    // 子管理器
    public SubBattleManager battleMng;
    public SubEffectManager effectMng;
    public SubMessageManager messageMng = new SubMessageManager();

    public ActorParts actorParts;
    public PuppetMaster puppet;

    // public bool frozen;
    public Dictionary<string, System.Delegate> eventTable
    {
        get { return this.messageMng.eventTable; }
    }

    // events
    /// <summary>
    /// 在 stateController 中进行调用
    /// </summary>
    public InputManager.BodyActionDele OnBodyStateChange = null;
    public InputManager.HandActionDele OnHandStateChange = null;
    public PuppetMaster.UpdateDelegate OnPreIK
    {
        get
        {
            return puppet.OnPreIK;
        }
        set
        {
            puppet.OnPreIK = value;
        }
    }





    public bool controlling = false;
    public override void Init()
    {
        puppet = GetComponentInChildren<PuppetMaster>();
        if (!puppet.initiated) puppet.Initiate();
        actorController.Init(this);
        actorParts = actorController.GetActorParts();
        Global.Tranverse(actorParts.puppetRoot,EnableCols);
        if (controlling)
        {
            cameraHandler.am = this;
            var gm = GetGM();
            gm.AddEvent("Move", (InputManager.Value2)actorController.Move);
            gm.AddEvent("View", (InputManager.Value2)actorController.View);
            gm.AddEvent("Defence", (InputManager.Bool1)actorController.Defence);
            gm.AddEvent("Attack", (InputManager.Bool1)actorController.Attack);
            gm.AddEvent("View", (InputManager.Value2)cameraHandler.View);
            gm.AddEvent("AdRange", (InputManager.Value1)actorController.AdRange);
            gm.AddEvent("Run", (InputManager.Bool1)actorController.Run);
            gm.AddEvent("Dodge", (InputManager.Value0)actorController.Dodge);
            gm.AddEvent("Jump", (InputManager.Value1)actorController.Jump);
            gm.AddEvent("PickDrop", (InputManager.Bool1)actorController.PickDrop);
            gm.AddEvent("Skill", (InputManager.Value0)actorController.Skill);
        }
        else
        {

        }

        state = gameObject.GetComponent<StateController>();
        if (state == null)
        {
            state = gameObject.AddComponent<StateController>();
        }
        state.Init(this);

        if (skillController == null)
        {
            skillController = gameObject.AddComponent<SkillController>();
        }
        skillController.Init(actorParts);


        // SubManager初始化
        battleMng = GetComponent<SubBattleManager>();
        if (battleMng == null)
        {
            battleMng = gameObject.AddComponent<SubBattleManager>();
        }
        effectMng = GetComponent<SubEffectManager>();
        if (effectMng == null)
        {
            effectMng = gameObject.AddComponent<SubEffectManager>();
        }

        battleMng.Init();
        effectMng.Init();

        LateInit();

        // 测试代码位置
        // TODO 删除 test代码
        Test();
    }

    // 初始化 各个委托
    public void LateInit()
    {
        OnPreIK = actorController.PreIK;
        // OnBodyStateChange += effectMng.OnStateChange;   
        // if(cameraHandler){
        //     OnHandStateChange += cameraHandler.OnHandStateChange;
        //     OnBodyStateChange += cameraHandler.OnBodyStateChange;   
        // }
        // OnBodyStateChange(state.BodyAction);
        // if(OnHandStateChange!=null)
        //     OnHandStateChange(state.HandAction);
        MessageManager.Invoke<Global.BodyAction>("BodyStateChange", state.BodyAction, transform);
        MessageManager.Invoke<Global.HandAction>("HandStateChange", state.HandAction, transform);

    }

    // actorMng 转发函数
    /// <summary>
    /// 在技能释途中 根据 动作的准确来修改显示效果以及 其伤害参数等
    /// </summary>
    public void OnSkill(Vector3 skillTarPos, float bias, PropBase prop)
    {
        if (state) state.OnSkill(bias);
        if (effectMng) effectMng.OnSkill(skillTarPos, bias, prop);
    }

    public void LockOn(Transform target)
    {
        cameraHandler.lockOn(target);
    }
    public void unLock()
    {
        cameraHandler.unLock();
    }

    // stateController 转发函数



    /// // Test
    public IKTarget targetTest;

    /// <summary>
    ///  测试和代码的位置
    /// </summary>
    /// 
    public void Test()
    {
        skillController.equipSkill(actorParts.GetLearnableSkill()[0]);
        skillController.equipSkill(actorParts.GetLearnableSkill()[1]);
        skillController.equipSkill(actorParts.GetLearnableSkill()[2]);
        targetTest = GetComponentInChildren<IKTarget>();
    }

    private void EnableCols(Transform t)
    {
        Collider[] cs = t.GetComponents<Collider>();
        foreach (var c in cs)
        {
            c.enabled = true;
        }
    }





}
