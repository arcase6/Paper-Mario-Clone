using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(fileName = "UnitRuntimeSet", menuName = "Paper Mario Clone/Unit Related/UnitRuntimeSet", order = 0)]
public class UnitRuntimeSet : ScriptableObject {
    public List<Unit> Units;
    
    [HideInInspector]
    public List<UnitSetHolder> Holders = new List<UnitSetHolder>();

    public void AddHolder(UnitSetHolder holder){
        Holders.Add(holder);
    }

    public void RemoveHolder(UnitSetHolder holder){
        if(Holders.Contains(holder))
            Holders.Remove(holder);
    }

    public void AddUnit(Unit unit){
        this.Units.Add(unit);
        foreach(UnitSetHolder holder in Holders)holder.OnUnitAdd.Invoke(unit);
    }

    public void RemoveUnit(Unit unit){
        if(this.Units.Contains(unit)){
            Units.Remove(unit);
            foreach(UnitSetHolder holder in Holders)holder.OnUnitRemove.Invoke(unit);
        }
    }

    public void NotifyTurnEnded(Unit unit){
        foreach(UnitSetHolder holder in Holders)holder.OnUnitEndTurn.Invoke(unit);
    }
}