using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MGTA
{
    [AddComponentMenu("MGTA/Characters/FirstPerson/FirstPersonInteractionHandler")]
    public class InteractableObject : MonoBehaviour
    {

        public bool interactionEnabled = true;

        [SerializeField]
        UnityEvent OnInteractEvent;

        public void Interact()
        {
            OnInteractEvent.Invoke();
        }

    }
}
