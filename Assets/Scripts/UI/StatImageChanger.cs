using data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatImageChanger : MonoBehaviour
{
    [System.Serializable]
    private class StatImageValuePair
    {
        [SerializeField]
        public float value;
        [SerializeField]
        public Texture2D texture;
    }

    [SerializeField]
    RawImage image;

    [SerializeField]
    private string statCharId;
    [SerializeField]
    private string statId;

    [SerializeField]
    Texture2D startingTexture;

    [SerializeField]
    StatImageValuePair[] statLevelsToTextures;

    Stat stat = null;
    StatProgressData statProgress = null;

    void Start()
    {
        stat = GameData.Instance.findStat(statId);

        if (stat != null)
        {
            if (stat.IsGlobalStat())
            {
                statProgress = GameData.Instance.ProgressData.findGlobalStat(stat.Id);
                if (statCharId != null && statCharId != "") Debug.LogWarning("Stat '" + stat.Id + "' is not a character stat. The statCharId is ignored.");
            }
            else
            {
                CharacterProgressData charProgress = GameData.Instance.ProgressData.findCharData(statCharId);
                statProgress = (charProgress != null) ? charProgress.findStat(stat.Id) : null;
            }
        }

        Bind();

        Refresh();
    }


    private void OnDestroy()
    {
        Unbind();
    }

    private void Bind()
    {
        if (statProgress != null)
        {
            statProgress.onValueChanged += onStatChange;
        }
    }

    private void Unbind()
    {
        if (statProgress != null)
        {
            statProgress.onValueChanged -= onStatChange;
        }
    }

    private void onStatChange (float oldValue, float newValue)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (image != null && stat != null && statProgress != null)
        {
            Texture2D texture = startingTexture;
            float maxLevelPassed = stat.Min;
            foreach (StatImageValuePair pair in statLevelsToTextures)
            {
                if (pair != null && pair.texture != null
                    && (statProgress.Value > pair.value || Mathf.Approximately(statProgress.Value, pair.value))
                    && pair.value > maxLevelPassed)
                {
                    maxLevelPassed = pair.value;
                    texture = pair.texture;
                }
            }

            if (texture != null)
            {
                image.texture = texture;
            }
        }
    }
}
