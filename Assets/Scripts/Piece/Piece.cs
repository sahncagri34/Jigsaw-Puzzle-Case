using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Piece : MonoBehaviour
{
    public TileType TileType;
    public List<Edge> Edges;

    public Image subPuzzle;

    public Vector3 initialPosition;
    public Vector2 initialScale;
    private Transform initialParent;

    private Dragger dragger;

    public event Action<Piece> OnPlacedCorrectly;

    private void Awake() {
        dragger = GetComponent<Dragger>();

        dragger.OnReleasedObject += Dragger_OnReleasedObject;
        dragger.OnGrabbedObject += Dragger_OnGrabbedObject;
    }
    private void OnDestroy() {
        dragger.OnReleasedObject -= Dragger_OnReleasedObject;
        dragger.OnGrabbedObject -= Dragger_OnGrabbedObject;
    }

    public void SetDraggerActive()
    {
        dragger.SetDraggerActive();
    }

    private void Dragger_OnReleasedObject(Vector3 releasedPosition)
    {
        Vector3 newPosition = SetClosestPosition(releasedPosition);

        if (CheckIfPositionCorrect(newPosition))
        {
            dragger.canMove = false;
            OnPlacedCorrectly?.Invoke(this);
        }
    }
    private void Dragger_OnGrabbedObject()
    {
        SetParent(initialParent,initialScale);
    }
    private Vector3 SetClosestPosition(Vector3 targetPos)
    {
        SetParent(initialParent,initialScale);

        Vector3 position = GameManager.Instance.GetClosestPosition(targetPos);
        transform.position = position;

        return position;
    }
    private bool CheckIfPositionCorrect(Vector3 targetPosition)
    {
        return Vector3.Distance(initialPosition, targetPosition) < 1f;
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

    public void SetPiece(Sprite puzzleImage, int rowIndex, int colIndex, float scaleAndPositionRatio)
    {
        subPuzzle.sprite = puzzleImage;

        transform.localScale = transform.localScale * scaleAndPositionRatio;

        transform.localPosition = GetCalculatedPosition(rowIndex, colIndex, scaleAndPositionRatio);

        subPuzzle.transform.localScale = subPuzzle.transform.localScale / scaleAndPositionRatio;
        subPuzzle.transform.rotation = Quaternion.identity;
    }
    
    public void SetSubPuzzleImagePosition(Vector3 puzzleImagePosition)
    {
        subPuzzle.transform.position = puzzleImagePosition;
    }
    public Vector2 GetCalculatedPosition(int row,int col, float scaleAndPositionRatio)
    {
       return new Vector2(-435f + (scaleAndPositionRatio * 171 * col), 401 - (scaleAndPositionRatio *160* row));
    }
    public void SetInitialVariables()
    {
        initialParent = transform.parent;
        initialPosition = transform.position;
        initialScale = transform.localScale;
    }
    public void SetParent(Transform parent,Vector2 scale)
    {
        transform.localScale = scale;
        transform.SetParent(parent);
    }
    #endregion

}
