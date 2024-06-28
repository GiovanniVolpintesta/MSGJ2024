using data;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.Rendering;

public class CharactersData : MonoBehaviour
{
    [SerializeField]
    CharacterConsts[] characterConsts;
    [SerializeField]
    string playerCharacterId = "Player";
    public ICollection<CharacterConsts> CharacterConst { get { return characterConsts.AsReadOnlyCollection(); } }

    public CharacterConsts findCharacterConsts(string characterId)
    {
        if (characterId == null) characterId = playerCharacterId;

        foreach (CharacterConsts e in CharacterConst)
        {
            if (e.Id.Equals(characterId))
                return e;
        }
        return null;
    }
}
