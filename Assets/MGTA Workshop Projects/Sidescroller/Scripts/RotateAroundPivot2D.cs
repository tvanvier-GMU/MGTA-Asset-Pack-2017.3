using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundPivot2D : MonoBehaviour {
    [Header("Leave blank to rotate around own pivot")]
    public Transform pivot;
    public float rotateSpeed = -90;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!pivot)
            transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
        else
            transform.RotateAround(pivot.position, Vector3.forward, rotateSpeed * Time.deltaTime);
    }
}
