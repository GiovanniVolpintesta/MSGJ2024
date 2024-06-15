
using data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

[System.Serializable]
public class DialogueActionInitializer
{
    public static class Types
    {
        public static readonly string receivedMsg = "received";
        public static readonly string sentMsg = "sent";
        public static readonly string statIncrement = "statIncrement";
    }

    [SerializeField]
    private string type;

    [SerializeField]
    private string text;

    [SerializeField]
    private string stat;

    [SerializeField]
    private int value;

    public DialogueAction createDialogueAction()
    {
        if (type.Equals(Types.receivedMsg))
        {
            return new DialogueMessage(DialogueMessage.MessageType.RECEIVED, text);
        }
        else if (type.Equals(Types.sentMsg))
        {
            return new DialogueMessage(DialogueMessage.MessageType.SENT, text);
        }
        else if (type.Equals(Types.statIncrement))
        {
            return new IncrementStatAction(stat, value);
        }
        throw new NotImplementedException();
    }

    public static DialogueAction[] createDialogActionList(DialogueActionInitializer[] initializers, GameData ownerData)
    {
        List<DialogueAction> result = new List<DialogueAction>();
        if (initializers != null)
        {
            foreach (DialogueActionInitializer e in initializers)
            {
                if (e != null)
                {
                    DialogueAction action = e.createDialogueAction();
                    if (action != null)
                    {
                        action.Initialize(ownerData);
                        result.Add(action);
                    }
                }
            }
        }
        return result.ToArray();
    }
}
