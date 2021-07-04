using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InputReader
{
    private Tilemap inputMap;
    private TileBase[] allTiles;
    private List<MyTile> allMyTiles;
    private Dictionary<TileBase, Neighbours> tilesDictionary;
    private bool isReady = false;


    public InputReader(Tilemap inputTilemap, bool debugNeighbours = false)
    {
        this.inputMap = inputTilemap;
        
        //Init all fields
        allMyTiles = new List<MyTile>();
        tilesDictionary = new Dictionary<TileBase, Neighbours>();

        GetInputTiles();

        //Create dictionary for unique tiles
        foreach (var myTile in allMyTiles)
            if (!tilesDictionary.ContainsKey(myTile.tile))
                tilesDictionary.Add(myTile.tile, GetAllTileNeigbours(myTile));


        if (debugNeighbours)
        {
            Debug.Log("Total My Tiles: " + allMyTiles.Count);
            Debug.Log("Dict size: " + tilesDictionary.Count);
            
            Debug.Log("");
            
            foreach (var item in tilesDictionary)
                item.Value.Debug(item.Key);
        }

        isReady = true;
    }

    private void GetInputTiles()
    {
        BoundsInt bounds = inputMap.cellBounds;
        

        //Get all tiles
        allTiles = inputMap.GetTilesBlock(bounds);

        foreach (var pos in inputMap.cellBounds.allPositionsWithin)
        {
            Vector3Int cellNumber = new Vector3Int(pos.x, pos.y, pos.z);  //Cell Number
            Vector3 place = inputMap.CellToWorld(cellNumber);  //World position

            if (inputMap.HasTile(cellNumber))
            {
                //Create new MyTile and set neighbours
                TileBase tile = inputMap.GetTile(cellNumber);
                MyTile myTile = new MyTile(tile, cellNumber);

                //Get neighbours of tile
                myTile.SetNeighbours(inputMap);

                //Add tile to list of tiles
                allMyTiles.Add(myTile);
            }
        }
    }

    private Neighbours GetAllTileNeigbours(MyTile myTile)
    {
        Neighbours tileNeighbours = new Neighbours();
        bool firstTime = true;
        foreach (var item in allMyTiles)
        {
            //Check if tiles match
            if (myTile.tile == item.tile)
            {
                if (firstTime)
                {
                    firstTime = false;

                    tileNeighbours = item.neighbours;
                }
                else
                {
                    //Get the neighbours of tile from combined list and merge
                    tileNeighbours.top = tileNeighbours.top.Union(item.neighbours.top).ToList();
                    tileNeighbours.down = tileNeighbours.down.Union(item.neighbours.down).ToList();
                    tileNeighbours.left = tileNeighbours.left.Union(item.neighbours.left).ToList();
                    tileNeighbours.right = tileNeighbours.right.Union(item.neighbours.right).ToList();
                }
            }
        }

        return tileNeighbours;
    }

    public Dictionary<TileBase, Neighbours> GetTilesDictionary()
    {
        if (isReady)
            return tilesDictionary;
        else
            throw new System.Exception("Dictionary not ready");
    }
}
