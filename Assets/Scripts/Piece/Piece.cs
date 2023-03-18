using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public TileType TileType;
    public List<Edge> Edges;

    #region Spawn

    #endregion

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
        for (int i = 0; i < 3; i++)
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

    internal void SetSpawnPosition(int row, int col)
    {
        int index = row * 6 + col;
        transform.position = new Vector2(2*col, -2*row);
    }
}
