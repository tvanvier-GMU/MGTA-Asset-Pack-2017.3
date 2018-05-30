using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class DialogManager : MonoBehaviour {

    [Header("Options")]
    public bool m_dontDestroyOnLoad = true;
    public static DialogManager instance;

    private Canvas m_canvas;
    [Header("UI Elements")]
    public DialogHandler dialogBox;
    public PictureDialogHandler pictureDialogBox;

    public KeyCode progressionKey = KeyCode.Space;
    public bool awaitingKeyInput = false; //enabled when the typewriter is finished.
    //public bool nextSlideAvailable = false; //if a node is available, this will be marked true. Used to decide if the box should close on progression.
    //public BaseDialogNode nextNode;
    
    [Header("Loaded Dialog Canvas")]
    public DialogNodeCanvas currentNodeCanvas;
    public int currentID;
    //private Dictionary<int, DialogNodeCanvas> _dialogIdTracker = new Dictionary<int, DialogNodeCanvas>();

    private void Awake()
    {
        //enforce singleton
        if (!instance) instance = this;
        else Destroy(this);

        m_canvas = GetComponent<Canvas>();
        //if (!m_continuePrompt || !m_displayedNameLabel || !m_displayedImage || !m_displayedStandaloneText || !m_displayedImageText) Debug.LogError("UI elements must be assigned on the DialogManager!");

        if (m_dontDestroyOnLoad) DontDestroyOnLoad(this.gameObject);

        m_canvas.enabled = false;
        //ResetText();
    }

    private void Update()
    {
        if (awaitingKeyInput && Input.GetKeyDown(progressionKey))
        {
            //if (nextSlideAvailable) ProgressDialog();
            //else EndDialog();
            awaitingKeyInput = false;
            ProgressDialog();
        }
    }

    /// <summary>
    /// Begins a dialogue based on the input nodeCanvas, beginning at the DialogStartNode with the given ID.
    /// </summary>
    /// <param name="nodeCanvas"></param>
    /// <param name="dialogID"></param>
    public void StartDialog(DialogNodeCanvas nodeCanvas, int dialogID)
    {
        /*_dialogIdTracker.Clear();
        foreach (int id in nodeCanvas.GetAllDialogId())
        {
            _dialogIdTracker.Add(id, nodeCanvas);
        }*/
        currentNodeCanvas = nodeCanvas;
        currentID = dialogID;
        currentNodeCanvas.ActivateDialog(dialogID, true);
        LoadNode(currentNodeCanvas.GetDialog(dialogID));
        m_canvas.enabled = true;

    }

    /// <summary>
    /// Goes to the next entry in the dialogue.
    /// </summary>
    void ProgressDialog()
    {
        currentNodeCanvas.InputToDialog(currentID, (int)BaseDialogNode.DialogInputValue.Next);
        LoadNode(currentNodeCanvas.GetDialog(currentID));
    }

    /// <summary>
    /// Returns to the previous entry in the dialogue.
    /// </summary>
    void RegressDialog()
    {
        currentNodeCanvas.InputToDialog(currentID, (int)BaseDialogNode.DialogInputValue.Back);
        LoadNode(currentNodeCanvas.GetDialog(currentID));
    }

    /// <summary>
    /// Progresses the dialogue based on a multiple choice node.
    /// </summary>
    /// <param name="optionSelected"></param>
    public void InputChoice(int optionSelected)
    {
        currentNodeCanvas.InputToDialog(currentID, optionSelected);
        LoadNode(currentNodeCanvas.GetDialog(currentID));
    }

    /// <summary>
    /// Handles the different loading patterns based off the type of node selected.
    /// </summary>
    /// <param name="node"></param>
    void LoadNode(BaseDialogNode node)
    {
        if (node is DialogStartNode) LoadDialogStartNode((DialogStartNode)node);
        else if (node is DialogNode) LoadDialogNode((DialogNode)node);
        else if (node is DialogMultiOptionsNode) LoadChoiceNode((DialogMultiOptionsNode)node);
        else if(node == null)
        {
            EndDialog();
            Debug.Log("Dialog Ended Successfully");
        }
        else
            Debug.LogError("Invalid Node Type:");

    }

    #region LoadDialog Variants
    void LoadDialogStartNode(DialogStartNode node)
    {
        if (node.CharacterPotrait)
        {
            pictureDialogBox.SetData(node.DialogLine, node.CharacterName, node.CharacterPotrait, false);
            dialogBox.Deactivate();
        }
        else
        {
            pictureDialogBox.Deactivate();
            dialogBox.SetData(node.DialogLine, node.CharacterName, false);
        }
    }

    void LoadDialogNode(DialogNode node)
    {
        if (node.CharacterPotrait)
        {
            pictureDialogBox.SetData(node.DialogLine, node.CharacterName, node.CharacterPotrait, false);
            dialogBox.Deactivate();
        }
        else
        {
            pictureDialogBox.Deactivate();
            dialogBox.SetData(node.DialogLine, node.CharacterName, false);
        }
    }

    void LoadChoiceNode(DialogMultiOptionsNode node)
    {
        ChoicePanelHandler choicePanel;
        if (node.CharacterPotrait)
        {
            pictureDialogBox.SetData(node.DialogLine, node.CharacterName, node.CharacterPotrait, true);
            dialogBox.Deactivate();
            choicePanel = pictureDialogBox.choicePanel;
        }
        else
        {
            pictureDialogBox.Deactivate();
            dialogBox.SetData(node.DialogLine, node.CharacterName, true);
            choicePanel = dialogBox.choicePanel;
        }
        List<string> options = node.GetAllOptions();
        for (int i = 0; i < 4; i++) {
            if (i < options.Count)
            {
                //disable excess buttons if the options count is below 4
                choicePanel.ChoiceButtons[i].gameObject.SetActive(true);
                choicePanel.ChoiceTexts[i].text = options[i];
            }
            else
                choicePanel.ChoiceButtons[i].gameObject.SetActive(false);
        }
    }
    #endregion

    /// <summary>
    /// Completes the dialogue, disabling the canvas.
    /// </summary>
    void EndDialog()
    {
        m_canvas.enabled = false;
        awaitingKeyInput = false;
        currentNodeCanvas = null;
        currentID = 0;
    }
}

