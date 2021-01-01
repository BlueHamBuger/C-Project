using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 角色下自带消息中心，
// 消息中中心 将负责 转发 角色组件内部的事件
// 而不会负责外部事件的转播
// 外部事件的转播 将直接通过  上层manager

public class SubMessageManager
{
    private Dictionary<string, Delegate> _eventTable = new Dictionary<string, Delegate>();
    public Dictionary<string, Delegate> eventTable{
        get{return _eventTable;}
    } 
}
