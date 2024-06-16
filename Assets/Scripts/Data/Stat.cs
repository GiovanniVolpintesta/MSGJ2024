using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace data {

    [System.Serializable]
    public class Stat
    {
        [SerializeField]
        private string id;
        public string Id { get { return id; } }

        [SerializeField]
        private string isGlobal;
        public bool IsGlobalStat() { return isGlobal != null && isGlobal.ToUpper() == "TRUE"; }
        public bool IsCharacterStat() { return !IsGlobalStat(); }

        [SerializeField]
        private int min = int.MinValue;
        public int Min { get { return min; } }

        [SerializeField]
        private int max = int.MaxValue;
        public int Max { get { return max; } }

        [SerializeField]
        private int initValue;
        public int InitValue { get { return initValue; } }

        public void Initialize (GameData ownerData) { }
    }
}