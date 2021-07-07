using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[ExecuteInEditMode]
public class Solver : MonoBehaviour
{
    public float timeStep = 0.5f;

    //bool isCollapsed = false;
    Tilemap outputTilemap;
    Dictionary<TileBase, Neighbours> tilesDictionary;
    List<TileBase> allTiles;

    int outputSize;
    Cell[,] outputCells;  //Array that holds the cells

    Sprite startSprite;




    public void Init(InputReader inputReader, Tilemap outputTilemap, int size, Sprite bottomLeftSprite)
    {
        //Init dictionary for solver
        tilesDictionary = new Dictionary<TileBase, Neighbours>();
        tilesDictionary = inputReader.GetTilesDictionary();

        //Init all tiles list
        allTiles = new List<TileBase>();
        foreach (var pair in tilesDictionary)
            allTiles.Add(pair.Key);


        //Init output tilemap
        this.outputTilemap = outputTilemap;

        //Init start sprite
        startSprite = bottomLeftSprite;

        //Create cells
        outputSize = size;
        InitializeCells(size);

        //Solving
        StartCoroutine(Solve(tilesDictionary));
    }

    private void InitializeCells(int size)
    {
        outputCells = new Cell[size, size];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                outputCells[x, y] = new Cell(position, allTiles, outputSize);
            }
        }
    }

    private IEnumerator Solve(Dictionary<TileBase, Neighbours> tilesDictionary)
    {

        Stack<Cell> cellStack = new Stack<Cell>();

        //Set bottom left sprite as start tile
        TileBase startTile = null;
        foreach (var item in tilesDictionary)
        {
            if (item.Key.name == startSprite.name)
            {
                startTile = item.Key;
                break;
            }
        }
        outputCells[0, 0].SetTile(startTile, outputTilemap);
        cellStack.Push(outputCells[0, 0]);


        Cell currentCell;
        while (cellStack.Count > 0)
        {
            //Get current cell
            currentCell = cellStack.Peek();

            //Set neighbours possible tiles of the cell
            if (currentCell.neighbours.hasTop)
                RemoveImpossibleTilesFromCell(currentCell.tile, currentCell.neighbours.top, "TOP");
            if (currentCell.neighbours.hasDown)
                RemoveImpossibleTilesFromCell(currentCell.tile, currentCell.neighbours.down, "DOWN");
            if (currentCell.neighbours.hasLeft)
                RemoveImpossibleTilesFromCell(currentCell.tile, currentCell.neighbours.left, "LEFT");
            if (currentCell.neighbours.hasRight)
                RemoveImpossibleTilesFromCell(currentCell.tile, currentCell.neighbours.right, "RIGHT");

            yield return new WaitForSecondsRealtime(timeStep);
        }
    }


    private void RemoveImpossibleTilesFromCell(TileBase currentTile, Vector3Int neighbourPos, string direction)
    {
        if (currentTile == null)
            throw new System.Exception("Tile is null");
        if (neighbourPos.x < 0 || neighbourPos.x > outputSize || neighbourPos.y < 0 || neighbourPos.y > outputSize)
            return;

        Debug.Log(neighbourPos.x + " " + neighbourPos.y);

        List<TileBase> possibleTiles = new List<TileBase>();
        Cell cell = outputCells[neighbourPos.x, neighbourPos.y];  //Current cell

        switch (direction)
        {
            case "TOP":
                possibleTiles = tilesDictionary[currentTile].top;
                Debug.Log(possibleTiles.Count);
                outputCells[cell.neighbours.top.x, cell.neighbours.top.y].SetNeighbours(possibleTiles);
                break;

            case "DOWN":
                possibleTiles = tilesDictionary[currentTile].down;
                Debug.Log(possibleTiles.Count);
                outputCells[cell.neighbours.top.x, cell.neighbours.top.y].SetNeighbours(possibleTiles);
                break;

            case "LEFT":
                possibleTiles = tilesDictionary[currentTile].left;
                Debug.Log(possibleTiles.Count);
                outputCells[cell.neighbours.top.x, cell.neighbours.top.y].SetNeighbours(possibleTiles);
                break;

            case "RIGHT":
                possibleTiles = tilesDictionary[currentTile].right;
                Debug.Log(possibleTiles.Count);
                outputCells[cell.neighbours.top.x, cell.neighbours.top.y].SetNeighbours(possibleTiles);
                break;

            default: throw new System.Exception("Something's wrong");
        }
    }
}




//Struct to hold cell data in output tilemap
public struct Cell
{
    public bool collapsed;
    public Vector3Int position;
    public List<TileBase> possibleTilesInCell;
    public TileBase tile;

    //Neighbour positions
    public CellNeighbours neighbours;

    //float cellEntropy;

    public Cell(Vector3Int position, List<TileBase> allTiles, int size)
    {
        this.position = position;
        possibleTilesInCell = allTiles;

        //Get cell neighbour positions
        neighbours = new CellNeighbours(new Vector3Int(position.x, position.y + 1, 0),
                                        new Vector3Int(position.x, position.y - 1, 0),
                                        new Vector3Int(position.x - 1, position.y, 0),
                                        new Vector3Int(position.x + 1, position.y, 0),
                                        size);
        

        tile = null;

        collapsed = false;
        //cellEntropy = 0;
    }

    public void SetTile(TileBase tile, Tilemap tilemap)
    {
        possibleTilesInCell.Clear();
        this.tile = tile;
        collapsed = true;

        tilemap.SetTile(position, tile);

        Debug.Log("Tile set position: " + position + "  Tile: " + tile);
    }

    public void DebugData()
    {
        Debug.Log("Cell: " + position + "   Possible Tiles Count: " + possibleTilesInCell.Count);
    }
    
    public void SetNeighbours(List<TileBase> possibleTiles)
    {
        possibleTilesInCell = possibleTiles;
    }
}


public struct CellNeighbours
{
    public Vector3Int top, down, left, right;
    public bool hasTop, hasDown, hasLeft, hasRight;

    public CellNeighbours(Vector3Int top, Vector3Int down, Vector3Int left, Vector3Int right, int size)
    {
        this.top = top;
        this.down = down;
        this.left = left;
        this.right = right;

        hasTop = hasDown = hasRight = hasLeft = false;
            
        hasTop = CellIsValidPos(top, size);
        hasDown = CellIsValidPos(down, size);
        hasLeft = CellIsValidPos(left, size);
        hasRight = CellIsValidPos(right, size);
    }

    private bool CellIsValidPos(Vector3Int position, int outputSize)
    {
        if (position.x < 0 || position.x > outputSize || position.y < 0 || position.y > outputSize)
            return false;
        else
            return true;
    }
}