using data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatBarController : MonoBehaviour
{

    [System.Serializable]
    public class StatBarImageAndLevel
    {
        [Range(0f, 1f)]
        public float percentage;
        public Texture2D texture;
    }


    [SerializeField]
    private string avatarId;

    [SerializeField]
    private string statId;

    private StatProgressData statProgress;

    [SerializeField]
    private RectTransform fillBarTransform;

    [SerializeField]
    private RawImage fillBarImage;

    [SerializeField]
    [Range(0f, 1f)]
    private float minPercentage = 0;

    [SerializeField]
    [Range(0f, 1f)]
    private float maxPercentage = 1;

    //[SerializeField]
    //private Image foregroundImage;

    [SerializeField]
    private RawImage avatarImage;

    [SerializeField]
    private Texture2D fillBarMinLevelTexture;
    [SerializeField]
    private StatBarImageAndLevel[] fillBarLevels;

    private float MinValue;
    private float MaxValue;

    private bool isStarted = false;

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

                CharacterConsts characterData = gameData.findCharacterConsts(avatarId);
                if (avatarImage != null && characterData != null)
                {
                    avatarImage.texture = characterData.AvatarTexture;
                }

                if (statConst.IsCharacterStat())
                {
                    statProgress = gameData.ProgressData.findCharData(avatarId).findStat(statId);
                }
                else
                {
                    statProgress = gameData.ProgressData.findGlobalStat(statId);
                }
            }

            Bind();
        }
        isStarted = true;
    }

    private void OnEnable()
    {
        if (isStarted)
        {
            Bind();
        }
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
        float percentage = Mathf.Clamp(((float)(newValue - MinValue)) / (MaxValue - MinValue), minPercentage, maxPercentage);
        
        if (fillBarTransform != null)
        {
            fillBarTransform.anchorMax = new Vector2(percentage, fillBarTransform.anchorMax.y);            
        }

        if (fillBarImage != null)
        {
            float maxReachedPercentage = 0f;
            Texture2D maxPercentageTexture = fillBarMinLevelTexture;
            if (fillBarLevels != null)
            {
                foreach (StatBarImageAndLevel level in fillBarLevels)
                {
                    if (level != null)
                    {
                        if (percentage >= level.percentage && level.percentage > maxReachedPercentage)
                        {
                            maxReachedPercentage = level.percentage;
                            maxPercentageTexture = level.texture;
                        }
                    }
                }
            }

            if (maxPercentageTexture != null)
            {
                fillBarImage.texture = maxPercentageTexture;
            }
        }
    }

}
