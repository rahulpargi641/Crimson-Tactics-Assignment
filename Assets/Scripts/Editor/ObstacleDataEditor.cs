using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObstacleData))]
public class ObstacleDataEditor : Editor
{
    private const int gridSize = 10;
    private const int buttonSize = 20;

    public override void OnInspectorGUI()
    {
        ObstacleData obstacleData = (ObstacleData)target;

        EditorGUILayout.LabelField("Obstacle Grid", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();

        for (int row = 0; row < gridSize; row++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int col = 0; col < gridSize; col++)
            {
                obstacleData.obstacleGrid[row, col] = EditorGUILayout.Toggle(obstacleData.obstacleGrid[row, col], GUILayout.Width(buttonSize), GUILayout.Height(buttonSize));
            }
            EditorGUILayout.EndHorizontal();
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(obstacleData);
        }
    }
}
