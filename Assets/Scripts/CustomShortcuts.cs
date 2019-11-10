using UnityEngine;
using UnityEditor;

public class CustomShortcuts : EditorWindow {

    [MenuItem("GameObject/Quick Reset Transform %r")]
    private static void ResetTransform() {
        foreach(Transform transform in Selection.transforms){
            if(transform as RectTransform == null){
                Undo.RecordObject(transform,"Reset " + transform.name);
                transform.position = Vector3.zero;
                transform.rotation = Quaternion.identity;
                transform.localScale = Vector3.one;
            }
        }
    }
    
    [MenuItem("GameObject/Change Layout 1 &1")]
    private static void SwitchLayout1() {
        EditorApplication.ExecuteMenuItem("Window/Layouts/Default");
    }

    [MenuItem("GameObject/Change Layout 2 &2")]
    private static void SwitchLayout2() {
        EditorApplication.ExecuteMenuItem("Window/Layouts/2 by 3");
    }

    [MenuItem("GameObject/Change Layout 3 &3")]
    private static void SwitchLayout3() {
        EditorApplication.ExecuteMenuItem("Window/Layouts/Animation");
    }
}