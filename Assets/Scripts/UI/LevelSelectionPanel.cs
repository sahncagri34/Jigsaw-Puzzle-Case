using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionPanel : BasePanel
{
    [Header("Parents")]
    [SerializeField] private Transform levelItemsParent;
    [SerializeField] private Transform pieceCountsParent;

    [Header("Prefabs")]
    [SerializeField] private LevelImageItem levelItemPrefab;
    [SerializeField] private BoardSizeItem boardSizeItemPrefab;

    [Header("References")]
    [SerializeField] private Button okayButton;

    private List<LevelImageItem> levelItems = new List<LevelImageItem>();
    private List<BoardSizeItem> boardItems = new List<BoardSizeItem>();

    private Sprite[] levelSprites;
    private int[] boardSizeItems;

    private LevelImageItem currentSelectedLevelItem;
    private BoardSizeItem currentSelectedSizeItem;

    public static event Action<Sprite,int> OnLevelSelected;

    private void Awake()
    {
        levelSprites = GameData.Instance.GetLevelDatas();
        boardSizeItems = GameData.Instance.GetBoardSizeItems();

        okayButton.onClick.AddListener(OnOkayButtonClicked);
    }

    private void Start() => SetLevelItems();

    private void OnOkayButtonClicked()
    {
        if(currentSelectedSizeItem == null || currentSelectedLevelItem == null)
        {
            // TODO  : UIManager.Instance.PushPanel(UIPanelsType.Notify); with the message : "Please Select one of the images"
            return;
        }
        OnLevelSelected?.Invoke(currentSelectedLevelItem.LevelSprite, currentSelectedSizeItem.boardSize);

        UIManager.Instance.PushPanel(UIPanelType.GamePanel);
    }

    private void SetLevelItems()
    {
        foreach (var levelData in levelSprites)
        {
            LevelImageItem levelItemInstance =  Instantiate(levelItemPrefab,levelItemsParent);
            levelItemInstance.Set(levelData);
            levelItems.Add(levelItemInstance);

            levelItemInstance.OnSelected += LevelItemInstance_OnSelected;
            levelItemInstance.OnDestroyed += LevelItemInstance_OnDestroyed;
        }

        foreach (int count in boardSizeItems)
        {
            BoardSizeItem countItemInstance = Instantiate(boardSizeItemPrefab, pieceCountsParent);
            countItemInstance.Set(count);
            boardItems.Add(countItemInstance);

            countItemInstance.OnSelected += PieceCountItem_OnSelected;
            countItemInstance.OnDestroyed += PieceCountItem_OnDestroyed;
        }

    }

    private void LevelItemInstance_OnSelected(LevelImageItem levelUIItem)
    {
        currentSelectedLevelItem = levelUIItem;

        foreach (var levelItem in levelItems)
        {
            levelItem.SetIfSelected(levelUIItem == levelItem);
        }
    }

    private void LevelItemInstance_OnDestroyed(LevelImageItem levelUIItem)
    {
        levelUIItem.OnSelected -= LevelItemInstance_OnSelected;
        levelUIItem.OnDestroyed -= LevelItemInstance_OnDestroyed;
    }

    private void PieceCountItem_OnSelected(BoardSizeItem boardSizeItem)
    {
        currentSelectedSizeItem = boardSizeItem;

        foreach (var countItem in boardItems)
        {
            countItem.SetIfSelected(boardSizeItem == countItem);
        }
    }

    private void PieceCountItem_OnDestroyed(BoardSizeItem boardSizeItem)
    {
        boardSizeItem.OnSelected -= PieceCountItem_OnSelected;
        boardSizeItem.OnDestroyed -= PieceCountItem_OnDestroyed;
    }


}
