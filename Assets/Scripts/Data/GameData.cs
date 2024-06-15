using System;
using UnityEngine;

namespace data
{
    [System.Serializable]
    public class GameData
    {
        //[SerializeField]
        public Stat[] stats;

        //[SerializeField]
        public Dialogue[] dialogues;

        static public GameData createFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<GameData>(jsonString);
        }
    }
}