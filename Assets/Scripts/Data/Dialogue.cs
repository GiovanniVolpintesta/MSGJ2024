using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace data
{

    [System.Serializable]
    public class Dialogue
    {
        private static int defaultIdGenerator;
        public static void ResetDefaultIdGenerator() { defaultIdGenerator = 1; }

        [SerializeField]
        private string id = "Dial_Gen_ID_" + defaultIdGenerator++;
        public string Id { get { return id; } }


        [SerializeField]
        private string characterId;
        public string CharacterId { get { return characterId; } }

        // TODO: implement check unlock condition

        [SerializeField]
        private int priority = 0;
        public int DialoguePriority { get { return priority; } }
      
        [SerializeField]
        private DialogueActionInitializer[] messages;
        private DialogueAction[] dialogueActions;

        [SerializeField]
        DialogueChoiceCase[] choices;

        DialogueProgressData dialogueProgress;

        private int chosenCaseIndex;
        public int ChosenCaseIndex
        {
            get { return chosenCaseIndex; }
            set
            {
                if (chosenCaseIndex == -1)
                {
                    if (areDialogueActionsEnded())
                    {
                        chosenCaseIndex = value;
                    }
                    else
                    {
                        Debug.LogError("A dialog choice has been aborted because the previous actions are not complete.");
                    }
                }
                else
                {
                    Debug.LogWarning("A dialog choice override operation has been aborted.");
                }
            }
        }

        private DialogueChoiceCase getChosenCase()
        {
            if (chosenCaseIndex >= 0 && chosenCaseIndex < choices.Length)
            {
                return choices[chosenCaseIndex];
            }
            return null;
        }

        private DialogueAction getActionToExecute()
        {
            foreach (DialogueAction action in dialogueActions)
            {
                if (action != null && !action.isEnded()) return action;
            }

            return null;
        }

        public void Initialize(GameData ownerData)
        {
            dialogueActions = DialogueActionInitializer.createDialogActionList(messages, this, ownerData);
            if (choices != null)
            {
                foreach (DialogueChoiceCase c in choices) if (c != null) c.Initialize(this, ownerData);
            }
            chosenCaseIndex = -1;
            dialogueProgress = ownerData.ProgressData.findDialogue(Id);
        }

        public void execute()
        {
            if (!isEnded())
            {
                Debug.Log("Executing dialog: " + Id);
                if (!areDialogueActionsEnded())
                {
                    DialogueAction actionToExecute = getActionToExecute();
                    if (actionToExecute != null)
                        actionToExecute.execute();
                    if (areDialogueActionsEnded())
                    {
                        Debug.Log("Ended dialog sequence: " + Id);
                    }
                }
                else if (!areChoiceActionsEnded())
                {
                    DialogueChoiceCase chosenCase = getChosenCase();
                    if (chosenCase != null)
                    {
                        chosenCase.execute();
                        Debug.Log("Executed dialog choice: " + chosenCase.BoxText);
                    }
                    if (areChoiceActionsEnded())
                    {
                        Debug.Log("Choice execution completed: " + chosenCase.BoxText);
                    }
                }
                else
                {
                    Debug.LogError("Inconsistency in the dialog completion state. This should never happen.");
                }
                if (isEnded())
                {
                    dialogueProgress.setEnded();
                }
            }
        }

        public bool isEnded()
        {
            return areDialogueActionsEnded() && areChoiceActionsEnded();
        }

        private bool areDialogueActionsEnded()
        {
            DialogueAction actionToExecute = getActionToExecute();
            return actionToExecute == null;
        }

        private bool areChoiceActionsEnded()
        {
            DialogueChoiceCase chosenCase = getChosenCase();
            return chosenCase != null ? chosenCase.isEnded() : false;
        }
    }
}
