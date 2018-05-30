using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogStartHelper : MonoBehaviour {
    public DialogNodeCanvas dialog;
    public int targetID = 0;

    public void StartDialog()
    {
        DialogManager.instance.StartDialog(dialog, targetID);
    }
}
