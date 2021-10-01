using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

public class DelayAction : MonoBehaviour
{
    //Example
    // DelayAction.Invoke("YourAction", DelayTime);
    // string id = DelayAction.Invoke("YourAction", DelayTime);
    // DelayAction.CancelInvoke(id);
    // DelayAction.CancelAll();

    private static DelayAction  Instance;

    [SerializeField] private List<ActionData> ActiveActions = new List<ActionData>();
    // private static DelayAction Instance
    // {
    //     get
    //     {
    //         // if (_instance == null)
    //         // {
    //         //     _instance = FindObjectOfType<DelayAction>();
    //         //
    //         //     if (_instance == null)
    //         //     {
    //         //         _instance = new GameObject("DelayAction").AddComponent<DelayAction>();
    //         //     }
    //         // }
    //         //
    //         return  _instance;
    //     }
    // }
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void OnDestroy()
    {
        CancelAll();
    }

    public static string Invoke(Action action, float delayTime = 0)
    {
        Guid guid = Guid.NewGuid();

        Coroutine actionCoroutine = Instance.StartCoroutine(Instance.InvokeAction(action, delayTime));
        
        ActionData actionData = new ActionData(guid.ToString(),delayTime, action,  actionCoroutine);
       
        Instance.ActiveActions.Add(actionData);
       
        Instance.StartCoroutine(Instance.RemoveActionCoroutine(guid.ToString(), delayTime));

        return guid.ToString();
    }
    public static void CancelInvoke(string id)
    {
        ActionData actionData  =  Instance.GetActionData(id);
        Instance.CancelInvokeAction(actionData);
    }
    public static void CancelAll()
    {
        foreach (var actionData  in Instance.ActiveActions.ToList())
       {
           try
           {
               Instance.CancelInvokeAction(actionData);
           }
           catch (Exception e)
           {
               Debug.LogError(e);
           }
       }
    }
    
    
    private IEnumerator InvokeAction(Action action, float delayTime = 0)
    {
        yield return new WaitForSecondsRealtime(delayTime);
        action.Invoke();
    }
    
    private IEnumerator RemoveActionCoroutine(string id, float delayTime = 0)
    {
        yield return new WaitForSecondsRealtime(delayTime+0.01f);
        GetActionData(id);
    }
    private ActionData GetActionData(string id)
    {
        return ActiveActions.Find(data => data.Id ==id);
    }

    private void CancelInvokeAction(ActionData actionData)
    {
        if (actionData != null)
        {
            Instance.StopCoroutine(actionData.Coroutine);
            
            if (Instance.ActiveActions.Contains(actionData))
            {
                Instance.ActiveActions.Remove(actionData);
            }
        }
    }
    
    
}

[System.Serializable]
public class ActionData
{
    public string Id;
    public float DelayTime;
    public Action Action;
    public Coroutine Coroutine;
    
    public ActionData(){}

    public ActionData(string id, float delayTime, Action action, Coroutine coroutine)
    {
        Id = id;
        Action = action;
        DelayTime = delayTime;
        Coroutine = coroutine;
    }
    
}
