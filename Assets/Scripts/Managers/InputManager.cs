using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class InputManager : ManagerBase
{

    private const string MOVEX = "Horizontal";
    private const string MOVEY = "Vertical";

    private const string VIEWX = "Mouse X";
    private const string VIEWY = "Mouse Y";
    private const string ATTACK = "Attack";
    private const string DEFENCE = "Defence";
    private const string ROLL = "Roll";
    private const string JUMP = "Jump";
    private const string ADRANGE = "Mouse ScrollWheel";
    private const string LOCK = "Lock";
    private const string RUNORDODGE = "RunOrDodge";
    private const string PICKDROP ="PickDrop";
    private const string SKILL = "Skill";





    public delegate void Value2(Vector2 vec2);
    public delegate void Value1(float x);
    public delegate void Value0();
    public delegate void Bool1(bool b);
    public delegate void BodyActionDele(Global.BodyAction b);
    public delegate void HandActionDele(Global.HandAction h);

    //public event HandVec2 HandAction;
    public event Bool1 Attack;
    public event Bool1 Defence;

    public event Value2 Move;
    public event Value2 View;
    public event Value1 AdRange;
    public event Value1 Jump;
    public event Bool1 Run;
    public event Value0 Dodge;
    public event Value0 Roll;
    public event Bool1 PickDrop;
    public event Value0 Skill;

    public EventHandlerList eventHandler = new EventHandlerList();



    //private Dictionary<Button, string> buttonDic;
    Vector2 viewXY = new Vector2();
    Vector2 moveXY = new Vector2();




    Button visionShiftR;
    Button visionShiftL;
    Button dodgeOrRun = new Button(RUNORDODGE);
    Button roll = new Button(ROLL);
    Button attack = new Button(ATTACK);
    Button defence = new Button(DEFENCE);
    Button jump = new Button(JUMP);
    Button pickDrop = new Button(PICKDROP);
    Button skill = new Button(SKILL);

    public override void Init()
    {
        // eventHandler.AddHandler("HandAction", HandAction);
        // eventHandler.AddHandler("Run", Run);
        // eventHandler.AddHandler("Dodge", Dodge);
        // eventHandler.AddHandler("Roll", Roll);
        // eventHandler.AddHandler("View", View);
        // eventHandler.AddHandler("AdRange", AdRange);
        // eventHandler.AddHandler("Move", Move);
        // eventHandler.AddHandler("Jump",Jump);
        // eventHandler.AddHandler("PickDrop",PickDrop);
        // eventHandler.AddHandler("Skill",Skill);


        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.gameMng.pause) return;
        // 调用事件

        if (Move != null)
        {
            moveXY.Set(Input.GetAxis(MOVEX), Input.GetAxis(MOVEY));
            viewXY.Set(Input.GetAxis(VIEWX), Input.GetAxis(VIEWY));
            dodgeOrRun.Tick();
            roll.Tick();
            attack.Tick();
            defence.Tick();
            jump.Tick();
            pickDrop.Tick();
            skill.Tick();
            

            if(jump.IsPressing && Input.GetAxis(jump.axisName) == 1.0f){Jump(1.0f); jump.IsPressing = false;}
            else if(jump.IsDelaying && jump.OnReleased){ Jump(Input.GetAxis(jump.axisName));}

            if ((dodgeOrRun.IsPressing && !dodgeOrRun.IsDelaying)) { Run(true); }
            else if (dodgeOrRun.OnReleased && dodgeOrRun.IsDelaying) { Dodge(); }
            else { Run(false); }

            if(pickDrop.OnPressed){PickDrop(true);}
            else if(pickDrop.OnReleased){
                PickDrop(false);
            }

            if(skill.OnPressed){ Skill(); }


            //TODO 使用 单独的键 来控制 攻击的方式 以使得 左右键分别 控制左右手
            //Global.HandAction handAction = Global.HandAction.NONE;
            if (attack.OnPressed) { Attack(true); }
            else if(attack.OnReleased){ Attack(false); }

            if (defence.OnPressed) { Defence(true); }
            else if(defence.OnReleased){ Defence(false); }

            


            // triggers  not yet
            //if (roll.OnPressed) {Roll();}

            // 固定调用
            Move(moveXY);
            View(viewXY);
            float adrange = Input.GetAxis(ADRANGE);
            if(adrange!=0)
                AdRange(adrange);
        }

    }


    public void AddEvent(string eventName, System.Delegate del)
    {
        var eventinfo = this.GetType().GetEvent(eventName);
        eventinfo.AddEventHandler(this, del);
    }
    void RemoveEvent(string eventName, System.Delegate del)
    {
        
    }

    //内部类 Button 用于 表示 按键的状态 和绑定的 事件
    private class Button
    {
        public string axisName;
        public bool IsPressing = false;
        public bool OnPressed = false;
        public bool OnReleased = false;
        public bool IsExtending = false;
        public bool IsDelaying = false;

        public float extendingDuration = 0.15f;
        public float delayDuration = 0.20f;



        private bool curState = false;
        private bool lastState = false;
        private Timer extTimer;
        private Timer delayTimer;

        public Button(string axisName)
        {
            this.axisName = axisName;
            extTimer = new Timer(); extTimer.duration = extendingDuration;
            delayTimer = new Timer(); delayTimer.duration = extendingDuration;

        }

        public void Tick()
        {


            extTimer.Tick();
            delayTimer.Tick();

            curState = Input.GetButton(axisName);
            IsPressing = curState;

            OnPressed = false;
            OnReleased = false;
            IsExtending = false;
            IsDelaying = false;

            if (curState != lastState)
            {
                if (curState == true)
                {
                    OnPressed = true;
                    StartTimer(delayTimer, delayDuration);
                }
                else
                {
                    OnReleased = true;
                    StartTimer(extTimer, extendingDuration);
                }
            }
            lastState = curState;


            //extTimer 如果是在 run 说明 需要有一个 附加时间来判断 多次输入
            if (extTimer.state == Timer.STATE.RUN)
                IsExtending = true;
            if (delayTimer.state == Timer.STATE.RUN)
                IsDelaying = true;
        }

        private void StartTimer(Timer timer, float duration)
        {
            timer.duration = duration;
            timer.Go();
        }

        //内部类 Timer
        private class Timer
        {
            public enum STATE
            {
                IDLE, RUN, FINISHED
            }

            public STATE state;
            public float duration = 1.0f;

            private float elapsedTime = 0;
            public void Tick()
            {
                switch (state)
                {
                    case STATE.IDLE:
                        break;
                    case STATE.FINISHED:
                        break;
                    case STATE.RUN:
                        elapsedTime += Time.deltaTime;
                        if (elapsedTime >= duration)
                            state = STATE.FINISHED;
                        break;
                    default:
                        Debug.Log("Timer Error");
                        break;
                }
            }

            public void Go()
            {
                elapsedTime = 0;
                state = STATE.RUN;
            }
        }
    }




}
