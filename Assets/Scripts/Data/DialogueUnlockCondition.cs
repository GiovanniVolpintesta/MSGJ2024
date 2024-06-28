using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static data.DialogueUnlockCondition;

namespace data
{
    [System.Serializable]
    public class DialogueUnlockCondition
    {
        public enum StatType
        {
            NONE,
            STAT_EQUAL,
            STAT_NOT_EQUAL,
            STAT_HIGHER,
            STAT_HIGHER_OR_EQUAL,
            STAT_LOWER,
            STAT_LOWER_OR_EQUAL,
            DIALOGUE_PLAYED
        }

        private readonly static Dictionary<string, StatType> STRING_TO_TYPE = new Dictionary<string, StatType>
        {
            // {"", StatType.NONE} // Do not add StatType.NONE value

            {"statEqual", StatType.STAT_EQUAL},
            {"statNotEqual", StatType.STAT_NOT_EQUAL},
            {"statLower", StatType.STAT_HIGHER},
            {"statLowerEqual", StatType.STAT_HIGHER_OR_EQUAL},
            {"statHigher", StatType.STAT_LOWER},
            {"statHigherEqual", StatType.STAT_LOWER_OR_EQUAL},
            {"dialoguePlayed", StatType.DIALOGUE_PLAYED}
        };

        private readonly static StatType[] STAT_CONDITION_TYPES =
        {
            StatType.STAT_EQUAL,
            StatType.STAT_NOT_EQUAL,
            StatType.STAT_HIGHER,
            StatType.STAT_HIGHER_OR_EQUAL,
            StatType.STAT_LOWER,
            StatType.STAT_LOWER_OR_EQUAL
        };

        [SerializeField]
        private string type;
        public StatType Type { get { return STRING_TO_TYPE.GetValueOrDefault(type, StatType.NONE); } }

        [SerializeField]
        private string id;
        public string Id { get { return id; } }

        [SerializeField]
        private int value;
        public int Value { get { return value; } }

        public static bool checkCondition (Dialogue dialogue, DialogueUnlockCondition unlockCondition)
        {
            if (unlockCondition != null && dialogue != null)
            {
                StatType tmpType = unlockCondition.Type;
                if (tmpType != StatType.NONE)
                {
                    if (STAT_CONDITION_TYPES.Contains(tmpType))
                    {
                        Stat statConsts = GameData.Instance.findStat(unlockCondition.Id);
                        if (statConsts != null)
                        {
                            StatProgressData statProgressData = null;
                            if (statConsts.IsGlobalStat())
                            {
                                statProgressData = GameData.Instance.ProgressData.findGlobalStat(unlockCondition.Id);
                            }
                            else if (statConsts.IsCharacterStat())
                            {
                                CharacterProgressData characterProgressData = GameData.Instance.ProgressData.findCharData(dialogue.CharacterId);
                                statProgressData = characterProgressData != null ? characterProgressData.findStat(unlockCondition.Id) : null;
                            }

                            if (statProgressData != null)
                            {
                                switch (tmpType)
                                {
                                    case StatType.STAT_EQUAL: return unlockCondition.Value == statProgressData.Value;
                                    case StatType.STAT_NOT_EQUAL: return unlockCondition.Value != statProgressData.Value;
                                    case StatType.STAT_HIGHER: return unlockCondition.Value > statProgressData.Value;
                                    case StatType.STAT_HIGHER_OR_EQUAL: return unlockCondition.Value >= statProgressData.Value;
                                    case StatType.STAT_LOWER: return unlockCondition.Value < statProgressData.Value;
                                    case StatType.STAT_LOWER_OR_EQUAL: return unlockCondition.Value <= statProgressData.Value;
                                    default: throw new NotImplementedException("Invalid Case");
                                }
                            }
                        }
                        else
                        {
                            Debug.LogError("Error: UnlockCondition of dialog '"+dialogue.Id+"' is based on stat '"+ unlockCondition.Id + "', which is not defined. This dialog cannot be unlocked.");
                        }
                    }
                    else if (tmpType == StatType.DIALOGUE_PLAYED)
                    {
                        DialogueProgressData dialogueProgressData = GameData.Instance.ProgressData.findDialogue(unlockCondition.Id);
                        return dialogueProgressData != null && dialogueProgressData.IsEnded;
                    }
                }
            }

            return false;
        }
    }
}