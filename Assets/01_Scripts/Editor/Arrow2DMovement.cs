#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

public class Arrows2DMovement : EditorWindow
{
    private float viewportPercentRotation = 0.1f;
    private float viewportPercentMovement = 0.01f;
    private float viewportPercentScale = 0.001f;
    private int rotationSnapAngle = 15;
    private bool checkCameraSize = false;

    private float rateValue = 10f;
    private bool rateOnOff = false;
    private bool CtrlOnOff = false;

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
        EditorGUILayout.HelpBox("이 윈도우가 열려있으면 2D씬 오브젝트는 키보드로 이동한다.", MessageType.Info);
        EditorGUILayout.HelpBox("화살표 키보드는 이동\n" +
                                "W 는 위 오브젝트\n" +
                                "S 는 아래 오브젝트\n" +
                                "A 는 Parent\n" +
                                "D 는 Child\n" +
                                "Space 는 보이기/감추기\n" +
                                "Z 는 크게 이동 On/Off\n" +
                                "Ctrl + G는 묶어서 하나로", MessageType.Info);
        // var movement = viewportPercentMovement;
        // var rotation = viewportPercentRotation;
        // var scale = viewportPercentScale;
        // var size = checkCameraSize;

        viewportPercentMovement = EditorGUILayout.FloatField("이동값", viewportPercentMovement);
        // viewportPercentRotation = EditorGUILayout.FloatField("회전값", viewportPercentRotation);
        // viewportPercentScale = EditorGUILayout.FloatField("스케일값", viewportPercentScale);
        // checkCameraSize = EditorGUILayout.Toggle("카메라비례", checkCameraSize);
        rateValue = EditorGUILayout.FloatField("Z 비율값", rateValue);
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
                    moveSelectedObjects(Vector3.right, sceneView);
                    break;
                case KeyCode.LeftArrow:
                    moveSelectedObjects(Vector3.left, sceneView);
                    break;
                case KeyCode.UpArrow:
                    moveSelectedObjects(Vector3.up, sceneView);
                    break;
                case KeyCode.DownArrow:
                    moveSelectedObjects(Vector3.down, sceneView);
                    break;
                case KeyCode.W:
                    ChangeObject(Vector3.up, sceneView);
                    break;
                case KeyCode.S:
                    ChangeObject(Vector3.down, sceneView);
                    break;
                case KeyCode.A:
                    ChangeObject(Vector3.left, sceneView);
                    break;
                case KeyCode.D:
                    if (CtrlOnOff) return;
                    ChangeObject(Vector3.right, sceneView);
                    break;
                case KeyCode.Space:
                    OnOffObjects(sceneView);
                    break;
                case KeyCode.G:
                    if (!CtrlOnOff) return;
                    GroupingObjects(sceneView);
                    break;
                case KeyCode.Z:
                    rateOnOff = true;
                    break;
                case KeyCode.LeftControl:
                    CtrlOnOff = true;
                    break;
            }
        }
        else if (currentEvent.isKey
                 && currentEvent.type == EventType.KeyUp &&
                 (currentEvent.modifiers == EventModifiers.None || currentEvent.modifiers == EventModifiers.FunctionKey))
        {
            switch (currentEvent.keyCode)
            {
                case KeyCode.Z:
                    rateOnOff = false;
                    break;
                case KeyCode.LeftControl:
                    CtrlOnOff = false;
                    break;
            }
        }
    }

    #region Selected

    private void moveSelectedObjects(Vector3 direction, SceneView sceneView)
    {
        //the step size is a percent of the scene viewport
        Vector2 cameraSize = getCameraSize(sceneView.camera);
        Vector3 step = checkCameraSize ? Vector3.Scale(direction, cameraSize) : direction;
        //choose the transformation based on the selected tool
        Action<Transform, Vector3> transform;
        switch (Tools.current)
        {
            case Tool.Rotate:
                transform = rotateObject;
                break;
            case Tool.Scale:
                transform = scaleObject;
                break;
            default:
                transform = moveObject;
                break;
        }

        //get the current scene selection and move them
        var selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.Editable | SelectionMode.ExcludePrefab);
        //apply the transformation to every selected gameObject
        for (int i = 0; i < selection.Length; i++) transform((selection[i] as GameObject).transform, step);
        //only consume the event if there was at least one gameObject selected, otherwise the camera will move as usual :)
        if (selection.Length > 0) Event.current.Use();
    }

    private Vector2 getCameraSize(Camera sceneCamera)
    {
        Vector3 topRightCorner = sceneCamera.ViewportToWorldPoint(new Vector2(1, 1));
        Vector3 bottomLeftCorner = sceneCamera.ViewportToWorldPoint(new Vector2(0, 0));
        return (topRightCorner - bottomLeftCorner);
    }

    private void rotateObject(Transform t, Vector3 rotation)
    {
        //allow undo of the rotation
        Undo.RecordObject(t, "Rotation Step");
        if (rotation.y != 0)
        {
            //move the rotation to the nearest multiple of the snap angle.
            Vector3 currentRotation = t.rotation.eulerAngles;
            if (rotation.y > 0)
            {
                currentRotation.z = (Mathf.RoundToInt(currentRotation.z) / rotationSnapAngle) * rotationSnapAngle;
                currentRotation.z += rotationSnapAngle;
            }
            else
            {
                int current = Mathf.RoundToInt(currentRotation.z);
                if (current % rotationSnapAngle == 0) currentRotation.z -= rotationSnapAngle;
                else currentRotation.z = (current / rotationSnapAngle) * rotationSnapAngle;
            }

            //set the new angle
            t.rotation = Quaternion.Euler(currentRotation);
        }
        else
        {
            t.Rotate(new Vector3(0, 0, -rotation.x) * viewportPercentRotation * (rateOnOff ? rateValue : 1f));
        }
    }

    private void scaleObject(Transform t, Vector3 scale)
    {
        //allow undo of the scale
        Undo.RecordObject(t, "Scale Step");
        t.localScale = t.localScale + scale * viewportPercentScale * (rateOnOff ? rateValue : 1f);
    }

    private void moveObject(Transform t, Vector3 movement)
    {
        //allow undo of the movements
        Undo.RecordObject(t, "Move Step");
        var value = (movement * viewportPercentMovement) * 1 / viewportPercentMovement;
        value = new Vector3((int) value.x, (int) value.y, (int) value.z);
        value *= viewportPercentMovement;
        t.position = t.position + value * (rateOnOff ? rateValue : 1f);
    }

    #endregion

    #region Change

    private void ChangeObject(Vector3 direction, SceneView sceneView)
    {
        var selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.Editable | SelectionMode.ExcludePrefab);
        //apply the transformation to every selected gameObject
        if (selection.Length != 1) return;
        var current = (selection[0] as GameObject).transform;
        GameObject target = null;
        if (direction == Vector3.up)
        {
            if (current.parent != null && current.GetSiblingIndex() > 0) target = current.parent.GetChild(current.GetSiblingIndex() - 1).gameObject;
            else target = current.gameObject;
        }
        else if (direction == Vector3.down)
        {
            if (current.parent != null && current.parent.childCount > current.GetSiblingIndex() + 1)
                target = current.parent.GetChild(current.GetSiblingIndex() + 1).gameObject;
            else target = current.gameObject;
        }
        else if (direction == Vector3.left)
        {
            if (current.parent == null) target = current.gameObject;
            else target = current.parent.gameObject;
        }
        else if (direction == Vector3.right)
        {
            if (current.childCount == 0) target = current.gameObject;
            else target = current.GetChild(0).gameObject;
        }

        Selection.objects = new Object[] {target};
    }

    #endregion

    #region 오브젝트 컨트롤

    private void OnOffObjects(SceneView sceneView)
    {
        var selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.Editable | SelectionMode.ExcludePrefab);
        //apply the transformation to every selected gameObject
        for (int i = 0; i < selection.Length; i++)
        {
            var current = (selection[i] as GameObject);
            current.SetActive(!current.activeSelf);
        }
    }

    private void GroupingObjects(SceneView sceneView)
    {
        var selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.Editable | SelectionMode.ExcludePrefab);
        Transform parent = null;
        Transform newObj = null;
        Undo.IncrementCurrentGroup();
        //apply the transformation to every selected gameObject
        var list = new List<GameObject>();
        for (int i = 0; i < selection.Length; i++)
        {
            var current = (selection[i] as GameObject);
            if (current == null) continue;
            if (i == 0)
            {
                parent = current.transform.parent;
                newObj = new GameObject(GetName(current.name)).transform;
                if (newObj != null)
                    Undo.RegisterCreatedObjectUndo (newObj.gameObject, "Created go");
                newObj.SetParent(parent);
                if (parent != null)
                    newObj.SetSiblingIndex(current.transform.GetSiblingIndex());
                
                Vector3 totalPos = Vector3.zero;
                int count = 0;
                if (selection.Length == 0) return;
                foreach (var t in selection)
                {
                    var select = (t as GameObject);
                    if (@select == null) continue;
                    totalPos += @select.transform.position;
                    count++;
                }
                if(count > 0)
                    newObj.transform.position = totalPos / count;
            }

            list.Add(current);
        }

        foreach (var item in 
            list.OrderBy(x => x.transform.GetSiblingIndex()))
        {
            Undo.SetTransformParent(item.transform, newObj, $"SetParent");
        }


        string GetName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "";
            if (name.Contains("FallenTree") || name.Contains("쓰러진나무") || name.Contains("쓰러진 나무") || name.Contains("Wood"))
            {
                return name;
            }
            else if (name.Contains("Tree") || name.Contains("나무"))
            {
                return name;
            }
            else if (name.Contains("Stump"))
            {
                return name;
            }
            else if (name.Contains("꽃") || name.Contains("Flower") || name.Contains("화분") || name.Contains("flower"))
            {
                return name;
            }
            else if (name.Contains("Bush") || name.Contains("수풀") || name.Contains("Shrub"))
            {
                return name;
            }
            else if (name.Contains("바위") || name.Contains("Rock"))
            {
                return name;
            }
            else if (name.Contains("Water") || (name.Contains("물") && !name.Contains("식물")) || name.Contains("폭포") || name.Contains("분수"))
            {
                return name;
            }
            else if (name.Contains("Leaves") || name.Contains("낙엽") || name.Contains("나뭇잎") || name.Contains("Leaf"))
            {
                return name;
            }
            else if (name.Contains("Branches") || name.Contains("나뭇가지") || name.Contains("branch"))
            {
                return name;
            }
            else if (name.Contains("Dust") || name.Contains("먼지") || name.Contains("바닥") || name.Contains("타일"))
            {
                return name;
            }
            else if (name.Contains("lamp") || name.Contains("가로등"))
            {
                return name;
            }
            else if (name.Contains("Soil") || name.Contains("흙") || name.Contains("밭"))
            {
                return name;
            }
            else if (name.Contains("풀") || name.Contains("Grass") || name.Contains("Weeds") || name.Contains("잡초")
                     || name.Contains("잔디") || name.Contains("덩굴") || name.Contains("식물") || name.Contains("방석")
                     || name.Contains("Vine"))
            {
                return name;
            }
            else if (name.Contains("의자") || name.Contains("Chair") || name.Contains("테이블") ||
                     name.Contains("Table") || name.Contains("책상") || name.Contains("돌") ||
                     name.Contains("간판") || name.Contains("파라솔") || name.Contains("Gate") ||
                     name.Contains("Wall") || name.Contains("문") || name.Contains("벽") ||
                     name.Contains("Broken") || name.Contains("Decoration") || name.Contains("Tile") ||
                     name.Contains("울타리") || name.Contains("road") || name.Contains("bench") ||
                     name.Contains("연못") || name.Contains("화로") || name.Contains("가게") ||
                     name.Contains("가판대") || name.Contains("수레") || name.Contains("지붕") ||
                     name.Contains("난간") || name.Contains("석상"))
            {
                return name;
            }
            else
                return "";
        }
    }

    #endregion
}
#endif