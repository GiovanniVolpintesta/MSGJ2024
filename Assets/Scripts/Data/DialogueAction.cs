using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace data
{
    public abstract class DialogueAction
    {
        public abstract void Initialize(GameData ownerData);

        // Executes the action, then returns true if the execution has ended, false otherwise
        public abstract void execute();
        public abstract bool isEnded();
    }
}