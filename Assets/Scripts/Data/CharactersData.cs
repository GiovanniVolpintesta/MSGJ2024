using data;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class CharactersData : MonoBehaviour
{
    [SerializeField]
    CharacterConsts[] characterConsts;
    public ICollection<CharacterConsts> CharacterConst { get { return characterConsts.AsReadOnlyCollection(); } }
}
