using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogStartTest : MonoBehaviour {
    public DialogNodeCanvas dialog;
    public int targetID = 0;
	// Use this for initialization
	void Start () {
        DialogManager.instance.StartDialog(dialog, targetID);
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
}
