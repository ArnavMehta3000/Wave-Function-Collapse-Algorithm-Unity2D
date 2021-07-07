using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System;


[ExecuteInEditMode]
public class Generator : MonoBehaviour
{
    [Header("Tilemap Data")]
    public Tilemap inputTilemap;  //Tilemap taken for input reading
    public Tilemap outputTileMap;  //Tilemap where the output is shown

    [Header("Output Data")]
    public Sprite bottomLeftSprite;
    public int outputSize = 10;  //Output grid square size

    private InputReader reader = null;  //Input reader object
    [SerializeField] private Solver solver = null;  //WFC solver



    public void Generate()
    {
        //Prepare reader 
        reader = new InputReader(inputTilemap, false);

        //Prepare solver
        solver.Init(reader, outputTileMap, outputSize, bottomLeftSprite);
    }

    internal void ClearOutput()
    {
        //Reset output tilemap
        outputTileMap.ClearAllTiles();
        outputTileMap.CompressBounds();
    }
}


[CustomEditor(typeof(Generator))]
public class GeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        Generator generator = (Generator)target;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate"))
            generator.Generate();

        if (GUILayout.Button("Reset"))
            generator.ClearOutput();

        GUILayout.EndHorizontal();
    }


}