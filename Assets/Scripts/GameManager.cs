using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    private List<Vector3> piecePositions = new List<Vector3>();
    private List<Piece> correctlyPlacedPieces = new List<Piece>();

    private int boardSize;
    public override void Awake()
    {
        GamePanel.OnBoardReady += GamePanel_OnBoardReady;
        base.Awake();
    }


    public Vector3 GetClosestPosition(Vector3 position)
    {
        Vector3 closestPosition = Vector3.zero;
        float closestDistance = Mathf.Infinity;

        foreach (Vector3 piecePosition in piecePositions)
        {
            float distance = Vector3.Distance(position, piecePosition);
            if (distance < closestDistance)
            {
                closestPosition = piecePosition;
                closestDistance = distance;
            }
        }

        return closestPosition;
    }

    private void GamePanel_OnBoardReady(Piece[,] Board)
    {
        boardSize = Board.GetLength(0) * Board.GetLength(1);
        Board.Iterate((int rowIndex, int colIndex) =>
        {
            var piece = Board[rowIndex, colIndex];
            piece.SetDraggerActive();
            piecePositions.Add(piece.initialPosition);

            piece.OnPlacedCorrectly += Piece_OnPlacedCorrectly;
        });
    }
    private void Piece_OnPlacedCorrectly(Piece piece)
    {
        piece.OnPlacedCorrectly -= Piece_OnPlacedCorrectly;

        if (correctlyPlacedPieces.Contains(piece))
            return;

        correctlyPlacedPieces.Add(piece);
        piecePositions.Remove(piece.initialPosition);

        CheckWinState();
    }

    private void CheckWinState()
    {
        var lengthOfCorrectlyPlacedObjects = correctlyPlacedPieces.Count;
        
        if(lengthOfCorrectlyPlacedObjects >= boardSize)
        {
            UIManager.Instance.PushPanel<WinPanel>();
        }
    }
}
