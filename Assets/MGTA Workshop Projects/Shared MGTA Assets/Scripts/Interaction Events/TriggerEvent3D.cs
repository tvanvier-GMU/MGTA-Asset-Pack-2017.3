using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent3D : MonoBehaviour {

    public bool requirePlayerTag = true;
    public string playerTag = "Player";

    [SerializeField]
    UnityEvent OnTriggerEnterEvent;
    [SerializeField]
    UnityEvent OnTriggerExitEvent;

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log(this.gameObject.name + " Triggered Event Enter");
        OnTriggerEnterEvent.Invoke();
    }

    private void OnTriggerExit(Collider collision)
    {
        OnTriggerExitEvent.Invoke();
    }
}
