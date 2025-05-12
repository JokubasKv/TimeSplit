using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonMaker))]
public class DungeonMakerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DungeonMaker dungeonCreator = (DungeonMaker)target;
        if (GUILayout.Button("CreateNewDungeon"))
        {
            dungeonCreator.CreateDungeon();
        }
    }
}
