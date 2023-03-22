using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Piece : MonoBehaviour
{
    public TileType TileType;
    public List<Edge> Edges;

    public Image wholePuzzle;

    private Vector2 initialScale;


    private void Awake() {
        initialScale = transform.localScale;
    }

    
    #region Spawn

    public void Rotate(int rotateTimes)
    {
        //Rotated 90 degree each time, Changed Edge States According to rotation
     
        for (int i = 0; i < rotateTimes; i++)
        {
            var first = Edges[0].EdgeState;
            var sec = Edges[1].EdgeState;
            var thrd = Edges[2].EdgeState;
            var last = Edges[3].EdgeState;

            Edges[0].EdgeState = last;
            Edges[1].EdgeState = first;
            Edges[2].EdgeState = sec;
            Edges[3].EdgeState = thrd;
        }

        transform.Rotate(0, 0, rotateTimes*-SpawnerTools.DEFAULT_ROTATION_AMOUNT);
    }
    public EdgeStates GetEdgeState(EdgesType edgesType)
    {
        var edge = Edges.Find(x => x.EdgeType == edgesType);
        return edge.EdgeState;
    }

    public void RotateIfNeeded(List<Edge> possibleEdges)
    {
        //Rotated 90 degree each time
        for (int i = 0; i < 4; i++)
        {
            var isSame = CompareEdges(possibleEdges);
            if (isSame)
            {
                break;
            }
            Rotate(1);
        }
    }
    private bool CompareEdges(List<Edge> possibleEdges)
    {
        foreach (var edge in Edges)
        {
            var possibleEdge = possibleEdges.Find(x => x.EdgeType == edge.EdgeType);

            if (possibleEdge.EdgeState == EdgeStates.None)
                continue;

            if (possibleEdge.EdgeState != edge.EdgeState)
                return false;

        }
        return true;
    }

    internal void SetPiece(Sprite puzzleImage,int rowIndex, int colIndex,float scaleAndPositionRatio)
    {
        wholePuzzle.sprite = puzzleImage;

         transform.localScale = transform.localScale * scaleAndPositionRatio;
         wholePuzzle.transform.localScale = wholePuzzle.transform.localScale / scaleAndPositionRatio;
        transform.localPosition = GetCalculatedPosition(rowIndex, colIndex, scaleAndPositionRatio);
        wholePuzzle.transform.rotation = Quaternion.identity;
    }
    
    public void SetWholePuzzleImagePosition(Vector3 puzzleImagePosition)
    {
        wholePuzzle.transform.position = puzzleImagePosition;
    }
    public Vector2 GetCalculatedPosition(int row,int col, float scaleAndPositionRatio)
    {
       return new Vector2(-435f + (scaleAndPositionRatio * 171 * col), 401 - (scaleAndPositionRatio *160* row));
    }
    
    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
        parent.localScale = Vector2.one;
    }
    #endregion
}
