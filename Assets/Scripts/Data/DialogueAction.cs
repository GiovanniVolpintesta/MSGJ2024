using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace data
{
    public abstract class DialogueAction
    {
        private Dialogue ownerDialogue;
        public Dialogue OwnerDialogue { get { return ownerDialogue; } }

        public void Initialize(Dialogue ownerDialogue, GameData ownerData)
        {
            this.ownerDialogue = ownerDialogue;
            Initialize(ownerData);
        }

        protected abstract void Initialize(GameData ownerData);

        // Executes the action, then returns true if the execution has ended, false otherwise
        public abstract void execute();
        public abstract bool isEnded();
    }
}