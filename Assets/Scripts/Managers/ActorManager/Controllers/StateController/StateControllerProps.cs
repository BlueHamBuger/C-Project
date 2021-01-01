using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion;
using RootMotion.Dynamics;


public partial class StateController : MonoBehaviour
{

    private float maxHP = 100.0f;
    private float maxSP = 100.0f;
    [Range(0, 100)] private int level = 0;
    private int neededExp = 0;

    // 当前属性值
    // [SerializeField,SetProperty("CurHP")]
    private float curHP;
    private float curSP;
    private int curExp;


    public float CurHP
    {
        get { return curHP; }
    }
    public float CurSP
    {
        get { return curSP; }
    }
    public float Level
    {
        get { return level; }
    }
    public int CurExp
    {
        get { return curExp; }
        set
        {
            curExp = value;
            if (curExp >= neededExp)
            {
                LevelUp();
                curExp = 0;
            }
        }
    }

    public delegate void ActorPropDelegate(PropsBase ap);
    public delegate void UpdateEntry(ActorPropDelegate apd, bool add);
    private ActorPropDelegate updateProps;
    private UpdateEntry updateEntry;
    public PropsBase actorProp; // actorProp 是 角色的整体属性，应该只和角色的等级  种族等挂钩

    // 和 puppet 关联的值
    // baseProp 应该是 人物的 各个肢体的属性
    [SerializeField]
    private GroupProp[] baseProps;

    // 为 人物肢体外的 muscle group 的属性 比如武器
    [SerializeField]
    private GroupProp[] extraProps = null;

    //private Dictionary<>



    private void InitProps()
    {
        actorProp = new PropsBase(1, 1, 1f, 1f,1f);
        UpdateAllMuscles();
        updateProps(actorProp);
        UpdateStateProps();
    }

    // 属性的更新直接交给 委托
    // 一旦对应的位置的 属性得到更新 将会在函数最末将 对应的 函数解除委托
    public void UpdateProps()
    {
        if (updateProps != null)
        {
            updateProps(actorProp);
        }
    }



    private void UpdateAllMuscles()
    {
        foreach (var bp in baseProps)
        {
            bp.UpdateAllProp();
        }
    }

    public void UpdateStateProps()
    {
        Global.MuscleGroupStateProps mgs = am.actorParts.GetStateProp(actionState);
        foreach (var groupProp in baseProps)
        {
            PropsBase pb = mgs.GetGroupProp(groupProp.MscleGroup);
            if (pb != null)
                groupProp.stateProp.SetValue(pb);
        }
        // StopAllCoroutines();
        // StartCoroutine(PropSetCoroutine(1f, mgs));
    }

