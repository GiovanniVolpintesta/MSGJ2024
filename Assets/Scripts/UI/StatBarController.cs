using data;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.UI;

public class StatBarController : MonoBehaviour
{
    [SerializeField]
    private string characterId;

    [SerializeField]
    private string statId;

    private StatProgressData statProgress;

    [SerializeField]
    private RectTransform fillBarTransform;

    //[SerializeField]
    //private Image fillBarImage;

    //[SerializeField]
    //private Image foregroundImage;

    [SerializeField]
    private RawImage avatarImage;

    private float MinValue;
    private float MaxValue;

    private void Start()
    {
        if (statId != null && !statId.Equals(""))
        {
            GameData gameData = GameData.Instance;
            Stat statConst = gameData.findStat(statId);

            if (statConst != null)
            {
                MinValue = statConst.Min;
                MaxValue = statConst.Max;

                if (statConst.IsGlobalStat())
                {
                    CharacterConsts characterData = gameData.findCharacterConsts(characterId);
                    statProgress = gameData.ProgressData.findGlobalStat(statId);
                    if (avatarImage != null)
                    {
                        avatarImage.texture = characterData.AvatarTexture;
                    }
                }
                else
                {
                    statProgress = gameData.ProgressData.findCharData(characterId).findStat(statId);
                }
            }

            Bind();
        }
    }

    private void OnEnable()
    {
        Bind();
    }

    private void OnDisable() 
    {
        UnBind();
    }

    private void Bind()
    {
        if (statProgress != null)
        {
            statProgress.onValueChanged += OnValueChanged;
            OnValueChanged(statProgress.Value, statProgress.Value);
        }
    }

    private void UnBind()
    {
        if (statProgress != null)
        {
            statProgress.onValueChanged -= OnValueChanged;
        }
    }

    private void OnValueChanged(float oldValue, float newValue)
    {
        if (fillBarTransform != null)
        {
            float percentage = Mathf.Clamp(((float)(newValue - MinValue)) / (MaxValue - MinValue), 0f, 1f);
            fillBarTransform.anchorMax = new Vector2(percentage, fillBarTransform.anchorMax.y);
        }
    }

}
