using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public ActorManager am;
    public Transform handler;

    // 跟随对象的 root
    public Transform RightHandler;
    public Transform LeftHandler;
    Transform target;
    Transform tarRoot;
    Transform cameraTarPos;
    float rotateFactor = 1.0f;
    float targetRotateFactor = 1.0f;

    // Lock相关
    Transform lockTran = null;
    float anglelimit = 10f;
    Vector3 ikColDir =Vector3.zero;


    //TODO 箭头切换 inputManager化
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position,RightHandler.position,Time.deltaTime);   
            StartCoroutine("LerpCamera", RightHandler);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position,RightHandler.position,Time.deltaTime);
            StartCoroutine("LerpCamera", LeftHandler);
        }
    }
    private void Start()    
    {
        am = GetComponentInParent<ActorManager>();
        // 通常 取 spine 作为 角色根点
        target = am.actorParts.puppetRoot.GetChild(0);
        cameraTarPos = am.actorParts.rigidRoot;
        tarRoot = am.actorParts.animationRoot;
        // listeners
        MessageManager.AddListener<Vector3>("IKTargetCol",OnIKTarCol,transform);
        MessageManager.AddListener<Global.HandAction>("HandStateChange",OnHandStateChange,transform);
        MessageManager.AddListener<Global.BodyAction>("BodyStateChange",OnBodyStateChange,transform);
    }
    

    public void View(Vector2 viewXY)
    {

        float mousex = viewXY.x;
        float mousey = viewXY.y;
        rotateFactor = Mathf.Lerp(rotateFactor, targetRotateFactor, Time.deltaTime * 2.0f);

        // TEST
        // 将 限制 玩家的攻击被挡住时候 的 视角连同角色的移动
        if (ikColDir != Vector3.zero)
        {
            Vector3 coldir = -am.targetTest.CollisionDir;
            Vector3 handDir = am.targetTest.transform.position - am.actorParts.rigidRoot.position;
            Quaternion quaTemp = Quaternion.FromToRotation(handDir, am.actorParts.rigidRoot.forward);
            coldir = am.actorParts.rigidRoot.InverseTransformDirection(quaTemp * coldir); ;
            float f = mousex * coldir.x + (-mousey) * coldir.y;
            if (f > 0)
            {
                mousex -= f * coldir.x;
                mousey -= f * coldir.y;
            }
        }
        handler.localEulerAngles = new Vector3(handler.localEulerAngles.x - mousey * rotateFactor, 0, 0);
        if (lockTran)
        {
            // 只比较水平角度
            Vector3 lockDir = Vector3.ProjectOnPlane(lockTran.position - cameraTarPos.position, Vector3.up);
            float angle = Vector3.SignedAngle(lockDir, cameraTarPos.forward, Vector3.up);
            Quaternion qua = Quaternion.AngleAxis(-anglelimit, Vector3.up);
            Debug.DrawRay(cameraTarPos.position, lockDir * 20);
            Debug.DrawRay(cameraTarPos.position, cameraTarPos.forward * 20);
            Debug.DrawRay(cameraTarPos.position, qua * lockDir * 20);
            Quaternion qua1 = Quaternion.AngleAxis(anglelimit, Vector3.up);
            Debug.DrawRay(cameraTarPos.position, qua1 * lockDir * 20);
            if (Mathf.Abs(angle) >= anglelimit)
            {
                if (angle < 0)
                {
                    if (mousex < 0)
                    {
                        mousex = 0;
                    }
                    else
                    {
                        float angleDelta = Vector3.SignedAngle(cameraTarPos.forward, qua * lockDir, Vector3.up);
                        cameraTarPos.Rotate(Vector3.up, angleDelta / 10);
                        handler.parent.forward = cameraTarPos.forward;
                        am.actorController.rigid.MoveRotation(Quaternion.AngleAxis(angleDelta / 10, Vector3.up) * am.actorController.rigid.rotation);

                    }
                }
                else
                {
                    if (mousex > 0)
                    {
                        mousex = 0;
                    }
                    else
                    {
                        float angleDelta = Vector3.SignedAngle(cameraTarPos.forward, qua1 * lockDir, Vector3.up);
                        cameraTarPos.Rotate(Vector3.up, angleDelta / 10);
                        handler.parent.forward = cameraTarPos.forward;
                        am.actorController.rigid.MoveRotation(Quaternion.AngleAxis(angleDelta / 10, Vector3.up) * am.actorController.rigid.rotation);
                    }
                }

            }
        }
        handler.parent.Rotate(0, mousex * rotateFactor, 0, Space.Self);
        cameraTarPos.Rotate(0, mousex * rotateFactor, 0, Space.Self);
        if (am.state.actionState.bodyAction != Global.BodyAction.FALLING)
        {
            Quaternion quat = Quaternion.Euler(0, mousex * rotateFactor, 0);
            am.actorController.rigid.MoveRotation(quat * am.actorController.rigid.rotation); // 设置玩家的旋转
        }


        // 处理欧拉角边界
        if (handler.localEulerAngles.x > 45 && handler.localEulerAngles.x < 180)
        {
            handler.localEulerAngles = new Vector3(45f, handler.localEulerAngles.y, handler.localEulerAngles.z);
        }
        else if (handler.localEulerAngles.x < 295 && handler.localEulerAngles.x > 180)
        {
            handler.localEulerAngles = new Vector3(295f, handler.localEulerAngles.y, handler.localEulerAngles.z);
        }

        // target 是 跟踪对象的 动作量最少的 spine 节点
        // TODO 将 Vector3.up 换成 头部到spine 的 垂直距离
        cameraTarPos.position = target.position + Vector3.up;
        handler.parent.position = Vector3.Lerp(handler.position, target.position + Vector3.up, Time.deltaTime * 5);

    }


    IEnumerator LerpCamera(Transform tarTran)
    {
        float time = 0.0f;

        while (time < 1.0f)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, tarTran.position, time);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    public void lockOn(Transform tar)
    {
        lockTran = tar;
    }
    public void unLock()
    {
        lockTran = null;
    }


    private void HandActionSwitch()
    {
        switch (am.state.HandAction)
        {
            case Global.HandAction.SLASH:
                targetRotateFactor = 0.7f;
                break;
            default:
                break;
        }
    }



    public void OnBodyStateChange(Global.BodyAction newState)
    {
        switch (newState)
        {
            case Global.BodyAction.SKILL:
                targetRotateFactor = 0.3f;
                break;
            case Global.BodyAction.FALLING:
                targetRotateFactor = 1f;
                break;
            default:
                targetRotateFactor = 5.0f;
                HandActionSwitch();
                break;
        }
    }
    public void OnHandStateChange(Global.HandAction newState)
    {
        switch (newState)
        {
            case Global.HandAction.SLASH:
                targetRotateFactor = 0.7f;
                break;
            case Global.HandAction.FETCH:
                targetRotateFactor = 1f;
                anglelimit = 10;
                break;
            default:
                OnBodyStateChange(am.state.BodyAction);
                break;
        }
    }

    private void OnIKTarCol(Vector3 dir){
        ikColDir = dir;
    }






}
