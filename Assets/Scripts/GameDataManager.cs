using data;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    [SerializeField]
    private string DialogStructureFilePath;

    private GameData gameData;

    // Start is called before the first frame update
    void Awake()
    {
        using (StreamReader reader = new StreamReader(DialogStructureFilePath))
        {
            gameData = GameData.createFromJSON(reader.ReadToEnd());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
