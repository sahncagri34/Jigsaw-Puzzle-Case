using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GamePanel : BasePanel
{
    [Header("Parents")]
    [SerializeField] Transform pieceParent;
    [SerializeField] RectTransform centeredObject;
    [SerializeField] RectTransform bottomLayout;

    [Header("GUI")]
    [SerializeField] Button playButton;
    [SerializeField] GameObject backGround;
    [SerializeField] Button restartButton;
    [SerializeField] Button tipsButton;

    private Piece[,] Board;
    private List<Piece> piecePrefabs;

    private Sprite levelSprite;
    private int boardSize;

    public static event Action<Piece[,]> OnBoardReady;

    private void Awake()
    {
        piecePrefabs = GameData.Instance.GetPiecePrefabs();
        playButton.onClick.AddListener(MovePiecesToBottom);
        restartButton.onClick.AddListener(OnRestartButtonClicked);
        tipsButton.onClick.AddListener(OnTipsButtonClicked);
    }



    public void SetLevel(Sprite levelSprite, int boardSize)
    {
        this.levelSprite = levelSprite;
        backGround.GetComponentInChildren<Image>().sprite = levelSprite;
        this.boardSize = boardSize;
        int length = (int)Mathf.Sqrt(boardSize);

        CreateBoard(length);
        
        playButton.gameObject.SetActive(true);
    }

    private void CreateBoard(int length)
    {
        Board = new Piece[length, length];
        //Spawn Corners. We need to rotate the corner piece according to index of it
        SetCorners();

        //Spawn Inners
        SetInnerSlots();

        // Used Algortyhm to fill the empty slots.
        SetEmptySlots();

        //SetSubPuzzleSpriteObjectThatExistInEachPiece
        SetSubPuzzleImages();

        //AlignCenter
        AlignCenter();
        
        //SetInitialVariablesOfPieces
        SetInitialVariables();

        
        //Wait for Pieces to placed to bottom
        // MovePiecesToBottom();
        // yield return MovePiecesToBottom();
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
            cornerInstance.SetPiece(levelSprite, rowCorner, colCorner, resizeRatio);
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
                innerInstance.Rotate();

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
    private void SetSubPuzzleImages()
    {
        var position = Board.GetCenterPositionOfBoard();

        Board.Iterate((int rowIndex, int columnIndex) =>
        {
            Piece piece = Board[rowIndex, columnIndex];
            piece.SetSubPuzzleImagePosition(position);
        });
    }
    private void AlignCenter()
    {
        var centerPositionOfBoard = Board.GetCenterPositionOfBoard();

        centeredObject.position = centerPositionOfBoard;
        pieceParent.SetParent(centeredObject);
        centeredObject.localPosition = Vector3.zero;
        backGround.transform.position = centeredObject.parent.position;
    }
    private void SetInitialVariables()
    {
        Board.Iterate((int rowIndex,int colIndex) =>{
            Piece piece = Board[rowIndex, colIndex];
            piece.SetInitialVariables();
        });
    }
    private void MovePiecesToBottom()
    {
        var shuffledBoard = Board.Shuffle();

        shuffledBoard.Iterate((int rowIndex, int colIndex) =>
        {
            Piece piece = shuffledBoard[rowIndex, colIndex];
            piece.SetParent(bottomLayout,Vector2.one);
        });
        
        OnBoardReady?.Invoke(Board);
    }

    private void OnRestartButtonClicked()
    {
        SceneManager.LoadScene(0);
    }
    private void OnTipsButtonClicked()
    {
        var isActive = backGround.activeSelf;

        backGround.gameObject.SetActive(!isActive);
    }
}
