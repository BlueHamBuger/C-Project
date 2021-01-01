using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager gameMng;
    //public ActorManager[] actorMngs;
    // public ActorManager[] controllingMngs;
    // public ActorManager[] actors;
    private InputManager inputMng;
    public UIManager uIMng;


    // 全局设定相关内容         
    public PhysicMaterial normalPM;
    public PhysicMaterial zeroPM;
    public PhysicMaterial lowPM;
    public bool pause;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (gameMng == null)
        {
            gameMng = this;
        }
        else if(gameMng!= this)
        {
            Destroy(this);
        }
        uIMng = GetComponent<UIManager>();
        inputMng = gameObject.AddComponent<InputManager>(); inputMng.Init();
        // foreach (var ctlMng in controllingMngs){
        //     ctlMng.Init();
        // }
        // foreach (var actor in actors){
        //     actor.Init();
        // }
        Global.zeroPM = zeroPM;
        Global.normalPM = normalPM;
        Global.lowPM = lowPM;
        SceneManager.sceneLoaded += initActors;
        SceneManager.sceneLoaded += uIMng.OnLoadScene;
    }
    // 开始游戏
    public void GameStart()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
        uIMng.NewGame();
        //FindObjectOfType<ActorManager>()
    }
    private void initActors(Scene s, LoadSceneMode l)
    {
        ActorManager[] am = FindObjectsOfType<ActorManager>();
        print(am.Length);
        foreach (var m in am)
        {
            m.Init();
        }
    }
    public void Welcome(){
        uIMng.GoBack();
        SceneManager.LoadScene(0, LoadSceneMode.Single);
        SceneManager.sceneLoaded -= initActors;
        SceneManager.sceneLoaded -= uIMng.OnLoadScene;
        DestroyImmediate(gameObject);
    }
    public void Resume()
    {
        pause = false;
        uIMng.ContinueGame();
    }
    public void Pause(bool p){
        pause = p;
    }

    public void AddEvent(string eventName, System.Delegate del)
    {
        inputMng.AddEvent(eventName, del);
    }
    void RemoveEvent(string eventName, System.Delegate del)
    {
        inputMng.AddEvent(eventName, del);
    }

}
