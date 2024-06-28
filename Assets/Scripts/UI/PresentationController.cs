using data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PresentationController : MonoBehaviour
{
    [SerializeField]
    CanvasGroup dialoguePanel;
    [SerializeField]
    DialogScreenController dialogScreenController;

    [SerializeField]
    CanvasGroup mainPanel;

    [SerializeField]
    Button answerTelephoneButton;
    [SerializeField]
    UnityEngine.UI.Image incomingCallImage;
    [SerializeField]
    float incomingCallImageDelay = 3;
    
    float dialoguePanelFadeSeconds = 1;

    bool dialoguePanelVisible;

    bool isStarted = false;

    void Start()
    {
        if (dialoguePanel)
        {
            setDialoguePanelVisible(false);
            dialoguePanel.alpha = 0; // immediately hide canvas group
        }
        Bind();
        isStarted = true;
    }

    private void OnEnable()
    {
        if (isStarted)
        {
            Bind();
        }
    }

    private void OnDisable()
    {
        Unbind();
    }

    private void Bind()
    {
        if (answerTelephoneButton != null)
        {
            answerTelephoneButton.onClick.AddListener(OnAnswerTelephoneButtonClick);
        }
        if (dialogScreenController != null)
        {
            dialogScreenController.onProceedButtonClicked += OnProceedDialogueButtonClick;
            dialogScreenController.onChoiceSelected += onChoiceSelectedCallback;
        }
    }

    private void Unbind()
    {
        if (answerTelephoneButton != null)
        {
            answerTelephoneButton.onClick.RemoveListener(OnAnswerTelephoneButtonClick);
        }
        if (dialogScreenController != null)
        {
            dialogScreenController.onProceedButtonClicked -= OnProceedDialogueButtonClick;
            dialogScreenController.onChoiceSelected -= onChoiceSelectedCallback;
        }
    }

    void Update()
    {
        if (dialoguePanel != null)
        {
            float desiredAlpha = dialoguePanelVisible ? 1.0f : 0.0f;
            if (!Mathf.Approximately(desiredAlpha, dialoguePanel.alpha))
            {
                float sign = Mathf.Sign(desiredAlpha - dialoguePanel.alpha);
                dialoguePanel.alpha = Mathf.Clamp01(dialoguePanel.alpha + sign * Time.deltaTime / dialoguePanelFadeSeconds);
            }
        }
    }

    private void setDialoguePanelVisible(bool isVisible)
    {
        if (dialoguePanel != null)
        {
            dialoguePanelVisible = isVisible;
            dialoguePanel.interactable = isVisible;
            dialoguePanel.blocksRaycasts = isVisible;
        }

        if (mainPanel != null)
        {
            mainPanel.interactable = !isVisible;
        }

        if (isVisible)
        {
            if (incomingCallImage != null)
            {
                incomingCallImage.gameObject.SetActive(false);
            }
            if (answerTelephoneButton != null)
            {
                answerTelephoneButton.enabled = false;
            }
        }
        else
        {
            StartCoroutine(VisualizeIncomingCallImageDelayed());
        }

    }

    private void OnAnswerTelephoneButtonClick()
    {
        if (!data.GameData.Instance.canExecuteCurrentDialogue())
        {
            data.GameData.Instance.extractNewDialog();            
        }

        if (data.GameData.Instance.canExecuteCurrentDialogue())
        {
            setDialoguePanelVisible(true);
            RefreshMessages();
        }
        else
        {
            Debug.LogError("No dialogue can be executed.");
        }
    }

    private void OnProceedDialogueButtonClick()
    {
        if (GameData.Instance.canExecuteCurrentDialogue())
        {
            GameData.Instance.executeCurrentDialogue();
        }

        if (GameData.Instance.canExecuteCurrentDialogue())
        {
            RefreshMessages();
        }
        else
        {
            if (dialogScreenController != null)
            {
                dialogScreenController.clearMessages();
            }
            setDialoguePanelVisible(false);
        }
    }

    private void RefreshMessages()
    {
        Dialogue currentDialogue = GameData.Instance.findDialogue(GameData.Instance.ProgressData.CurrentDialogueId);
        DialogueAction actionToExecute = currentDialogue.getActionToExecute();
        if (actionToExecute == null && currentDialogue.getChoicesMessages().Length > 0)
        {
            DialogueChoiceCase choice = currentDialogue.getChosenCase();
            if (choice == null)
            {
                dialogScreenController.setupChoices(currentDialogue.getChoicesMessages());
                return;
            }
            else
            {
                actionToExecute = choice.getActionToExecute();
            }
        }

        if (actionToExecute != null && actionToExecute.GetType().IsAssignableFrom(typeof(DialogueMessage)))
        {
            DialogueMessage messageAction = (DialogueMessage)actionToExecute;
            if (messageAction.Type == DialogueMessage.MessageType.SENT)
            {
                dialogScreenController.pushMessage(null, messageAction.Text);
            }
            else
            {
                dialogScreenController.pushMessage(currentDialogue.CharacterId, messageAction.Text);
            }
            return;
        }
    }

    private void onChoiceSelectedCallback(int i)
    {
        Dialogue currentDialogue = GameData.Instance.findDialogue(GameData.Instance.ProgressData.CurrentDialogueId);
        if (currentDialogue != null)
        {
            currentDialogue.SetChoiceSelection(i);
            OnProceedDialogueButtonClick();
        }
    }

    private IEnumerator VisualizeIncomingCallImageDelayed()
    {
        yield return new WaitForSeconds(incomingCallImageDelay);
        if (!dialoguePanelVisible)
        {
            if (incomingCallImage != null)
            {
                incomingCallImage.gameObject.SetActive(true);
            }
            if (answerTelephoneButton != null)
            {
                answerTelephoneButton.enabled = true;
            }
        }
    }

}
