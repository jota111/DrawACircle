#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MapTestViewer : EditorWindow
{

    [MenuItem("Window/Scene 2DObject Control")]
    static void Open()
    {
        Arrows2DMovement win = GetWindow<Arrows2DMovement>();
        win.titleContent = new GUIContent("Scene 2DObject Control");
        win.Show();
    }

    void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneView;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneView;
    }

    void OnGUI()
    {
        EditorGUILayout.HelpBox("이 윈도우는 맵 구역이 선택됐을 때 작동합니다.", MessageType.Info);
        EditorGUILayout.HelpBox("이 윈도우는 맵 구역이 선택됐을 때 작동합니다.", MessageType.Info);
        EditorGUILayout.HelpBox("화살표 키보드는 이동", MessageType.Info);
    }

    void OnSceneView(SceneView sceneView)
    {
        Event currentEvent = Event.current;
        //if the event is a keyDown on an orthographic camera
        if (currentEvent.isKey
            && currentEvent.type == EventType.KeyDown
            && (currentEvent.modifiers == EventModifiers.None || currentEvent.modifiers == EventModifiers.FunctionKey ||
                currentEvent.modifiers == EventModifiers.Control) //arrow keys are function keys
            && sceneView.camera.orthographic)
        {
            //choose the right direction to move
            switch (currentEvent.keyCode)
            {
                case KeyCode.RightArrow:
                case KeyCode.LeftControl:
                    break;
            }
        }
        else if (currentEvent.isKey
                 && currentEvent.type == EventType.KeyUp &&
                 (currentEvent.modifiers == EventModifiers.None || currentEvent.modifiers == EventModifiers.FunctionKey))
        {
            switch (currentEvent.keyCode)
            {
                
            }
        }
    }
}
#endif