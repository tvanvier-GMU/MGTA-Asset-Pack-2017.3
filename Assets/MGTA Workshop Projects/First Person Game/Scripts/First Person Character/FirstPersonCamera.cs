using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGTA
{
    [AddComponentMenu("MGTA/Characters/FirstPerson/FirstPersonCamera")]
    public class FirstPersonCamera : MonoBehaviour
    {
        [SerializeField]
        Vector2 mouseVector;

        [Header("Camera References")]
        public Camera cam;
        public Transform target;
        public bool lockCursor;
        public float tiltSpeed = 1f;
        public float minTilt = -45;
        public float maxTilt = 80;
        float rotationY = 0F;


        public float rotateSpeed = 1f;
        float rotationX = 0F;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !lockCursor;

            mouseVector.y = Input.GetAxis("Mouse Y");
            rotationY += mouseVector.y * tiltSpeed;
            float mouseInputX = Input.GetAxis("Mouse X");
            rotationY = Mathf.Clamp(rotationY, minTilt, maxTilt);
            rotationX += mouseInputX * rotateSpeed;
            rotationX %= 360;

            cam.transform.localEulerAngles = new Vector3(-rotationY, 0, 0);

            //target.localEulerAngles = new Vector3(0, rotationX, 0);
            target.Rotate(0, mouseInputX * rotateSpeed, 0);


            /* mouseVector.y = Input.GetAxis("Mouse Y");
             mouseVector.x = Input.GetAxis("Mouse X");

             cam.transform.Rotate(-mouseVector.y, 0, 0);
             float verticalRotation = cam.transform.eulerAngles.x;
             verticalRotation = Mathf.Clamp(verticalRotation, minTilt, maxTilt);
             cam.transform.localEulerAngles = new Vector3(verticalRotation, cam.transform.eulerAngles.y, cam.transform.eulerAngles.z);
             target.transform.Rotate(0, mouseVector.x, 0);
             */
        }

        void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}