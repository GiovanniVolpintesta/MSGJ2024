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
            dialogueActions = DialogueActionInitializer.createDialogActionList(messages, ownerData);
        }

        public void execute()
        {
            DialogueAction actionToExecute = getActionToExecute();
            if (actionToExecute != null)
                actionToExecute.execute();
        }

        public bool isEnded()
        {
            DialogueAction actionToExecute = getActionToExecute();
            return actionToExecute == null;
        }
    }
}