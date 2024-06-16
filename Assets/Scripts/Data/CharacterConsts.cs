using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace data
{
    [System.Serializable]
    public class CharacterConsts
    {
        [SerializeField]
        private string id;
        public string Id { get { return id; } }

        [SerializeField]
        private Texture2D avatarTexture;
        public Texture2D AvatarTexture { get { return avatarTexture; } }

        [SerializeField]
        private Color SatisfationBarColor;

        [SerializeField]
        private Texture2D SatisfationBarForeground;
    }
}
