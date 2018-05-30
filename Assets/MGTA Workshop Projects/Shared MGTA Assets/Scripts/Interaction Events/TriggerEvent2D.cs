using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent2D : MonoBehaviour {

    public bool requirePlayerTag = true;
    public string playerTag = "Player";

    [SerializeField]
    UnityEvent OnTriggerEnterEvent;
    [SerializeField]
    UnityEvent OnTriggerExitEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Triggered Event Enter");
        OnTriggerEnterEvent.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnTriggerExitEvent.Invoke();
    }
}
