using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public abstract class HittableBlock2D : MonoBehaviour {

    public bool enableDebugMessage;

    public virtual void HitFromBelow()
    {
        if (enableDebugMessage) Debug.Log(this.gameObject.name + " hit from below");
    }
}
