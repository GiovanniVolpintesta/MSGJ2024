using data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PresentationController : MonoBehaviour
{
    private enum UIType { DESK, DIALOGUE }

    [SerializeField]
    CanvasGroup dialoguePanel;
    [SerializeField]
    DialogScreenController dialogueScreenController;

    [SerializeField]
    CanvasGroup mainPanel;

    [SerializeField]
    Button answerTelephoneButton;
    [SerializeField]
    float incomingCallImageDelay = 3;
    [SerializeField]
    Animator phoneAnimator;
    [SerializeField]
    string startRingingEvent = "startRinging";
    [SerializeField]
    string stopRingingEvent = "stopRinging";

    [SerializeField]
    string creditsSceneName;

    float dialoguePanelFadeSeconds = 1;

    bool dialoguePanelVisible;

    bool isStarted = false;

    void Start()
    {
        refreshUI(UIType.DESK);

        if (dialoguePanel)
        {
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
        if (dialogueScreenController != null)
        {
            dialogueScreenController.onProceedButtonClicked += OnProceedDialogueButtonClick;
            dialogueScreenController.onChoiceSelected += onChoiceSelectedCallback;
        }
    }

    private void Unbind()
    {
        if (answerTelephoneButton != null)
        {
            answerTelephoneButton.onClick.RemoveListener(OnAnswerTelephoneButtonClick);
        }
        if (dialogueScreenController != null)
        {
            dialogueScreenController.onProceedButtonClicked -= OnProceedDialogueButtonClick;
            dialogueScreenController.onChoiceSelected -= onChoiceSelectedCallback;
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

    private void refreshUI(UIType type)
    {
        if (dialoguePanel != null)
        {
            dialoguePanelVisible = (type == UIType.DIALOGUE);
            dialoguePanel.interactable = (type == UIType.DIALOGUE);
            dialoguePanel.blocksRaycasts = (type == UIType.DIALOGUE);
        }

        if (mainPanel != null)
        {
            mainPanel.interactable = (type == UIType.DESK);
        }

        if (type == UIType.DIALOGUE)
        {
            if (phoneAnimator != null)
            {
                phoneAnimator.ResetTrigger(startRingingEvent);
                phoneAnimator.SetTrigger(stopRingingEvent);
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
        Dialogue currentDialogue = GameData.Instance.getCurrentDialogue();
        if (currentDialogue != null && currentDialogue.canExecute() && !currentDialogue.isStarted())
        {
            currentDialogue.execute();
        }

        if (dialogueScreenController != null)
        {
            dialogueScreenController.SetupDialogue(data.GameData.Instance.getCurrentDialogue());
        }
        refreshUI(UIType.DIALOGUE);
    }

    private void OnProceedDialogueButtonClick()
    {
        if (dialogueScreenController != null && dialogueScreenController.getDialogue() != data.GameData.Instance.getCurrentDialogue())
        {
            Debug.LogWarning("Dialogue inconsisistency: current dialogue is "+ data.GameData.Instance.getCurrentDialogue().Id + " while the visualized dialogue is "+ dialogueScreenController.getDialogue() .Id + ". The current dialogue will replace the visualized one");
            dialogueScreenController.SetupDialogue(data.GameData.Instance.getCurrentDialogue());
        }

        Dialogue currentDialogue = GameData.Instance.getCurrentDialogue();
        if (currentDialogue != null && currentDialogue.canExecute())
        {
            currentDialogue.execute();

            if (dialogueScreenController != null)
            {
                dialogueScreenController.Refresh();
            }
        }
        else
        {
            if (dialogueScreenController != null)
            {
                dialogueScreenController.Clear();
            }
            refreshUI(UIType.DESK);

            currentDialogue = GameData.Instance.extractNewDialog();
            if (currentDialogue == null || !currentDialogue.canExecute())
            {
                SceneManager.LoadScene(creditsSceneName);
            }
        }
    }

    private void onChoiceSelectedCallback(int i)
    {
        Dialogue currentDialogue = data.GameData.Instance.getCurrentDialogue();
        if (dialogueScreenController != null && dialogueScreenController.getDialogue() != currentDialogue)
        {
            Debug.LogWarning("Dialogue inconsisistency: current dialogue is " + data.GameData.Instance.getCurrentDialogue().Id + " while the visualized dialogue is " + dialogueScreenController.getDialogue().Id + ". The choice selection will be ignored.");
        }

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
            if (phoneAnimator != null)
            {
                phoneAnimator.ResetTrigger(stopRingingEvent);
                phoneAnimator.SetTrigger(startRingingEvent);
            }
            if (answerTelephoneButton != null)
            {
                answerTelephoneButton.enabled = true;
            }
        }
    }

}
