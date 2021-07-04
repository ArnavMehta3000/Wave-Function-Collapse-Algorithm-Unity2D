using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MyTile
{
    public TileBase tile;
    public Neighbours neighbours;
    public Vector3Int cellPosition;

    //Neighbours to this tile
    TileBase top, down, left, right;


    public MyTile(TileBase tile, Vector3Int cellPosition)
    {
        this.tile = tile;
        this.cellPosition = cellPosition;
    }

    public void SetNeighbours(Tilemap inputMap)
    {
        Vector3Int
            LEFT = new Vector3Int(-1, 0, 0),
            RIGHT = new Vector3Int(1, 0, 0),
            TOP = new Vector3Int(0, 1, 0),
            DOWN = new Vector3Int(0, -1, 0);

        top = inputMap.GetTile(cellPosition + TOP);
        down = inputMap.GetTile(cellPosition + DOWN);
        left = inputMap.GetTile(cellPosition + LEFT);
        right = inputMap.GetTile(cellPosition + RIGHT);

        neighbours = new Neighbours(top, down, left, right); 
        neighbours = RemoveNull(neighbours);
    }

    public void SetNeighbours(MyTile4 neighbourTiles)
    {
        top = neighbourTiles.top;
        down = neighbourTiles.down;
        left = neighbourTiles.left;
        right = neighbourTiles.right;

        neighbours = new Neighbours(top, down, left, right);
        neighbours = RemoveNull(neighbours);
    }

    public Neighbours RemoveNull(Neighbours n)
    {
        n.top.RemoveAll(item => item == null);
        n.down.RemoveAll(item => item == null);
        n.left.RemoveAll(item => item == null);
        n.right.RemoveAll(item => item == null);

        return n;
    }
}

public struct MyTile4
{
    public TileBase top, down, left, right;

    public MyTile4(TileBase top, TileBase down, TileBase left, TileBase right)
    {
        this.top = top;
        this.right = right;
        this.left = left;
        this.down = down;
    }
}

public struct Neighbours
{
    public List<TileBase> top, down, left, right;

    public Neighbours(List<TileBase> top, List<TileBase> down, List<TileBase> left, List<TileBase> right)
    {
        this.top = top;
        this.down = down;
        this.left = left;
        this.right = right;
    }

    public Neighbours(TileBase top = null, TileBase down = null, TileBase left = null, TileBase right = null)
    {
        this.top = new List<TileBase>();
        this.down = new List<TileBase>();
        this.left = new List<TileBase>();
        this.right = new List<TileBase>();

        this.top.Add(top);
        this.down.Add(down);
        this.left.Add(left);
        this.right.Add(right);
    }


    public void Debug(TileBase tile)
    {
        UnityEngine.Debug.Log("-----Tile: " + tile + "-----");

        UnityEngine.Debug.Log("Top Neighbours : " + top.Count);
        foreach (var item in top)
            UnityEngine.Debug.Log(item);
        UnityEngine.Debug.Log("");

        UnityEngine.Debug.Log("Down Neighbours: " + down.Count);
        foreach (var item in down)
            UnityEngine.Debug.Log(item);
        UnityEngine.Debug.Log("");

        UnityEngine.Debug.Log("Left Neighbours: " + left.Count);
        foreach (var item in left)
            UnityEngine.Debug.Log(item);
        UnityEngine.Debug.Log("");

        UnityEngine.Debug.Log("Right Neighbours: " + right.Count);
        foreach (var item in right)
            UnityEngine.Debug.Log(item);
        UnityEngine.Debug.Log("");

        UnityEngine.Debug.Log("----------");

        UnityEngine.Debug.Log("");
    }
}