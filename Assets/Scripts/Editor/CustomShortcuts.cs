using UnityEngine;
using UnityEditor;

public class CustomShortcuts : EditorWindow {

    [MenuItem("GameObject/Reset Transform %r")]
    private static void ResetTransform() {
        foreach(Transform transform in Selection.transforms){
            if(transform as RectTransform == null){
                transform.position = Vector3.zero;
                transform.rotation = Quaternion.identity;
                transform.localScale = Vector3.one;
            }
        }
    }

    
}