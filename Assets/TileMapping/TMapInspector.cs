using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GraphicsTilesMap))]
public class TMapInspector : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Regenerate"))
        {
            GraphicsTilesMap tmap = (GraphicsTilesMap)target;
            tmap.BuildMesh();
        }
    }
}
