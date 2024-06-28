using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

namespace data
{
    public delegate void OnValueChanged(float OldValue, float NewValue);
    public delegate void OnEvent();

    [System.Serializable]
    public class ProgressData
    {
        [SerializeField]
        private List<CharacterProgressData> charactersData;
        public CharacterProgressData findCharData(string id)
        {
            foreach (CharacterProgressData e in charactersData)
            {
                if (e.Id.Equals(id)) return e;
            }
            return null;
        }

        [SerializeField]
        private List<StatProgressData> globalStats;
        public StatProgressData findGlobalStat(string id)
        {
            foreach (StatProgressData e in globalStats)
            {
                if (e.Id.Equals(id)) return e;
            }
            return null;
        }

        [SerializeField]
        private List<DialogueProgressData> dialogues;
        public DialogueProgressData findDialogue(string id)
        {
            foreach (DialogueProgressData e in dialogues)
            {
                if (e.Id.Equals(id)) return e;
            }
            return null;
        }

        [SerializeField]
        private string currentDialogueId;
        public string CurrentDialogueId
        {
            get { return currentDialogueId; }
            set { currentDialogueId = value; }
        }

        [SerializeField]
        private int playedDialogues;
        public int PlayedDialogues { get { return playedDialogues; } }

        public ProgressData()
        {
            charactersData = new List<CharacterProgressData>();
            globalStats = new List<StatProgressData>();
            dialogues = new List<DialogueProgressData>();
            playedDialogues = 0;
            currentDialogueId = null;
        }

        public void Bind()
        {
            foreach (DialogueProgressData d in dialogues)
            {
                if (d != null)
                {
                    d.onDialogueEnded += OnDialogueEnded;
                }
            }
        }

        public bool addGlobalStat(StatProgressData newStatData)
        {
            foreach (StatProgressData e in globalStats)
            {
                if (e.Id.Equals(newStatData.Id))
                {
                    Debug.LogError("Cannot add new stat progress data for global stat '" + newStatData.Id + "' because this stat already exists.");
                    return false;
                }
            }
            globalStats.Add(newStatData);
            return true;
        }

        public bool addCharacterData(CharacterProgressData newCharacterData)
        {
            foreach (CharacterProgressData e in charactersData)
            {
                if (e.Id.Equals(newCharacterData.Id))
                {
                    Debug.LogError("Cannot add nee progress data for character '" + newCharacterData.Id + "' because this character's data already exists.");
                    return false;
                }
            }
            charactersData.Add(newCharacterData);
            return true;
        }

        public bool addDialogueData(DialogueProgressData newDialogueProgressData)
        {
            foreach (DialogueProgressData e in dialogues)
            {
                if (e.Id.Equals(newDialogueProgressData.Id))
                {
                    Debug.LogError("Cannot add new progress data for dialogue '" + newDialogueProgressData.Id + "' because this dialogue's data already exists.");
                    return false;
                }
            }
            dialogues.Add(newDialogueProgressData);
            return true;
        }

        public bool dialogueCanBeUnlocked(string dialogueId)
        {
            // TODO
            return true;
        }

        private void OnDialogueEnded()
        {
            playedDialogues++;
        }
    }

    [System.Serializable]
    public class CharacterProgressData
    {
        [SerializeField]
        private string id;
        public string Id { get { return id; } }

        [SerializeField]
        private List<StatProgressData> stats;
        public StatProgressData findStat(string id)
        {
            foreach (StatProgressData e in stats)
            {
                if (e.Id.Equals(id)) return e;
            }
            return null;
        }

        public CharacterProgressData(string id)
        {
            this.id = id;
            stats = new List<StatProgressData>();
        }

        public bool addStat(StatProgressData newStatData)
        {
            foreach (StatProgressData e in stats)
            {
                if (e.Id.Equals(e.Id))
                {
                    Debug.LogError("Cannot add new stat progress data for stat '" + newStatData.Id + "' in character '" + Id + "' because this stat already exists.");
                    return false;
                }
            }
            stats.Add(newStatData);
            return true;
        }
    }

    [System.Serializable]
    public class StatProgressData
    {
        public event OnValueChanged onValueChanged;

        [SerializeField]
        private string id;
        public string Id { get { return id; } }

        [SerializeField]
        private int value;
        public int Value { get { return value; } }

        private int min = int.MinValue;
        public int Min { get { return min; } }

        private int max = int.MaxValue;
        public int Max { get { return max; } }

        public StatProgressData(string id, int startingValue)
        {
            this.id = id;
            this.value = startingValue;
        }

        public void setConstraints(int minValue, int maxValue)
        {
            this.min = minValue;
            this.max = maxValue;
        }

        public void setValue(int value)
        {
            value = Mathf.Clamp(value, min, max);
            int oldValue = this.value;
            if (oldValue != value)
            {
                this.value = value;
                Debug.Log("Stat " + Id + " changed from " + oldValue + " to " + value);
                if (onValueChanged != null)
                {
                    onValueChanged(oldValue, value);
                }
            }
        }

        public void incrementValue(int increment) { setValue(this.value + increment); }
    }

    [System.Serializable]
    public class DialogueProgressData
    {
        public event OnEvent onDialogueEnded;

        [SerializeField]
        private string id;
        public string Id { get { return id; } }

        [SerializeField]
        private bool unlocked;
        public bool IsUnlocked { get { return unlocked; } }
        public void Unlock()
        {
            if (!unlocked)
            {
                unlocked = true;
                Debug.Log("Dialogue '" + id + "' unlocked");
            }
        }

        [SerializeField]
        private bool ended;
        public bool IsEnded { get { return ended; } }
        public void setEnded()
        {
            if (!ended)
            {
                ended = true;
                Debug.Log("Dialogue '" + id + "' ended");
                if (onDialogueEnded != null)
                {
                    onDialogueEnded();
                }
            }
        }

        public DialogueProgressData(string id)
        {
            this.id = id;
            this.ended = false;
            this.unlocked = false;
        }
    }

}