    // 携程方式 逐渐过渡式地改变 肌肉属性
        // TODO 测试 是否可去
    private IEnumerator PropSetCoroutine(float time, Global.MuscleGroupStateProps mgs)
    {
        float curF = 0;
        PropsBase pbtemp = new PropsBase(0, 0, 0, 0,0);
        while (curF < 1.0)
        {
            foreach (var groupProp in baseProps)
            {
                PropsBase pb = mgs.GetGroupProp(groupProp.MscleGroup);
                if (pb != null)
                {
                    pbtemp.Accuracy = Mathf.Lerp(groupProp.stateProp.Accuracy, pb.Accuracy, curF);
                    pbtemp.Agility = Mathf.Lerp(groupProp.stateProp.Agility, pb.Agility, curF);
                    pbtemp.Power = Mathf.Lerp(groupProp.stateProp.Power, pb.Power, curF);
                    pbtemp.Toughness = Mathf.Lerp(groupProp.stateProp.Accuracy, pb.Toughness, curF);

                    groupProp.stateProp.SetValue(pbtemp);

                }
            }
            curF += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        foreach (var groupProp in baseProps)
        {
            PropsBase pb = mgs.GetGroupProp(groupProp.MscleGroup);
            if (pb != null)
            {
                pbtemp.Accuracy = Mathf.Lerp(groupProp.stateProp.Accuracy, pb.Accuracy, 1.0f);
                pbtemp.Agility = Mathf.Lerp(groupProp.stateProp.Agility, pb.Agility, 1.0f);
                pbtemp.Power = Mathf.Lerp(groupProp.stateProp.Power, pb.Power, 1.0f);
                pbtemp.Toughness = Mathf.Lerp(groupProp.stateProp.Accuracy, pb.Toughness, 1.0f);
                groupProp.stateProp.SetValue(pbtemp);

            }
        }




    }

    // 玩家数值改变

    private void LevelUp()
    {
        level++;
        //TODO 上升属性
    }

    public void TryDoDamage(float damage){
        float resultHP = curHP - damage;
        if(resultHP <= 0){
            
        }
    }

    // 对外函数
    public void OnSkill(float bias){
        
    }












    // 肌肉属性相关
    public GroupProp GetExtraProps(int index)
    {
        return extraProps[index];
    }
    public GroupProp GetExtraProps(string s)
    {
        return GetPropsByString(extraProps, s);
    }


    public GroupProp GetBaseProps(int index)
    {
        return baseProps[index];
    }
    public GroupProp GetBaseProps(string s)
    {
        return GetPropsByString(baseProps, s);
    }
    public GroupProp GetBaseProps(ActorParts.MuscleGroup muscleGroup)
    {
        return baseProps[muscleGroup.GroupIndex];
    }



    private GroupProp GetPropsByString(GroupProp[] groupProps, string groupName)
    {
        foreach (var group in groupProps)
        {
            if (group.name.Equals(groupName))
            {
                return group;
            }
        }
        return null;
    }


    //将 所有的 肌肉系数 设置到 一个 共同值
    public void setAllStateProp(float? power, float? toughness, float? accuracy, float? agility)
    {
        foreach (var prop in baseProps)
        {
            if (accuracy.HasValue) prop.stateProp.Accuracy = accuracy.Value;
            if (agility.HasValue) prop.stateProp.Agility = agility.Value;
            if (toughness.HasValue) prop.stateProp.Toughness = toughness.Value;
            if (power.HasValue) prop.stateProp.Power = power.Value;
        }
    }

    /// <summary>
    ///  作为 muscleProp 的 delegate 的入口，来更新 stateController 的数值
    /// </summary>
    /// <param name="apd"></param>
    /// <param name="add"></param>
    private void UpdateDelegate(ActorPropDelegate apd, bool add)
    {
        if (add)
        {
            if (updateProps == null) { updateProps = apd; return; }
            foreach (ActorPropDelegate adp1 in updateProps.GetInvocationList())
            {
                if (adp1 == apd)
                {
                    return;
                }
            }
            updateProps += apd;
        }
        else
        {
            updateProps -= apd;
        }
    }



    public class PropsBase
    {
        public float power;
        public float toughness;
        public float accuracy;
        public float agility;
        public float tolerance;
        public PropsBase(float power, float toughness, float accuracy, float agility,float tolerance)
        {
            this.power = power; this.toughness = toughness; this.accuracy = accuracy; this.agility = agility;this.tolerance = tolerance;
        }

        // 本肌肉的攻击力
        public virtual float Power 
        {
            get { return power; }
            set { power = value; }
        }
        // 本肌肉的强韧度
        public virtual float Toughness
        {
            get { return toughness; }
            set { toughness = value; }
        }
        // 肌肉的准确率
        public virtual float Accuracy
        {
            get { return accuracy; }
            set { accuracy = value; }
        }
        // 肌肉的 精准度
        public virtual float Agility
        {
            get { return agility; }
            set { agility = value; }
        }
        // 肌肉的 忍耐力 表示 受到多大的冲击 才会使得身体失衡
        public virtual float Tolerance{
            get{return tolerance;}
            set{tolerance = value;}
        }

        public void SetValue(PropsBase pb2)
        {
            Accuracy = pb2.Accuracy;
            Agility = pb2.Agility;
            Power = pb2.Power;
            Toughness = pb2.Toughness;
            Tolerance = pb2.Tolerance;
        }
    }


    [System.Serializable]
    public class GroupProp
    {
        // private static StateController sc;

        [HideInInspector]
        public string name;


        // 此 属性组对应的 肌肉组
        protected ActorParts.MuscleGroup muscleGroup;

        public ActorParts.MuscleGroup MscleGroup
        {
            get
            {
                return muscleGroup;
            }
        }

        // 控制底层的 给与 behaviour 控制的 肌肉数值（底层数据的控制）
        // 和 muscleGroup 一一对应
        public ConsciousBehaviour.MuscleBasePropsGroup muscleBasePropsGroup;

        //数值
        public VisibleProp equipProp;
        public VisibleProp buffProp;
        public UnVisibleProp stateProp;// 是不可见的 状态属性
        public PropsBase curProp; // 是对外显示的 可见属性的综合

        // 如果对应的 muscle 是有 propHandler 的则要求 在 此 属性更新的时候 连带更新 相关联的muscle 属性
        protected UpdateEntry updateEntry;



        public GroupProp(ActorParts.MuscleGroup muscleGroup, UpdateEntry v)
        {
            curProp = new PropsBase(0, 0, 0, 0, 0);
            buffProp = new VisibleProp(this, 0, 0, 0, 0,0);
            equipProp = new VisibleProp(this, 0, 0, 0, 0,0);
            //print(muscleBasePropsGroup.props.knockOutDistance);
            stateProp = new UnVisibleProp(this, 1, 1, 1, 1,0.1f);

            this.muscleGroup = muscleGroup;
            Muscle m = muscleGroup.muscles[0];
            name = muscleGroup.groupName;
            this.updateEntry = v;
        }

        protected void UpdateVisibleAgility()
        {
            curProp.Agility = equipProp.Agility + buffProp.Agility;
            updateEntry(UpdateAgility, true);
        }
        protected void UpdateVisiblePower()
        {
            curProp.Power = equipProp.Power + buffProp.Power;
            updateEntry(UpdatePower, true);

        }
        protected void UpdateVisibleToughness()
        {
            curProp.Toughness = equipProp.Toughness + buffProp.Toughness;
            updateEntry(UpdateToughness, true);
        }
        protected void UpdateVisibleAccuracy()
        {
            curProp.Accuracy = equipProp.Accuracy + buffProp.Accuracy;
            updateEntry(UpdateAccuracy, true);
        }
        protected void UpdateVisibleTolerance(){
            curProp.Tolerance = equipProp.Tolerance + buffProp.Tolerance;
            updateEntry(UpdateTolerance,true);
        }


        public void UpdateAllProp()
        {
            UpdateVisibleAccuracy();
            UpdateVisibleAgility();
            UpdateVisiblePower();
            UpdateVisibleToughness();
            UpdateVisibleTolerance();
        }


        // 不可见数值更新 现阶段特指StateProp
            //Update** 中的参数 是 ActorProp 即 代表角色本身 的 整体属性
        protected void UpdateAgility(PropsBase ap)
        {
            float pinWeight = (ap.Agility + (ap.Agility * curProp.Agility)) * stateProp.Agility;
            foreach (Muscle m in muscleGroup.muscles) { m.props.pinWeight = pinWeight; }
            // PropHandler ph = muscleGroup.propHandler;
            // if (ph && ph.currentProp) for (int i = 0; i < ph.muscles.Length; i++) { ph.muscles[i].props.pinWeight = pinWeight * ph.currentProp.muscleWeights[i]; }
            updateEntry(UpdateAgility, false);
        }
        protected void UpdatePower(PropsBase ap)
        {
            float impulseMlp = (ap.Power + (ap.Power * curProp.Power)) * stateProp.Power;
            foreach (Muscle m in muscleGroup.muscles) { m.state.impulseMlp = impulseMlp; }
            // PropHandler ph = muscleGroup.propHandler;
            // if (ph && ph.currentProp) for (int i = 0; i < ph.muscles.Length; i++) { ph.muscles[i].state.impulseMlp = impulseMlp * ph.currentProp.muscleWeights[i]; }
            updateEntry(UpdatePower, false);
        }
        protected void UpdateToughness(PropsBase ap)
        {
            float immunity = (ap.Toughness + (ap.Toughness * curProp.Toughness)) * stateProp.Toughness;
            foreach (Muscle m in muscleGroup.muscles) { m.state.immunity = immunity; }
            // PropHandler ph = muscleGroup.propHandler;
            // if (ph && ph.currentProp) for (int i = 0; i < ph.muscles.Length; i++) { ph.muscles[i].state.immunity = immunity * ph.currentProp.muscleWeights[i]; }
            updateEntry(UpdateToughness, false);
        }
        protected void UpdateAccuracy(PropsBase ap)
        {
            float muscleWeight = (ap.Accuracy + (ap.Accuracy * curProp.Accuracy)) * stateProp.Accuracy;
            foreach (Muscle m in muscleGroup.muscles) { m.props.muscleWeight = muscleWeight; }
            // PropHandler ph = muscleGroup.propHandler;
            // if (ph && ph.currentProp) for(int i = 0; i < ph.muscles.Length; i++) { ph.muscles[i].props.muscleWeight = muscleWeight * ph.currentProp.muscleWeights[i]; }
            updateEntry(UpdateAccuracy, false);
        }
        public void UpdateTolerance(PropsBase ap){
            float knockOutDist = (ap.Tolerance+(ap.Tolerance*curProp.Tolerance)) * stateProp.Tolerance;
            muscleBasePropsGroup.props.knockOutDistance = knockOutDist;

            updateEntry(UpdateTolerance,false);
        }





        public class MuscleGroupProp : PropsBase
        {
            protected GroupProp groupProp;
            public MuscleGroupProp(GroupProp g, float power, float toughness, float accuracy, float agility,float tolerance) : base(power, toughness, accuracy, agility,tolerance) { groupProp = g; }
        }

        public class VisibleProp : MuscleGroupProp
        {
            public VisibleProp(GroupProp g, float power, float toughness, float accuracy, float agility,float tolerance) : base(g, power, toughness, accuracy, agility,tolerance)
            {
            }

            public override float Power { get => base.Power; set { if (power == value) return; power = value; groupProp.UpdateVisiblePower(); } }
            public override float Toughness { get => base.Toughness; set { if (toughness == value) return; toughness = value; groupProp.UpdateVisibleToughness(); } }
            public override float Accuracy { get => base.Accuracy; set { if (accuracy == value) return; accuracy = value; groupProp.UpdateVisibleAccuracy(); } }
            public override float Agility { get => base.Agility; set { if (agility == value) return; agility = value; groupProp.UpdateVisibleAgility(); } }

            public override float Tolerance { get => base.Tolerance; set { if (tolerance == value) return; tolerance = value; groupProp.UpdateVisibleTolerance(); } }

        }


        //stateProp 指代的 是 不同的动作对于 玩家数值的影响
        // 这不应该直接显示在 muscle 的状态版中 因为这是 和动作直接绑定的 附加值
        [System.Serializable]
        public class UnVisibleProp : MuscleGroupProp
        {
            public UnVisibleProp(GroupProp g, float power, float toughness, float accuracy, float agility,float tolerance) : base(g, power, toughness, accuracy, agility,tolerance)
            {}

            public override float Power { get => base.Power; set { if (power == value) return; power = value; groupProp.updateEntry(groupProp.UpdatePower, true); } }
            public override float Toughness { get => base.Toughness; set { if (toughness == value) return; toughness = value; groupProp.updateEntry(groupProp.UpdateToughness, true); } }
            public override float Accuracy { get => base.Accuracy; set { if (accuracy == value) return; accuracy = value; groupProp.updateEntry(groupProp.UpdateAccuracy, true); } }
            public override float Agility { get => base.Agility; set { if (agility == value) return; agility = value; groupProp.updateEntry(groupProp.UpdateAgility, true); } }
            public override float Tolerance { get => base.Tolerance; set { if (tolerance == value) return; tolerance = value; groupProp.updateEntry(groupProp.UpdateTolerance,true); } }
        }
    }


    public class PropGroupProp : GroupProp
    {
        public PropGroupProp(ActorParts.MuscleGroup muscleGroup, UpdateEntry v) : base(muscleGroup, v) { }
    }

}
