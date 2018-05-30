using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StompableEnemy2D : MonoBehaviour {

    public bool enableDebugMessage;

    public virtual void HitFromAbove()
    {
        if (enableDebugMessage) Debug.Log(this.gameObject.name + " hit from above");
    }
}
