using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameData", order = 1)]
public class GameData : ScriptableObject
{
    private static GameData instance;
   public static GameData Instance
    {
        get
        {
            if (instance == null)
                instance = Resources.Load<GameData>("ScriptableObjects/GameData");

            return instance;
        }
        set
        {
            instance = value;
        }
    }

    [SerializeField] List<Piece> piecePrefabs;
    [SerializeField] Sprite[] levelSprites;
    [SerializeField] int[] boardSizeCounts;


    public List<Piece> GetPiecePrefabs()
    {
        return piecePrefabs;
    } 
    public Sprite[] GetLevelDatas()
    {
        return levelSprites;
    }
    public int[] GetBoardSizeItems()
    {
        return boardSizeCounts;
    }
}
