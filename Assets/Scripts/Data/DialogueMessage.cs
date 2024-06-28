using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace data
{
    public class DialogueMessage : DialogueAction
    {
        public enum MessageType { SENT, RECEIVED };

        private MessageType type;
        public MessageType Type { get; }

        private string text;
        public string Text { get { return text; } }

        private bool executed;

        public DialogueMessage (MessageType type, string text)
        {
            this.type = type;
            this.text = text;
        }

        protected override void Initialize(GameData ownerData)
        {
            executed = false;
        }

        public override void execute()
        {
            if (!isEnded())
            {
                executed = true;
                //log();
            }
        }

        private void log()
        {
            if (type == MessageType.SENT)
            {
                Debug.Log("SENT: " + Text);
            }
            else if (type == MessageType.RECEIVED)
            {
                Debug.Log("RECEIVED: " + Text);
            }
            else
            {
                Debug.LogWarning("UNKNOWN MESSAGE TYPE: " + Text);
            }
        }

        public override bool isEnded()
        {
            return executed;
        }
    }
}