using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace data
{
    public class DialogueChoice : DialogueAction
    {
        private DialogueChoiceCase[] cases;

        public DialogueChoice(DialogueChoiceCase[] cases)
        {
            this.cases = cases;
        }

        [SerializeField]
        private int chosenCaseIndex;
        public int ChosenCaseIndex
        {
            get { return chosenCaseIndex; }
            set {
                if (chosenCaseIndex == -1)
                {
                    chosenCaseIndex = value;
                }
                else
                {
                    Debug.LogWarning("A valid dialog choice override operation has been aborted.");
                }
            }
        }

        private DialogueChoiceCase getChosenCase()
        {
            if (chosenCaseIndex >= 0 && chosenCaseIndex < cases.Length)
            {
                return cases[chosenCaseIndex];
            }
            return null;
        }

        public override void Initialize(GameData ownerData)
        {
            foreach (DialogueChoiceCase c in cases) if (c != null) c.Initialize(ownerData);
            chosenCaseIndex = -1;
        }

        public override void execute()
        {
            if (!isEnded())
            {
                DialogueChoiceCase chosenCase = getChosenCase();
                if (chosenCase != null)
                {
                    chosenCase.execute();
                    Debug.Log("Executed choice: " + chosenCase.BoxText);
                }
                if (isEnded())
                {
                    Debug.Log("Choice execution completed: " + chosenCase.BoxText);
                }
            }
        }

        public override bool isEnded()
        {
            DialogueChoiceCase chosenCase = getChosenCase();
            return chosenCase != null ? chosenCase.isEnded() : false;
        }
    }
}