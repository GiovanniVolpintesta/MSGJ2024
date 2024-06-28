using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Serialization;

namespace data
{
    [System.Serializable]
    public class DialogueChoiceCase
    {
        [SerializeField]
        private string boxText;
        public string BoxText { get { return boxText; } }

        [SerializeField]
        private DialogueActionInitializer[] messages;

        private DialogueAction[] dialogueActions;

        public DialogueAction getActionToExecute()
        {
            foreach (DialogueAction action in dialogueActions)
            {
                if (action != null && !action.isEnded()) return action;
            }
            return null;
        }

        public void Initialize(Dialogue ownerDialogue, GameData ownerData)
        {
            dialogueActions = DialogueActionInitializer.createDialogActionList(messages, ownerDialogue, ownerData);
        }

        public void execute()
        {
            DialogueAction actionToExecute;
            do
            {
                actionToExecute = getActionToExecute();
                if (actionToExecute != null)
                    actionToExecute.execute();
            } while (actionToExecute != null && !actionToExecute.shouldStopAfterExecuting());
        }

        public bool isEnded()
        {
            DialogueAction actionToExecute = getActionToExecute();
            return actionToExecute == null;
        }
    }
}