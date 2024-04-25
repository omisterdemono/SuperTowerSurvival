using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var mapGen = (MapGenerator)target;

        if (DrawDefaultInspector())
        {
            if (mapGen.AutoUpdate)
            {
                mapGen.GenerateMap(0);
            }
        }

        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateMap(0);
        }

        if (GUILayout.Button("Clear"))
        {
            mapGen.ClearMap();
        }
    }
}