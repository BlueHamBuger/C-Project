using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public delegate void Callback();
public delegate void Callback<T>(T arg1);
public delegate void Callback<T, U>(T arg1, U arg2);


// 全局的 信息管理员
// 将根据调用方法的方式来决定是进行消息的定向注册/转发还是全局
public class MessageManager : MonoBehaviour
{
    private static Dictionary<string, Delegate> eventHQTable = new Dictionary<string, Delegate>();
    static private Dictionary<string, Delegate> GetEventTable(Transform t)
    {
        if (t)
        {
            ActorManager am = t.root.GetComponent<ActorManager>();
            if(am == null) return null;
            return am.eventTable;
        }
        else
        {
            return eventHQTable;
        }
    }
    static public void AddListener<T, U>(string eventType, Callback<T, U> handler, Transform sender = null)
    {
        Dictionary<string, Delegate> eventTable = GetEventTable(sender);
        if(eventTable==null) return;
        // sender 不为空 则说明需要定向绑定消息

        // Obtain a lock on the event table to keep this thread-safe.
        lock (eventTable)
        {
            // Create an entry for this event type if it doesn't already exist.
            if (!eventTable.ContainsKey(eventType))
            {
                eventTable.Add(eventType, null);
            }
            // Add the handler to the event.
            eventTable[eventType] = (Callback<T, U>)eventTable[eventType] + handler;
        }
    }

    static public void RemoveListener<T, U>(string eventType, Callback<T, U> handler,Transform sender = null)
    {
        Dictionary<string, Delegate> eventTable = GetEventTable(sender);
        if(eventTable==null) return;
        // Obtain a lock on the event table to keep this thread-safe.
        lock (eventTable)
        {
            // Only take action if this event type exists.
            if (eventTable.ContainsKey(eventType))
            {
                // Remove the event handler from this event.
                eventTable[eventType] = (Callback<T, U>)eventTable[eventType] - handler;

                // If there's nothing left then remove the event type from the event table.
                if (eventTable[eventType] == null)
                {
                    eventTable.Remove(eventType);
                }
            }
        }
    }

    static public void Invoke<T, U>(string eventType, T arg1, U arg2,Transform sender = null)
    {
        Dictionary<string, Delegate> eventTable = GetEventTable(sender);
        if(eventTable==null) return;
        Delegate d;
        // Invoke the delegate only if the event type is in the dictionary.
        if (eventTable.TryGetValue(eventType, out d))
        {
            // Take a local copy to prevent a race condition if another thread
            // were to unsubscribe from this event.
            Callback<T, U> callback = (Callback<T, U>)d;

            // Invoke the delegate if it's not null.
            if (callback != null)
            {
                callback(arg1, arg2);
            }
        }
    }

    static public void AddListener<T>(string eventType, Callback<T> handler,Transform sender = null)
    {
        Dictionary<string, Delegate> eventTable = GetEventTable(sender);
        if(eventTable==null) return;
        // Obtain a lock on the event table to keep this thread-safe.
        lock (eventTable)
        {
            // Create an entry for this event type if it doesn't already exist.
            if (!eventTable.ContainsKey(eventType))
            {
                eventTable.Add(eventType, null);
            }
            // Add the handler to the event.
            eventTable[eventType] = (Callback<T>)eventTable[eventType] + handler;
        }
    }

    static public void RemoveListener<T>(string eventType, Callback<T> handler,Transform sender = null)
    {
        Dictionary<string, Delegate> eventTable = GetEventTable(sender);
        if(eventTable==null) return;
        // Obtain a lock on the event table to keep this thread-safe.
        lock (eventTable)
        {
            // Only take action if this event type exists.
            if (eventTable.ContainsKey(eventType))
            {
                // Remove the event handler from this event.
                eventTable[eventType] = (Callback<T>)eventTable[eventType] - handler;

                // If there's nothing left then remove the event type from the event table.
                if (eventTable[eventType] == null)
                {
                    eventTable.Remove(eventType);
                }
            }
        }
    }

    static public void Invoke<T>(string eventType, T arg1,Transform sender = null)
    {
        Dictionary<string, Delegate> eventTable = GetEventTable(sender);
        if(eventTable==null) return;
        Delegate d;
        // Invoke the delegate only if the event type is in the dictionary.
        if (eventTable.TryGetValue(eventType, out d))
        {
            // Take a local copy to prevent a race condition if another thread
            // were to unsubscribe from this event.
            Callback<T> callback = (Callback<T>)d;

            // Invoke the delegate if it's not null.
            if (callback != null)
            {
                callback(arg1);
            }
        }
    }


    static public void AddListener(string eventType, Callback handler,Transform sender = null)
    {
        Dictionary<string, Delegate> eventTable = GetEventTable(sender);
        if(eventTable==null) return;
        // Obtain a lock on the event table to keep this thread-safe.
        lock (eventTable)
        {
            // Create an entry for this event type if it doesn't already exist.
            if (!eventTable.ContainsKey(eventType))
            {
                eventTable.Add(eventType, null);
            }
            // Add the handler to the event.
            eventTable[eventType] = (Callback)eventTable[eventType] + handler;
        }
    }

    static public void RemoveListener(string eventType, Callback handler,Transform sender = null)
    {
        Dictionary<string, Delegate> eventTable = GetEventTable(sender);
        if(eventTable==null) return;
        // Obtain a lock on the event table to keep this thread-safe.
        lock (eventTable)
        {
            // Only take action if this event type exists.
            if (eventTable.ContainsKey(eventType))
            {
                // Remove the event handler from this event.
                eventTable[eventType] = (Callback)eventTable[eventType] - handler;

                // If there's nothing left then remove the event type from the event table.
                if (eventTable[eventType] == null)
                {
                    eventTable.Remove(eventType);
                }
            }
        }
    }

    static public void Invoke(string eventType,Transform sender = null)
    {
        Dictionary<string, Delegate> eventTable = GetEventTable(sender);
        if(eventTable==null) return;
        Delegate d;
        // Invoke the delegate only if the event type is in the dictionary.
        if (eventTable.TryGetValue(eventType, out d))
        {
            // Take a local copy to prevent a race condition if another thread
            // were to unsubscribe from this event.
            Callback callback = (Callback)d;

            // Invoke the delegate if it's not null.
            if (callback != null)
            {
                callback();
            }
        }
    }
}
