using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGTA
{
    [RequireComponent(typeof(MeshRenderer))]
    public class ColorToggle : MonoBehaviour
    {

        public bool toggleColor = false;
        public Material materialA;
        public Material materialB;
        MeshRenderer meshRenderer;

        // Use this for initialization
        void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            if (toggleColor)
                meshRenderer.material = materialB;
            else
                meshRenderer.material = materialA;
        }

        public void Toggle()
        {
            toggleColor = !toggleColor;
        }
    }
}

