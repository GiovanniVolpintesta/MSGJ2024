using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace data
{
    [System.Serializable]
    public class GameData
    {
        [SerializeField]
        private Stat[] stats;

        [SerializeField]
        private Dialogue[] dialogues;

        private Dialogue currentDialogue = null;
        private int gameTime = -1;

        static public GameData createFromJSON(string jsonString)
        {
            GameData gameData = JsonUtility.FromJson<GameData>(jsonString);

            foreach (Stat s in gameData.stats) s.Initialize(gameData);
            foreach (Dialogue d in gameData.dialogues) d.Initialize(gameData);

            return gameData;
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

        public void executeDialogue()
        {
            if (currentDialogue.isEnded())
                currentDialogue = null;

            if (currentDialogue == null)
                extractNewDialog();

            if (currentDialogue != null)
                currentDialogue.execute();
            else
                Debug.LogError("Nothing to execute!!!");
        }

        private void extractNewDialog()
        {
            List<Dialogue> unlockedDialogs = new List<Dialogue>();
            int maxPriority = int.MinValue;

            if (currentDialogue == null || currentDialogue.isEnded())
            {
                gameTime++;

                foreach (Dialogue d in dialogues)
                {
                    d.tryUnlock(stats, gameTime);

                    if (d.Unlocked)
                    {
                        if (d.Priority > maxPriority)
                        {
                            maxPriority = d.Priority;
                            unlockedDialogs.Clear();
                            unlockedDialogs.Add(d);
                        }
                        else if (maxPriority == d.Priority)
                        {
                            unlockedDialogs.Add(d);
                        }
                    }
                }
            }


        }
    }
}