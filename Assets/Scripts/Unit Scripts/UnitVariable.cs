using UnityEngine;

[CreateAssetMenu(fileName = "UnitVariable", menuName = "Paper Mario Clone/Unit Related/Unit Variable", order = 0)]
public class UnitVariable : ScriptableObject {
    public Unit InitialValue; 
    public Unit Value;

    private void Awake() {
        if(InitialValue != null)
            Value = InitialValue;
    }
}