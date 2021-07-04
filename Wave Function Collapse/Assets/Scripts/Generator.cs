using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Generator : MonoBehaviour
{
    public Tilemap inputTilemap;
    InputReader reader;

    private void Start()
    {
        reader = new InputReader(inputTilemap, false);
    }
}