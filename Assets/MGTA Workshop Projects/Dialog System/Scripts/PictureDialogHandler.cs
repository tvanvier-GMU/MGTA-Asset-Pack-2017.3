using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PictureDialogHandler : DialogHandler {
    [Header("Image")]
    public Image image;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetData(string dialogText, string characterName, Sprite sprite, bool showChoices)
    {
        textComponent.text = "";
        choicePanel.gameObject.SetActive(false);
        nameText.text = characterName;
        image.sprite = sprite;
        this.gameObject.SetActive(true);
        StartCoroutine(AutoTypeText(textComponent, dialogText, delayBetweenChars, showChoices));
    }
}
