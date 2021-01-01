using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager _instance;
    private GameManager gm;
    //public InventoryUI inventoryUI;

    //是否是暂停状态
    public bool isPaused = true;
    public GameObject pauseMenu;
    public Button continueBtn;
    public Button gobackBtn;
    public GameObject mainMenu;
    public Button startBtn;
    public Button quitBtn;
    public Image backGround;
    public Canvas package;

    public GameObject[] targetGOs;

    private void Start()
    {
        _instance = this;
        //游戏开始时的的状态
        mainMenu.SetActive(true);
        //Cursor.visible = true;
        package.enabled = false;
        pauseMenu.SetActive(false);
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(pauseMenu.gameObject.transform.root.gameObject);
    }
    public void init(GameManager gm)
    {
        this.gm = gm;

    }

    private void Update()
    {
        //判断是否按下ESC键，按下的话，调出Menu菜单，并将游戏状态更改为暂停状态
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                Pause();
            }
            else
            {
                UnPause();

            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            package.enabled = !package.enabled;
            // if(package.enabled){
            //     inventoryUI.OnClick_AddItemRandom();
            // }
            Cursor.visible = !Cursor.visible;
        }
    }

    //暂停状态
    private void Pause()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        gm.Pause(true);
    }
    //非暂停状态
    private void UnPause()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gm.Pause(false);
    }


    //从暂停状态恢复到非暂停状态
    public void ContinueGame(){
        UnPause();

    }

    //重新开始游戏
    public void NewGame()
    {
        UnPause();
        mainMenu.SetActive(false);
        Cursor.visible = false;
    }

    //退出游戏
    public void QuitGame()
    {
        Application.Quit();
    }

    //游戏帮助
    public void Help()
    {

    }

    //返回主界面
    public void GoBack()
    {
        Destroy(pauseMenu.gameObject.transform.root.gameObject);
        Destroy(gameObject);
        // pauseMenu.SetActive(false);
        // mainMenu.SetActive(true);
        Cursor.visible = true;
        return;
    }

    public void OnLoadScene(Scene s, LoadSceneMode l)
    {
        if(s.buildIndex == 1){
            //backGround.enabled = false;
            StartCoroutine(bkFade());
        }else{
            backGround.enabled = true;
        }
        pauseMenu.SetActive(true);
        mainMenu.SetActive(true);

        gm = GetComponent<GameManager>();
        // Transform cg = pauseMenu.transform.Find("Menu/ContinueGame");
        // Button cgbtn = cg.GetComponent<Button>();
        // cgbtn.onClick.RemoveAllListeners();
        // cgbtn.onClick.AddListener(gm.Resume);


        // Transform sg = mainMenu.transform.Find("Menu/NewGame");
        // Button sgbtn = cg.GetComponent<Button>();
        // sgbtn.onClick.RemoveAllListeners();
        // sgbtn.onClick.AddListener(gm.GameStart);
        startBtn.onClick.AddListener(gm.GameStart);
        continueBtn.onClick.AddListener(gm.Resume);
        gobackBtn.onClick.AddListener(gm.Welcome);
        quitBtn.onClick.AddListener(QuitGame);
        
        pauseMenu.SetActive(false);
        mainMenu.SetActive(false);

    }
    private IEnumerator bkFade(){
        while(backGround.color.a<1){
            backGround.color =   new Color(backGround.color.r,backGround.color.g,backGround.color.b,Mathf.MoveTowards(backGround.color.a,1,0.01f)); 
            yield return new WaitForEndOfFrame();
        }
        backGround.enabled = false;
    }

}
