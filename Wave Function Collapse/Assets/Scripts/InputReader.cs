using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;


//Class to read the input from the tile map and find all neighbours for all unique tiles
public class InputReader
{
    private Tilemap inputMap;  //Input tilemap
    private TileBase[] allTiles;  //Array for storing all tiles from input map (incl. duplicates)
    private List<MyTile> allMyTiles;  //Array for storing all tiles in MyTile format (incl duplicates)
    private Dictionary<TileBase, Neighbours> tilesDictionary;  //Dictionary to store tile and neighbours as key-value pairs
    private bool isReady = false;  //Get dictionary check


    public InputReader(Tilemap inputTilemap, bool debugNeighbours = false)
    {
        //Init all fields
        this.inputMap = inputTilemap;
        allMyTiles = new List<MyTile>();
        tilesDictionary = new Dictionary<TileBase, Neighbours>();

        //Read raw input
        GetInputTiles();

        //Create dictionary for unique tiles by comparing tiles (/) keys
        foreach (var myTile in allMyTiles)
            if (!tilesDictionary.ContainsKey(myTile.tile))
                tilesDictionary.Add(myTile.tile, GetAllTileNeigbours(myTile));

        //Debug if needed
        if (debugNeighbours)
        {
            Debug.Log("Total My Tiles: " + allMyTiles.Count);
            Debug.Log("Dict size: " + tilesDictionary.Count);

            Debug.Log("");

            foreach (var item in tilesDictionary)
                item.Value.Debug(item.Key);
        }

        isReady = true;
        Debug.Log("Input Read Successfull! (# of unique tiles: " + tilesDictionary.Count + ")");
    }

    private void GetInputTiles()
    {
        //Get tilemap bounds
        BoundsInt bounds = inputMap.cellBounds;

        //Get all tiles
        allTiles = inputMap.GetTilesBlock(bounds);

        //Loop through all tiles withing tilemap bounds
        foreach (var pos in inputMap.cellBounds.allPositionsWithin)
        {
            Vector3Int cellNumber = new Vector3Int(pos.x, pos.y, pos.z);  //Cell Number
            Vector3 place = inputMap.CellToWorld(cellNumber);  //World position

            //Tile found -> save it
            if (inputMap.HasTile(cellNumber))
            {
                //Create new MyTile
                TileBase tile = inputMap.GetTile(cellNumber);
                MyTile myTile = new MyTile(tile, cellNumber);

                //Get neighbours of tile
                myTile.SetNeighbours(inputMap);

                //Add mytTile to list of MyTiles
                allMyTiles.Add(myTile);
            }
        }
    }

    private Neighbours GetAllTileNeigbours(MyTile myTile)
    {
        Neighbours tileNeighbours = new Neighbours();
        bool firstTime = true;

        //Loop through all MyTiles (which contains duplicates)
        foreach (var item in allMyTiles)
        {
            //Check if tiles match
            if (myTile.tile == item.tile)
            {
                //If match found for the first time, then directly copy neighbours from MyTile
                if (firstTime)
                {
                    firstTime = false;
                    tileNeighbours = item.neighbours;
                }
                //If match found (not first time), then Union the two lists to remove duplicates
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
        if (isReady && tilesDictionary.Count > 0)  //Only return dict if it has anything
            return tilesDictionary;
        else
            throw new System.Exception("Dictionary not ready or empty (Count: " + tilesDictionary.Count + ")");
    }
}
