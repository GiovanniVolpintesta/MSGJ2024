using data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageController : MonoBehaviour
{
    [SerializeField]
    private RawImage avatarImage;

    [SerializeField]
    private TextMeshProUGUI messageTextBox;

    Texture avatarTexture;
    string message;

    public void Setup (string characterId, string message)
    {
        this.message = message;

        CharacterConsts characterConsts = GameData.Instance.findCharacterConsts(characterId);
        avatarTexture = characterConsts.AvatarTexture;

        if (avatarImage != null && avatarTexture != null)
        {
            avatarImage.texture = characterConsts.AvatarTexture;
        }
        if (messageTextBox)
        {
            messageTextBox.SetText(message);
        }
    }
}
