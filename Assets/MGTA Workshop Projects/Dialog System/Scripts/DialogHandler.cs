using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogHandler : MonoBehaviour {
    public Text nameText;
    public Text textComponent;
    public Text continueText;
    public ChoicePanelHandler choicePanel;

    [Header("(Optional) Sync Alpha Pong")]
    public AlphaPingPong alphaPong;

    [Header("Controls")]
    public string continuationPrompt = "SPACE";

    [Header("Typewriter Speed")]
    [Range(0, .15f)]
    public float delayBetweenChars = .01f;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetData(string dialogText, string characterName, bool showChoices)
    {
        choicePanel.gameObject.SetActive(false);
        textComponent.text = "";
        nameText.text = characterName;
        choicePanel.gameObject.SetActive(false);
        this.gameObject.SetActive(true);
        StartCoroutine(AutoTypeText(textComponent, dialogText, delayBetweenChars, showChoices));
    }

    public virtual void Deactivate()
    {
        this.gameObject.SetActive(false);
    }

    protected IEnumerator AutoTypeText(UnityEngine.UI.Text target, string source, float stepTime, bool showChoices)
    {
        textComponent.text = "";
        char[] splitText = source.ToCharArray();

        if (stepTime == 0)
        {
            target.text = source;
            yield return null;
        }

        foreach (char t in splitText)
        {
            target.text += t;
            yield return new WaitForSeconds(stepTime);
        }

        if (alphaPong) alphaPong.timer = 0;
        if(!showChoices)continueText.enabled = true;
        continueText.text = continuationPrompt;
        if (showChoices)
        {
            choicePanel.ActivateChoices();
        }
        else
        {
            DialogManager.instance.awaitingKeyInput = true;
        }
    }

}
