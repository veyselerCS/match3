using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;

[InitializeOnLoad]
public class EditorLayout
{
    static EditorLayout()
    {
        ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
    }
    
    static void OnToolbarGUI()
    {
        GUILayout.FlexibleSpace();
        
        if (GUILayout.Button(new GUIContent("Init", "Switch to init scene.")))
        {
            bool saved = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            if (!saved)
                return;
            EditorSceneManager.OpenScene("Assets/Scenes/InitScene.unity");
        }
        
        if (GUILayout.Button(new GUIContent("Puzzle", "Switch to Puzzle scene.")))
        {
            bool saved = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            if (!saved)
                return;
            EditorSceneManager.OpenScene("Assets/Scenes/PuzzleScene.unity");
        } 
    }
}