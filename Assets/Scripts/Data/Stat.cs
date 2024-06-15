using System.Collections;
using System.Collections.Generic;
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
        private int? actualValue;
        public int value
        {
            get { return actualValue != null ? actualValue.Value : initValue; }
            set { actualValue = value; }
        }
    }
}