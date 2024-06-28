using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace data
{
    public class IncrementStatAction : DialogueAction
    {
        private string statId;
        private int value;

        private StatProgressData statProgress;

        private bool executed;

        public IncrementStatAction(string statId, int value)
        {
            this.statId = statId;
            statProgress = null;
            this.value = value;
        }

        public override void execute()
        {
            if (!isEnded())
            {
                statProgress.incrementValue(value);
                executed = true;
            }
        }

        protected override void Initialize(GameData ownerData)
        {
            Stat statConsts = ownerData.findStat(statId);
            if (statConsts.IsCharacterStat())
            {
                CharacterProgressData charProgress = ownerData.ProgressData.findCharData(OwnerDialogue.CharacterId);
                if (charProgress != null)
                    statProgress = charProgress.findStat(statId);
                else
                    throw new KeyNotFoundException("Undefined character '"+OwnerDialogue.CharacterId+"'. Define it inside the CharacterConsts object.");
            }
            else if (statConsts.IsGlobalStat()) 
            {
                statProgress = ownerData.ProgressData.findGlobalStat(statId);
            }

            if (statProgress == null) Debug.LogError("Stat progress for stat '" + statId + "' has not been found.");

            executed = false;
        }

        public override bool isEnded()
        {
            return executed;
        }

        public override bool shouldStopBeforeExecuting()
        {
            return false;
        }
        
    }
}