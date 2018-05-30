using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RPGTriggerEvent : MonoBehaviour {

    public bool requirePlayerTag = true;
    public string playerTag = "Player";

    [SerializeField]
    UnityEvent OnTriggerEnterEvent;
    [SerializeField]
    UnityEvent OnTriggerExitEvent;
    [SerializeField]
    UnityEvent OnTriggerStayEvent;

    public void EnterInvoke()
    {
        OnTriggerEnterEvent.Invoke();
    }

    public void ExitInvoke()
    {
        OnTriggerExitEvent.Invoke();
    }

    public void StayInvoke()
    {
        OnTriggerStayEvent.Invoke();
    }
}
