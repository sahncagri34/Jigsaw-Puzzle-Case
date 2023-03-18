using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Edge
{
    public EdgesType EdgeType;
    public EdgeStates EdgeState;
}
public enum TileType
{
    TWO_FLAT,
    NO_FLAT,
    TWO_OUT,
    TWO_IN,
}
public enum EdgesType
{
    Top,
    Right,
    Left,
    Bottom
}
public enum EdgeStates
{
    None,
    Flat,
    In,
    Out
}