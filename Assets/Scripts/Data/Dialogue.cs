using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
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
        private DialogueUnlockCondition[] unlockConditions;

        [SerializeField]
        private DialogueActionInitializer[] messages;
        private DialogueAction[] dialogueActions;
        public ICollection<DialogueAction> DialogueActions { get { return dialogueActions.AsReadOnlyCollection(); } }

        [SerializeField]
        private DialogueChoiceCase[] choices;
        public string[] getChoicesMessages()
        {
            List<string> msgs = new List<string>();
            if (choices != null)
            {
                foreach (DialogueChoiceCase c in choices)
                {
                    msgs.Add((c != null && c.BoxText != null) ? c.BoxText : "");
                }
            }
            return msgs.ToArray();
        }

        private DialogueProgressData dialogueProgress;

        private int chosenCaseIndex;
        public void SetChoiceSelection(int index)
        {
            if (areDialogueActionsEnded())
            {
                if (chosenCaseIndex == -1)
                {
                    if (index >= 0 && index < choices.Length)
                    {
                        chosenCaseIndex = index;
                    }
                    else
                    {
                        Debug.LogError("The selected choice index ("+index+") is not valid (there are "+choices.Length+" choices available).");
                    }
                }
                else
                {
                    Debug.LogWarning("A dialog choice override operation has been aborted.");
                }
            }
            else
            {
                Debug.LogWarning("Choices cannot be set before the main dialog is ended.");
            }
        }

        public DialogueChoiceCase getChosenCase()
        {
            if (chosenCaseIndex >= 0 && chosenCaseIndex < choices.Length)
            {
                return choices[chosenCaseIndex];
            }
            return null;
        }

        private DialogueAction getDialogueActionToExecute()
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

        public bool isUnlocked()
        {
            return dialogueProgress != null && dialogueProgress.IsUnlocked;
        }

        public bool canBeUnlocked()
        {
            if (unlockConditions != null)
            {
                foreach (DialogueUnlockCondition condition in unlockConditions)
                {
                    if (condition != null && !DialogueUnlockCondition.checkCondition(this, condition))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool isStarted()
        {
            if (dialogueActions.Length > 0)
            {
                return dialogueActions[0].isEnded();
            }
            else
            {
                DialogueChoiceCase chosenCase = getChosenCase();
                if (chosenCase != null)
                    return chosenCase.DialogueActions.Count > 0 && chosenCase.DialogueActions.First().isEnded();
                else
                    return false;
            }
        }

        public bool isEnded()
        {
            return dialogueProgress != null && dialogueProgress.IsEnded;
        }

        public bool canExecute()
        {
            return isUnlocked() && !isEnded();
        }

        public void execute()
        {
            if (canExecute())
            {
                Debug.Log("Executing dialog: " + Id);
                if (!areDialogueActionsEnded())
                {
                    DialogueAction actionToExecute = getDialogueActionToExecute();
                    while (actionToExecute != null)
                    {
                        actionToExecute.execute();
                        actionToExecute = getDialogueActionToExecute();
                        if (actionToExecute != null && actionToExecute.shouldStopBeforeExecuting())
                            break;
                    }

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

                if (areDialogueActionsEnded() && areChoiceActionsEnded())
                {
                    if (dialogueProgress == null)
                        throw new InvalidOperationException("There is no dialogue progress data to store the ended state into.");

                    dialogueProgress.setEnded();
                }
            }
        }

        public bool areDialogueActionsEnded()
        {
            DialogueAction actionToExecute = getDialogueActionToExecute();
            return actionToExecute == null;
        }

        public bool areChoiceActionsEnded()
        {
            if (choices != null && choices.Length > 0)
            {
                DialogueChoiceCase chosenCase = getChosenCase();
                return chosenCase != null ? chosenCase.isEnded() : false;
            }
            else return true;
        }
    }
}
