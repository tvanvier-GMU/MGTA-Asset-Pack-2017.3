using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ChoicePanelHandler : MonoBehaviour {
    public List<UnityEngine.UI.Button> ChoiceButtons;
    public List<UnityEngine.UI.Text> ChoiceTexts;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ActivateChoices()
    {
        EventSystem.current.firstSelectedGameObject = ChoiceButtons[0].gameObject;
        ChoiceButtons[0].Select();
        this.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(ChoiceButtons[0].gameObject);

    }

    public void Deactivate()
    {
        this.gameObject.SetActive(false);
    }
}
