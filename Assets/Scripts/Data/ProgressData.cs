using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace data
{
    public delegate void OnValueChanged(float OldValue, float NewValue);

    [System.Serializable]
    public class ProgressData
    {
        public delegate void OnDialogueEnded (Dialogue dialogue, bool gameTimeIncremented);
        public event OnDialogueEnded onDialogueEnded;

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
        public ICollection<DialogueProgressData> Dialogues { get { return dialogues.AsReadOnlyCollection(); } }

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
        [SerializeField]
        private int playedDialoguesInLastDay;

        public ProgressData()
        {
            charactersData = new List<CharacterProgressData>();
            globalStats = new List<StatProgressData>();
            dialogues = new List<DialogueProgressData>();
            playedDialogues = 0;
            playedDialoguesInLastDay = 0;
            currentDialogueId = null;
        }

        public void Bind()
        {
            foreach (DialogueProgressData d in dialogues)
            {
                if (d != null)
                {
                    d.onDialogueEnded += OnDialogueEndedCallback;
                }
            }
        }

        public void Unbind()
        {
            foreach (DialogueProgressData d in dialogues)
            {
                if (d != null)
                {
                    d.onDialogueEnded -= OnDialogueEndedCallback;
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

        private void OnDialogueEndedCallback(string dialogueId)
        {
            Dialogue dialogue = GameData.Instance.findDialogue(dialogueId);

            bool incrementTime = false;
            if (dialogue != null)
            {
                if (dialogue.CharacterId != null && GameData.Instance.SendersCounted.Contains(dialogue.CharacterId))
                {
                    incrementTime = true;

                    // TODO: remove this logic and substitute with a time stat defined in json
                    playedDialogues++;
                    playedDialoguesInLastDay++;
                    if (playedDialoguesInLastDay >= GameData.Instance.DialoguesPerDay)
                    {
                        StatProgressData dayProgressData = findGlobalStat(GameData.Instance.DayStatId);
                        if (dayProgressData != null) dayProgressData.incrementValue(1);

                        playedDialoguesInLastDay -= GameData.Instance.DialoguesPerDay;
                    }
                }

                if (onDialogueEnded != null)
                {
                    onDialogueEnded(dialogue, incrementTime);
                }
            }
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

        [SerializeField]
        private bool saveHistory;
        [SerializeField]
        private List<int> history = new List<int>();
        public ICollection<int> History { get { return history.AsReadOnlyCollection(); } }

        public StatProgressData(string id, int startingValue, bool saveHistory)
        {
            this.id = id;
            this.value = startingValue;
            this.saveHistory = saveHistory;
            history.Clear();
            if (saveHistory)
            {
                history.Add(startingValue);
            }
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
                if (saveHistory)
                {
                    history.Add(value);
                }
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
        public delegate void OnDialogueEnded(string dialogue);

        public event OnDialogueEnded onDialogueEnded;

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
                    onDialogueEnded(id);
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
