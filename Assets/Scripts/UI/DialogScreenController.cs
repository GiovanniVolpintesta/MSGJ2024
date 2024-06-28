using data;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogScreenController : MonoBehaviour
{
    private enum InteractionType { PROCEED, SELECTION }

    public delegate void OnProceed();

    public event OnChoiceSelected onChoiceSelected;
    public event OnProceed onProceedButtonClicked;

    [SerializeField]
    GameObject messagesContainer;
    [SerializeField]
    ScrollRect scroll;
    [SerializeField]
    GameObject messageGameObject;

    [SerializeField]
    GameObject choicesContainer;
    [SerializeField]
    GameObject choicesGameObject;

    [SerializeField]
    Button proceedDialogueButton;
    [SerializeField]
    float proceedDialogueDelay = 2;

    List<GameObject> choiceControllers = new List<GameObject>();
    bool isStarted = false;

    Dialogue dialogue = null;
    public Dialogue getDialogue() { return dialogue; }

    List<DialogueMessage> visualizedMessages = new List<DialogueMessage>();

    public void Clear()
    {
        if (messagesContainer != null)
        {
            GameObject[] children = new GameObject[messagesContainer.transform.childCount];
            for (int i = 0; i < children.Length; i++) children[i] = messagesContainer.transform.GetChild(i).gameObject;
            messagesContainer.transform.DetachChildren();
            foreach (GameObject child in children)
                if (child != null && !child.IsDestroyed())
                    Destroy(child.gameObject);
        }
        visualizedMessages.Clear();
    }

    public void SetupDialogue (Dialogue dialogue)
    {
        Clear();
        this.dialogue = dialogue;
        Refresh();
    }

    public void Refresh()
    {
        if (this.dialogue != null)
        {
            enqueueNewMessages(this.dialogue.DialogueActions);

            DialogueChoiceCase choiceSelection = this.dialogue.getChosenCase();
            if (choiceSelection != null)
            {
                enqueueNewMessages(choiceSelection.DialogueActions);
            }

            string[] choiceMsgs = { };
            InteractionType interactionType = InteractionType.PROCEED;
            if (!this.dialogue.isEnded() // if the dialogue is not ended,
                && this.dialogue.areDialogueActionsEnded() // but the main dialogue sequence is ended,
                && choiceSelection == null) // and a choice has not been selected yet
            {
                // (if there is no choice the dialogue would end when the main sequence is ended)
                choiceMsgs = this.dialogue.getChoicesMessages();
                interactionType = InteractionType.SELECTION;
            }

            setupChoices(choiceMsgs);
            showInterationUI(interactionType);
        }            
    }

    private void showInterationUI(InteractionType interationType)
    {
        if (proceedDialogueButton != null && proceedDialogueButton.gameObject != null)
        {
            proceedDialogueButton.gameObject.SetActive(interationType == InteractionType.PROCEED);
        }
        if (choicesContainer != null)
        {
            choicesContainer.SetActive(interationType == InteractionType.SELECTION);
        }
    }

    private void enqueueNewMessages(ICollection<DialogueAction> actions)
    {
        foreach (DialogueAction action in actions)
        {
            if (action != null && action.isEnded() && action.GetType().IsAssignableFrom(typeof(DialogueMessage)))
            {
                DialogueMessage message = (DialogueMessage)action;
                if (message != null && !visualizedMessages.Contains(message))
                {
                    pushMessage(message);
                    visualizedMessages.Add(message);
                }
            }
        }
    }

    private void pushMessage (DialogueMessage message)
    {
        if (message != null && messageGameObject != null)
        {
            if (messagesContainer != null)
            {
                string character = (message.Type == DialogueMessage.MessageType.RECEIVED) ? message.OwnerDialogue.CharacterId : null;
                GameObject newMessage = GameObject.Instantiate(messageGameObject, messagesContainer.transform);
                newMessage.transform.SetAsFirstSibling();
                MessageController newMessageController = newMessage.GetComponent<MessageController>();
                if (newMessageController != null) newMessageController.Setup(character, message.Text);
            }

            if (scroll != null && scroll.verticalScrollbar != null)
            {
                scroll.verticalScrollbar.value = 0;
            }
        }
    }

    private void setupChoices (string[]choices)
    {
        foreach (GameObject go in choiceControllers)
        {
            ChoiceController choiceController = go.GetComponent<ChoiceController>();
            if (choiceController != null)
            {
                choiceController.onChoiceSelected -= onChoiceSelectedCallback;
            }
            Destroy(go);
        }
        choiceControllers.Clear();

        if (choicesGameObject != null)
        {
            if (choicesContainer != null)
            {
                for (int i = 0; i < choices.Length; i++)
                {
                    GameObject newChoice = GameObject.Instantiate(choicesGameObject, choicesContainer.transform);
                    ChoiceController choiceController = newChoice.GetComponent<ChoiceController>();
                    if (choiceController != null)
                    {
                        choiceController.SetupButton(choices[i], i);
                        if (isStarted && isActiveAndEnabled)
                        {
                            choiceController.onChoiceSelected += onChoiceSelectedCallback;
                        }
                    }
                    choiceControllers.Add(newChoice);
                }
            }
        }
    }

    private void onChoiceSelectedCallback(int i)
    {
        if (onChoiceSelected != null)
        {
            onChoiceSelected(i);
        }
    }

    private void onProceedCallback()
    {
        //StartCoroutine(StartProceedDialogueButtonCooldown());
        if (onProceedButtonClicked != null)
        {
            onProceedButtonClicked();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        choicesContainer.SetActive(choiceControllers.Count > 0);
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

    private void Bind()
    {
        foreach (GameObject go in choiceControllers)
        {
            ChoiceController choiceController = go.GetComponent<ChoiceController>();
            if (choiceController != null)
            {
                choiceController.onChoiceSelected += onChoiceSelectedCallback;
            }
        }
        if (proceedDialogueButton != null)
        {
            proceedDialogueButton.onClick.AddListener(onProceedCallback);
        }
    }

    private void OnDisable()
    {
        Unbind();
    }

    private void Unbind()
    {
        foreach (GameObject go in choiceControllers)
        {
            ChoiceController choiceController = go.GetComponent<ChoiceController>();
            if (choiceController != null)
            {
                choiceController.onChoiceSelected -= onChoiceSelectedCallback;
            }
        }
        if (proceedDialogueButton != null)
        {
            proceedDialogueButton.onClick.RemoveListener(onProceedCallback);
        }
    }

    private IEnumerator StartProceedDialogueButtonCooldown()
    {
        if (proceedDialogueButton != null)
        {
            proceedDialogueButton.enabled = false;
        }
        yield return new WaitForSeconds(proceedDialogueDelay);
        if (proceedDialogueButton != null)
        {
            proceedDialogueButton.enabled = true;
        }
    }

}
