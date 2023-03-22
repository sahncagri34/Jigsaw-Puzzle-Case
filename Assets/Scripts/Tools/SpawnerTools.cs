using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class SpawnerTools
{
    public const int DEFAULT_ROTATION_AMOUNT = 90;
    public const float DEFAULT_ROW_COLUMN_COUNT = 6;
    public static Coordinate[] GetCornerIndexes(this Piece[,] board)
    {
        int xLength = board.GetLength(0);
        int yLength = board.GetLength(1);

        Coordinate[] corners = new Coordinate[] {
          new Coordinate(0,0),                      // Top-left corner
          new Coordinate(0,xLength - 1),            // Top-right corner
          new Coordinate(xLength - 1,yLength - 1),  // Bottom-right corner
          new Coordinate(yLength-1,0)               // Bottom-left corner
        };

        return corners;
    }
    public static Coordinate[] GetInnerIndexes(this Piece[,] Board)
    {
        int rows = Board.GetLength(0);
        int cols = Board.GetLength(1);

        List<Coordinate> innerIndexes = new List<Coordinate>();

        for (int i = 1; i < rows - 1; i++)
        {
            for (int j = 1; j < cols - 1; j++)
            {
                innerIndexes.Add(new Coordinate(i, j));
            }
        }

        return innerIndexes.ToArray();
    }

    public static List<Edge> GetPossibleEdges(this Piece[,] Board, int rowIndex, int colIndex)
    {
        List<Edge> possibleEdges = new List<Edge>();
        Piece currentPiece = Board[rowIndex, colIndex];

        if (currentPiece != null)
            return possibleEdges;

        Edge topEdge = new Edge { EdgeType = EdgesType.Top, EdgeState = EdgeStates.None };
        Edge rightEdge = new Edge { EdgeType = EdgesType.Right, EdgeState = EdgeStates.None };
        Edge bottomEdge = new Edge { EdgeType = EdgesType.Bottom, EdgeState = EdgeStates.None };
        Edge leftEdge = new Edge { EdgeType = EdgesType.Left, EdgeState = EdgeStates.None };

        // Check top neighbor
        if (rowIndex > 0)
        {
            Piece topNeighbor = Board[rowIndex - 1, colIndex];
            if (topNeighbor != null)
            {
                EdgeStates correspondingState = GetCorrespondingState(topNeighbor.GetEdgeState(EdgesType.Bottom));
                topEdge.EdgeState = correspondingState;
            }
        }
        else
            topEdge.EdgeState = EdgeStates.Flat;


        // Check right neighbor
        if (colIndex < Board.GetLength(1) - 1)
        {
            Piece rightNeighbor = Board[rowIndex, colIndex + 1];
            if (rightNeighbor != null)
            {
                EdgeStates correspondingState = GetCorrespondingState(rightNeighbor.GetEdgeState(EdgesType.Left));
                rightEdge.EdgeState = correspondingState;
            }
        }
        else
            rightEdge.EdgeState = EdgeStates.Flat;


        // Check bottom neighbor
        if (rowIndex < Board.GetLength(0) - 1)
        {
            Piece bottomNeighbor = Board[rowIndex + 1, colIndex];
            if (bottomNeighbor != null)
            {
                EdgeStates correspondingState = GetCorrespondingState(bottomNeighbor.GetEdgeState(EdgesType.Top));
                bottomEdge.EdgeState = correspondingState;
            }
        }
        else
            bottomEdge.EdgeState = EdgeStates.Flat;

        // Check left neighbor
        if (colIndex > 0)
        {
            Piece leftNeighbor = Board[rowIndex, colIndex - 1];
            if (leftNeighbor != null)
            {
                EdgeStates correspondingState = GetCorrespondingState(leftNeighbor.GetEdgeState(EdgesType.Right));
                leftEdge.EdgeState = correspondingState;
            }
        }
        else
            leftEdge.EdgeState = EdgeStates.Flat;

        possibleEdges.Add(topEdge);
        possibleEdges.Add(rightEdge);
        possibleEdges.Add(bottomEdge);
        possibleEdges.Add(leftEdge);

        return possibleEdges;
    }

    private static EdgeStates GetCorrespondingState(EdgeStates pieceEdgeState)
    {
        if (pieceEdgeState == EdgeStates.Flat)
            return EdgeStates.Flat;
        if (pieceEdgeState == EdgeStates.In)
            return EdgeStates.Out;
        if (pieceEdgeState == EdgeStates.Out)
            return EdgeStates.In;

        return EdgeStates.None;
    }

    public static Vector3 GetCenterPositionOfBoard(this Piece[,] board)
    {
        int rowCount = board.GetLength(0);
        int colCount = board.GetLength(1);

        int centerRow = rowCount / 2 - 1;
        int centerCol = colCount / 2 - 1;

        Piece prevCenterPiece = board[centerRow, centerCol];
        Piece afterCenterPiece = board[centerRow + 1, centerCol + 1];
        return (prevCenterPiece.transform.position + afterCenterPiece.transform.position) / 2;
    }
    public static void Iterate(this Piece[,] Board,Action<int,int> OnIterate)
    {
        int rowCount = Board.GetLength(0);
        int colCount = Board.GetLength(1);

        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < colCount; columnIndex++)
            {
                OnIterate?.Invoke(rowIndex, columnIndex);
            }
        }
    }


}
[System.Serializable]
public class Coordinate
{
    public int rowIndex;
    public int colIndex;

    public Coordinate(int _rowIndex,int _colIndex)
    {
        rowIndex = _rowIndex;
        colIndex = _colIndex;
    }
}
