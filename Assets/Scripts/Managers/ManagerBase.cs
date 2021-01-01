using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ManagerBase : MonoBehaviour
{
    protected GameManager GetGM(){
        return GameManager.gameMng;
    }

    public abstract void Init();

    // InputManager GetInputManager(){
    //     return 

    // }
}
