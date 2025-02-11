using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
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
        public ICollection<DialogueAction> DialogueActions { get { return dialogueActions.AsReadOnlyCollection(); } }

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
            DialogueAction actionToExecute = getActionToExecute();
            while (actionToExecute != null)
            {
                actionToExecute.execute();
                actionToExecute = getActionToExecute();
                if (actionToExecute != null && actionToExecute.shouldStopBeforeExecuting())
                    break;
            }
        }

        public bool isEnded()
        {
            DialogueAction actionToExecute = getActionToExecute();
            return actionToExecute == null;
        }
    }
}