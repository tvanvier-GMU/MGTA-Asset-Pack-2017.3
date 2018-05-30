using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGTA
{
    public class PlayerInputManager : MonoBehaviour
    {

        Dictionary<string, KeyCodePair> m_inputs = new Dictionary<string, KeyCodePair>();
        public List<KeyCodePair> inputs;

        [System.Serializable]
        public struct KeyCodePair
        {
            public string ID;
            public KeyCode defaultKey;
            public KeyCode alternateKey;
            public bool Pressed
            {
                get
                {
                    if ((!Input.GetKey(alternateKey) && Input.GetKeyDown(defaultKey))
                       || (!Input.GetKey(defaultKey) && Input.GetKeyDown(alternateKey)))
                    {
                        return true;
                    }
                    else return false;
                }
            }
            public bool Held
            {
                get
                {
                    return Input.GetKey(defaultKey) || Input.GetKey(alternateKey);
                }
            }

            public bool Released
            {
                get
                {
                    if ((!Input.GetKey(alternateKey) && Input.GetKeyUp(defaultKey))
                      || (!Input.GetKey(defaultKey) && Input.GetKeyUp(alternateKey)))
                    {
                        return true;
                    }
                    else return false;
                }
            }
        }

        // Use this for initialization
        void Start()
        {
            UpdateInputs();
        }

        public bool GetInputDown(string inputID)
        {
            KeyCodePair retrieval;
            if (m_inputs.TryGetValue(inputID, out retrieval))
            {
                return retrieval.Pressed;
            }
            else
            {
                Debug.LogWarning("The inuptID (" + inputID + ") that you are trying to retrieve is not present in the inputs list.");
                return false;
            }
        }

        public bool GetInput(string inputID)
        {
            KeyCodePair retrieval;
            if (m_inputs.TryGetValue(inputID, out retrieval))
            {
                return retrieval.Held;
            }
            else
            {
                Debug.LogWarning("The inuptID (" + inputID + ") that you are trying to retrieve is not present in the inputs list.");
                return false;
            }
        }

        public bool GetInputUp(string inputID)
        {
            KeyCodePair retrieval;
            if (m_inputs.TryGetValue(inputID, out retrieval))
            {
                return retrieval.Released;
            }
            else
            {
                Debug.LogWarning("The inuptID (" + inputID + ") that you are trying to retrieve is not present in the inputs list.");
                return false;
            }
        }

        public void UpdateInputs()
        {
            m_inputs.Clear();
            foreach (KeyCodePair inputPair in inputs)
            {
                m_inputs.Add(inputPair.ID, inputPair);
            }
        }

        public void OnValidate()
        {
            UpdateInputs();
        }
    }
}