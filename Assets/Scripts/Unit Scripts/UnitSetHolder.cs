using UnityEngine;

public class UnitSetHolder: MonoBehaviour{
    public UnitRuntimeSet Set;
    public UnitEvent OnUnitAdd;
    public UnitEvent OnUnitRemove;
    public UnitEvent OnUnitEndTurn;

     private void OnEnable() {
         Set.AddHolder(this);
    }

    private void OnDisable() {
        Set.RemoveHolder(this);    
    }
}
