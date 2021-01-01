using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 管理角色身上的 特效的播放。
/// 应该挂载在 Effect gameobject 之下
/// </summary>
public class SubEffectManager : SubManagerBase
{
    // 右手 期望轨迹
    public TrailRenderer rSlashTarTrail;
    // 当前
    PropBase prop;
    // 右手 实际轨迹
    TrailRenderer rSlashTrail;
    bool isSkilling;



    
    public SubEffectManager(ActorManager am) : base(am){}
    public override void Init(){
        rSlashTarTrail.gameObject.SetActive(false);
        MessageManager.AddListener<Global.BodyAction>("BodyStateChange",OnStateChange,transform);
        //rSlashTrail.gameObject.SetActive(false);
    }

    public void OnStateChange(Global.BodyAction newBodyAction){
        if(newBodyAction != Global.BodyAction.SKILL){
            if(isSkilling){
                isSkilling = false;
                Invoke("TrailFade",2.0f);
                StartCoroutine("slerpEmission");
            }
        }else{
            isSkilling = true;
            CancelInvoke("TrailFade");
            StopCoroutine("slerpEmission");
            prop = am.actorParts.propHandlers[0].currentProp;
            rSlashTarTrail.gameObject.SetActive(true);
        }
    }

    


    // Effects
    public void OnSkill(Vector3 skillTarPos,float bias,PropBase p){        
        prop = p;
        prop.EmissionIntensity = Mathf.MoveTowards(prop.EmissionIntensity,bias*2,0.05f);
        rSlashTarTrail.transform.position = skillTarPos;
    }

    IEnumerator slerpEmission(){
        while(prop.EmissionIntensity!=0f){
            prop.EmissionIntensity = Mathf.MoveTowards(prop.EmissionIntensity,0,0.05f);
            yield return new WaitForEndOfFrame();
        }
    }

    void TrailFade(){
        rSlashTarTrail.gameObject.SetActive(false);
    }

}
