using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    [SerializeField] List<Piece> piecePrefabs;
    [SerializeField] Transform pieceParent;

    private Piece[,] Board;

    private void Start()
    {
        CreateBoard(6, 6);
    }

    private void CreateBoard(int rowCount, int colCount)
    {
        Board = new Piece[rowCount, colCount];

        //Spawn Corners. We need to rotate the corner piece according to index of it
        SetCorners();

        //Spawn Inners
        SetInnerSlots();

        // Used Algortyhm to fill the empty slots.
        SetEmptySlots();
    }

    #region Spawn Functions

    private void SetCorners()
    {
        var cornerPiecePrefab = piecePrefabs.Find(x => x.TileType == TileType.TWO_FLAT);
        Coordinate[] cornerIndexes = Board.GetCornerIndexes();
        for (int i = 0; i < cornerIndexes.Length; i++)
        {
            var cornerInstance = Instantiate(cornerPiecePrefab, pieceParent);
            cornerInstance.Rotate(i);

            int rowCorner = cornerIndexes[i].rowIndex;
            int colCorner = cornerIndexes[i].colIndex;
            cornerInstance.SetSpawnPosition(rowCorner, colCorner);
            Board[rowCorner, colCorner] = cornerInstance;
        }
    }
    private void SetInnerSlots()
    {
        var innerPiecePrefab = piecePrefabs.Find(x => x.TileType == TileType.NO_FLAT);
        Coordinate[] innerIndexes = Board.GetInnerIndexes();

        for (int i = 0; i < innerIndexes.Length; i++)
        {
            var innerInstance = Instantiate(innerPiecePrefab, pieceParent);

            int rowInner = innerIndexes[i].rowIndex;
            int colInner = innerIndexes[i].colIndex;

            var modeOfCol = colInner % 2;
            int modeOfRow = rowInner % 2;

            if ((modeOfRow == 0 && modeOfCol == 1) || (modeOfRow == 1 && modeOfCol == 0))
                innerInstance.Rotate(1);

            innerInstance.SetSpawnPosition(rowInner, colInner);
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
            pieceInstance.SetSpawnPosition(emptySlots[i].rowIndex, emptySlots[i].colIndex);
            Board[emptySlots[i].rowIndex, emptySlots[i].colIndex] = pieceInstance;
        }

    }


    #endregion

    
}
