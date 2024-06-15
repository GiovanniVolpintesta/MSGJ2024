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
        private Stat stat;

        private bool executed;

        public IncrementStatAction(string statId, int value)
        {
            this.statId = statId;
            stat = null;
            this.value = value;
        }

        public override void execute()
        {
            if (!isEnded())
            {
                stat.incrementValue(value);
                executed = true;
                Debug.Log("Incrementing '" + stat.Id + "' stat by '" + stat.value + "'");
            }
            else
            {
                Debug.LogWarning("Stat " + statId + " increment skipped because this action has already been executed.");
            }
        }

        public override void Initialize(GameData ownerData)
        {
            stat = ownerData.findStat(statId);
            if (stat == null) Debug.LogError("Stat " + statId + " has not been found.");
            executed = false;
        }

        public override bool isEnded()
        {
            return executed;
        }
    }
}