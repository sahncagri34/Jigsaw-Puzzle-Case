using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public TileType TileType;
    public List<Edge> Edges;

    public SpriteRenderer wholePuzzle;
    
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

    internal void SetSpawnPosition(int rowIndex, int colIndex,float scaleAndPositionRatio)
    {
        transform.localScale = transform.localScale * scaleAndPositionRatio;
        wholePuzzle.transform.localScale = wholePuzzle.transform.localScale / scaleAndPositionRatio;
        transform.localPosition = GetCalculatedPosition(rowIndex,colIndex,scaleAndPositionRatio);
        wholePuzzle.transform.rotation = Quaternion.identity;
    }
    public void SetWholePuzzleImagePosition(Vector3 puzzleImagePosition)
    {
        wholePuzzle.transform.position = puzzleImagePosition;
    }
    public Vector2 GetCalculatedPosition(int row,int col, float scaleAndPositionRatio)
    {
       return new Vector2(-2.07f + (scaleAndPositionRatio * 0.74f * col), 3.217f - (scaleAndPositionRatio * 0.747f * row));
    }
    #endregion
}
