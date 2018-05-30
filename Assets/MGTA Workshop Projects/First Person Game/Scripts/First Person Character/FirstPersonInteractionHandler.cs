using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGTA
{
    [AddComponentMenu("MGTA/Characters/FirstPerson/FirstPersonInteractionHandler")]
    public class FirstPersonInteractionHandler : MonoBehaviour
    {
        public float range = 1;
        public string interactableObjectTag = "Interactable";
        public LayerMask validLayers;
        RaycastHit hitInfo;
        public QueryTriggerInteraction checkAgainstTriggers = QueryTriggerInteraction.Ignore;
        public Camera playerCamera;
        public InteractableObject selectedInteractable;
        public PlayerInputManager inputManager;
        public bool debugRays = true;

        public Canvas interactionUI;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hitInfo, Mathf.Infinity, validLayers, checkAgainstTriggers))
            {
                if (hitInfo.collider && hitInfo.collider.CompareTag(interactableObjectTag))
                {
                    if (Vector3.Distance(hitInfo.collider.transform.position, transform.position) <= range)
                        selectedInteractable = hitInfo.collider.GetComponent<InteractableObject>();
                    else selectedInteractable = null;
                }
                else selectedInteractable = null;
            }
            else
            {
                selectedInteractable = null;
            }

            if (selectedInteractable && selectedInteractable.interactionEnabled)
            {
                if (interactionUI) interactionUI.gameObject.SetActive(true);

                if (inputManager.GetInputDown("Interact"))
                {
                    selectedInteractable.Interact();
                }
            }
            else
            {
                if (interactionUI) interactionUI.gameObject.SetActive(false);
            }
        }

        private void OnDrawGizmos()
        {
            if (debugRays)
            {
                Gizmos.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * range);
            }
        }
    }
}