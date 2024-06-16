using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace data
{
    [System.Serializable]
    public class GameData
    {
        private static GameData instance;
        public static GameData Instance { get { return instance; } }

        [SerializeField]
        private Stat[] stats;

        [SerializeField]
        private Dialogue[] dialogues;

        private CharactersData charactersData;
        [SerializeField]
        private ProgressData progressData;
        public ProgressData ProgressData { get { return progressData; } }

        private bool initialized = false;

        static public void createFromJSON(string jsonString)
        {
            instance = JsonUtility.FromJson<GameData>(jsonString);
        }

        public void Initialize(CharactersData charactersData)
        {
            if (!initialized)
            {
                this.charactersData = charactersData;

                InitializeProgressData();

                foreach (Stat s in stats) s.Initialize(this);
                foreach (Dialogue d in dialogues) d.Initialize(this);

                initialized = true;
            }
            else
            {
                Debug.LogError("Game data already initialized!!!");
            }
        }

        private void InitializeProgressData ()
        {
            progressData = new ProgressData();

            foreach (Stat s in stats)
            {
                if (s != null && s.IsGlobalStat())
                {
                    StatProgressData statProgressData = new StatProgressData(s.Id, s.InitValue);
                    statProgressData.setConstraints(s.Min, s.Max);
                    progressData.addGlobalStat(statProgressData);
                }
            }

            foreach (CharacterConsts c in charactersData.CharacterConst)
            {
                if (c != null)
                {
                    CharacterProgressData characterProgressData = new CharacterProgressData(c.Id);

                    foreach(Stat s in stats)
                    {
                        if (s != null && s.IsCharacterStat())
                        {
                            StatProgressData statProgressData = new StatProgressData(s.Id, s.InitValue);
                            statProgressData.setConstraints(s.Min, s.Max);
                            characterProgressData.addStat(statProgressData);
                        }
                    }

                    progressData.addCharacterData(characterProgressData);
                }
            }

            foreach(Dialogue d in dialogues)
            {
                if (d != null)
                {
                    DialogueProgressData dialogueProgressData = new DialogueProgressData(d.Id);
                    if (progressData.dialogueCanBeUnlocked(d.Id))
                    {
                        dialogueProgressData.Unlock();
                    }
                    progressData.addDialogueData(dialogueProgressData);
                }
            }
        }

        public Stat findStat(string id)
        {
            foreach (Stat e in stats) if (e.Id.Equals(id)) return e;
            return null;
        }

        public Dialogue findDialogue(string id)
        {
            foreach (Dialogue e in dialogues) if (e.Id.Equals(id)) return e;
            return null;
        }

        public CharacterConsts findCharacterConsts(string characterId)
        {
            foreach (CharacterConsts e in charactersData.CharacterConst)
            {
                if (e.Id.Equals(characterId))
                    return e;
            }
            return null;
        }

        public void executeDialogue()
        {
            DialogueProgressData dialogueProgress = null;            
            if (progressData.CurrentDialogueId != null)
                dialogueProgress = progressData.findDialogue(progressData.CurrentDialogueId);
            
            if (dialogueProgress == null || dialogueProgress.IsEnded)
                progressData.CurrentDialogueId = null;

            if (progressData.CurrentDialogueId == null)
                progressData.CurrentDialogueId = extractNewDialog();

            Dialogue newDialogue = null;
            if (progressData.CurrentDialogueId != null)
                newDialogue = findDialogue(progressData.CurrentDialogueId);

            if (newDialogue != null)
                newDialogue.execute();
            else
                Debug.LogError("Nothing to execute!!!");
        }

        private string extractNewDialog()
        {
            List<Dialogue> unlockedDialogs = new List<Dialogue>();
            int maxPriority = int.MinValue;

            DialogueProgressData dialogueProgress = null;
            if (progressData.CurrentDialogueId != null)
                dialogueProgress = progressData.findDialogue(progressData.CurrentDialogueId);

            if (dialogueProgress == null || dialogueProgress.IsEnded)
            {
                progressData.incrementGameTime();

                foreach (Dialogue d in dialogues)
                {
                    if (d != null)
                    {
                        DialogueProgressData dialogProgress = progressData.findDialogue(d.Id);

                        if (progressData.dialogueCanBeUnlocked(d.Id))
                        {
                            dialogProgress.Unlock();
                        }

                        if (dialogProgress.IsUnlocked)
                        {
                            if (d.DialoguePriority > maxPriority)
                            {
                                maxPriority = d.DialoguePriority;
                                unlockedDialogs.Clear();
                                unlockedDialogs.Add(d);
                            }
                            else if (maxPriority == d.DialoguePriority)
                            {
                                unlockedDialogs.Add(d);
                            }
                        }
                    }
                }
            }

            if (unlockedDialogs.Count > 0)
            {
                return unlockedDialogs[UnityEngine.Random.Range(0, unlockedDialogs.Count)].Id;
            }
            else
            {
                return null;
            }
        }
    }
}