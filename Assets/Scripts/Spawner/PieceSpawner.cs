using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    [SerializeField] Transform pieceParent;
    [SerializeField] Transform centeredObject;

    private Piece[,] Board;
    private List<Piece> piecePrefabs;

    private Sprite levelSprite;
    private int boardSize;

    #region UNITY FUNCTIONS

    private void Awake()
    {
        piecePrefabs = GameData.Instance.GetPiecePrefabs();
        LevelSelectionPanel.OnLevelSelected += LevelSelectionPanel_OnLevelSelected;
    }
   
    private void OnDestroy()
    {
        LevelSelectionPanel.OnLevelSelected -= LevelSelectionPanel_OnLevelSelected;
    }
    #endregion

    #region Spawn Functions
    private void CreateBoard(int length)
    {
        Board = new Piece[length, length];

        //Spawn Corners. We need to rotate the corner piece according to index of it
        SetCorners();

        //Spawn Inners
        SetInnerSlots();

        // Used Algortyhm to fill the empty slots.
        SetEmptySlots();

        //SetWholePuzzleSpriteObjectThatExistInEachPiece
        SetWholePuzzleImages();

        //AlignCenter
        AlignCenter();

    }
    private void SetCorners()
    {
        var cornerPiecePrefab = piecePrefabs.Find(x => x.TileType == TileType.TWO_FLAT);
        float resizeRatio = SpawnerTools.DEFAULT_ROW_COLUMN_COUNT / Board.GetLength(0);
        Coordinate[] cornerIndexes = Board.GetCornerIndexes();
        for (int i = 0; i < cornerIndexes.Length; i++)
        {
            var cornerInstance = Instantiate(cornerPiecePrefab, pieceParent);
            cornerInstance.Rotate(i);

            int rowCorner = cornerIndexes[i].rowIndex;
            int colCorner = cornerIndexes[i].colIndex;
            cornerInstance.SetPiece(levelSprite,rowCorner, colCorner, resizeRatio);
            Board[rowCorner, colCorner] = cornerInstance;
        }
    }
    private void SetInnerSlots()
    {
        var innerPiecePrefab = piecePrefabs.Find(x => x.TileType == TileType.NO_FLAT);
        Coordinate[] innerIndexes = Board.GetInnerIndexes();
        float resizeRatio = SpawnerTools.DEFAULT_ROW_COLUMN_COUNT / Board.GetLength(0);
        for (int i = 0; i < innerIndexes.Length; i++)
        {
            var innerInstance = Instantiate(innerPiecePrefab, pieceParent);

            int rowInner = innerIndexes[i].rowIndex;
            int colInner = innerIndexes[i].colIndex;

            var modeOfCol = colInner % 2;
            int modeOfRow = rowInner % 2;

            if ((modeOfRow == 0 && modeOfCol == 1) || (modeOfRow == 1 && modeOfCol == 0))
                innerInstance.Rotate(1);

            innerInstance.SetPiece(levelSprite, rowInner, colInner, resizeRatio);
            Board[rowInner, colInner] = innerInstance;
        }
    }
    private void SetEmptySlots()
    {
        if (Board == null)
            return;

        int rowStart = 0;
        int rowEnd = Board.GetLength(0) - 1;
        int colStart = 0;
        int colEnd = Board.GetLength(1) - 1;

        float resizeRatio = SpawnerTools.DEFAULT_ROW_COLUMN_COUNT / Board.GetLength(0);
        List<Coordinate> emptySlots = new List<Coordinate>();
        var availablePiecePrefabs = piecePrefabs.FindAll(x => x.TileType == TileType.TWO_IN || x.TileType == TileType.TWO_OUT);

        // Add empty slots on the top row
        for (int col = colStart; col <= colEnd; col++)
        {
            if (Board[rowStart, col] == null)
            {
                emptySlots.Add(new Coordinate(rowStart, col));
            }
        }

        // Add empty slots on the right column
        for (int row = rowStart + 1; row <= rowEnd; row++)
        {
            if (Board[row, colEnd] == null)
            {
                emptySlots.Add(new Coordinate(row, colEnd));
            }
        }

        // Add empty slots on the bottom row
        for (int col = colEnd - 1; col >= colStart; col--)
        {
            if (Board[rowEnd, col] == null)
            {
                emptySlots.Add(new Coordinate(rowEnd, col));
            }
        }

        // Add empty slots on the left column
        for (int row = rowEnd - 1; row > rowStart; row--)
        {
            if (Board[row, colStart] == null)
            {
                emptySlots.Add(new Coordinate(row, colStart));
            }
        }

        for (int i = 0; i < emptySlots.Count; i++)
        {
            var possibleEdges = Board.GetPossibleEdges(emptySlots[i].rowIndex, emptySlots[i].colIndex);
            var pieceInstance = Instantiate(availablePiecePrefabs[i % 2], pieceParent);
            pieceInstance.RotateIfNeeded(possibleEdges);
            pieceInstance.SetPiece(levelSprite, emptySlots[i].rowIndex, emptySlots[i].colIndex, resizeRatio);

            Board[emptySlots[i].rowIndex, emptySlots[i].colIndex] = pieceInstance;
        }

    }
    private void SetWholePuzzleImages()
    {
        var position = Board.GetCenterPositionOfBoard();

        Board.Iterate((int rowIndex,int columnIndex) =>
        {
            Piece piece = Board[rowIndex, columnIndex];
            piece.SetWholePuzzleImagePosition(position);
        });
    }
    private void AlignCenter()
    {
        var centerPositionOfBoard = Board.GetCenterPositionOfBoard();

        centeredObject.position = centerPositionOfBoard;
        pieceParent.SetParent(centeredObject);
        centeredObject.position = new Vector3(0, 1,0);
    }

    #endregion

    private void LevelSelectionPanel_OnLevelSelected(Sprite levelSprite, int boardSize)
    {
        this.levelSprite = levelSprite;
        this.boardSize = boardSize;
        int length = (int)Mathf.Sqrt(boardSize);
        CreateBoard(length);
    }


}
