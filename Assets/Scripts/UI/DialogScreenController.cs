using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogScreenController : MonoBehaviour
{
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

    public void clearMessages()
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
    }

    public void pushMessage (string character, string message)
    {
        if (messageGameObject != null)
        {
            if (messagesContainer != null)
            {
                GameObject newMessage = GameObject.Instantiate(messageGameObject, messagesContainer.transform);
                newMessage.transform.SetAsFirstSibling();
                MessageController newMessageController = newMessage.GetComponent<MessageController>();
                if (newMessageController != null) newMessageController.Setup(character, message);
            }
            if (scroll != null && scroll.verticalScrollbar != null)
            {
                scroll.verticalScrollbar.value = 0;
            }
        }
    }
    public void setupChoices (string[]choices)
    {
        if (choicesGameObject != null)
        {
            if (proceedDialogueButton != null)
            {
                proceedDialogueButton.gameObject.SetActive(false);
            }
            if (choicesContainer != null)
            {
                choicesContainer.SetActive(true);
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
        if (choicesContainer != null)
        { 
            choicesContainer.SetActive(false);
        }
        if (proceedDialogueButton != null)
        {
            proceedDialogueButton.gameObject.SetActive(true);
        }
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
