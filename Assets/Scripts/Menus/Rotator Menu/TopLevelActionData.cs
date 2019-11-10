using UnityEngine;

[CreateAssetMenu(fileName = "TopLevelActionData", menuName = "Paper Mario Clone/TopLevelActionData", order = 0)]
public class TopLevelActionData : ScriptableObject {
    public string ActionName;
    public Sprite IconDisplay;
    public ActionDataList Actions;

    public float MaxTagExpansionAmount = 5.12f;
}