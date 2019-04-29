using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(S_PlayerController))]
public class S_ActorInspector : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Calculate"))
        {
            S_PlayerController actor = (S_PlayerController)target;
            actor.CombatStatCalculator();
        }
    }
}

[CustomEditor(typeof(S_EnemyController))]
public class S_EnemyInspector : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Calculate"))
        {
            S_EnemyController actor = (S_EnemyController)target;
            actor.CombatStatCalculator();
        }
    }
}
