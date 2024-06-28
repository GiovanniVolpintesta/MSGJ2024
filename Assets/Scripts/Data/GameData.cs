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

        private string firstDialogueId;

        private bool initialized = false;

        static public void createFromJSON(string jsonString)
        {
            instance = JsonUtility.FromJson<GameData>(jsonString);
        }

        public void Initialize(CharactersData charactersData, string firstDialogueId)
        {
            if (!initialized)
            {
                this.charactersData = charactersData;
                this.firstDialogueId = firstDialogueId;

                InitializeProgressData();
                progressData.CurrentDialogueId = firstDialogueId;

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

            progressData.Bind();
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
            return charactersData.findCharacterConsts(characterId);
        }

        public bool canExecuteCurrentDialogue()
        {
            Dialogue dialogue = null;
            if (progressData.CurrentDialogueId != null)
            {
                dialogue = findDialogue(progressData.CurrentDialogueId);
                if (dialogue != null && !dialogue.isEnded())
                {
                    DialogueProgressData dialogueProgress = null;
                    dialogueProgress = progressData.findDialogue(progressData.CurrentDialogueId);
                    if (dialogueProgress == null || !dialogueProgress.IsEnded)
                    {
                        return true;
                    }
                    else
                    {
                        Debug.LogError("Dialog progress data are not consistent with current game data. Dialog: " + progressData.CurrentDialogueId);
                    }
                }
            }
            return false;
        }

        public void executeCurrentDialogue()
        {
            DialogueProgressData dialogueProgress = null;
            Dialogue dialogue = null;
            if (progressData.CurrentDialogueId != null)
            {
                dialogue = findDialogue(progressData.CurrentDialogueId);
            }

            if (dialogue != null && !dialogue.isEnded())
            {
                dialogue.execute();
                if (dialogue.isEnded())
                {
                    dialogueProgress = progressData.findDialogue(progressData.CurrentDialogueId);
                    if (dialogueProgress != null)
                    {
                        dialogueProgress.setEnded();
                    }
                }
            }
        }

        public void extractNewDialog()
        {
            List<Dialogue> dialoguesToExecute = new List<Dialogue>();
            int maxPriority = int.MinValue;

            DialogueProgressData dialogueProgress = null;
            if (progressData.CurrentDialogueId != null)
                dialogueProgress = progressData.findDialogue(progressData.CurrentDialogueId);

            if (dialogueProgress == null || dialogueProgress.IsEnded)
            {
                foreach (Dialogue d in dialogues)
                {
                    if (d != null)
                    {
                        DialogueProgressData dialogProgress = progressData.findDialogue(d.Id);

                        if (!dialogProgress.IsUnlocked && progressData.dialogueCanBeUnlocked(d.Id))
                        {
                            dialogProgress.Unlock();
                        }

                        if (dialogProgress.IsUnlocked && !dialogProgress.IsEnded)
                        {
                            if (d.DialoguePriority >= maxPriority)
                            {
                                if (d.DialoguePriority > maxPriority)
                                {
                                    maxPriority = d.DialoguePriority;
                                    dialoguesToExecute.Clear();
                                }

                                dialoguesToExecute.Add(d);
                            }
                        }
                    }
                }

                if (dialoguesToExecute.Count > 0)
                {
                    progressData.CurrentDialogueId = dialoguesToExecute[UnityEngine.Random.Range(0, dialoguesToExecute.Count)].Id;
                }
                else
                {
                    progressData.CurrentDialogueId = null;
                }
            }
        }
    }
}