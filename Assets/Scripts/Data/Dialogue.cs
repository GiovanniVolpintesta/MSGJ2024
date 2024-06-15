using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace data
{

    [System.Serializable]
    public class Dialogue
    {
        [SerializeField]
        private string characterId;
        public string CharacterId { get { return characterId; } }

        // TODO: implement check unlock condition

        [SerializeField]
        private int priority = 0;
        public int Priority { get { return priority; } }

    }
}
