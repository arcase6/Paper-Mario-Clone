using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicAttack : ActionData
{
    public bool isRanged = false;
    public int BaseAttackDamage = 1;

    public override void PerformAction(Unit ActingUnit, Unit[] TargetUnits)
    {
        if(!isRanged){
            Transform targetTransform = Unit.GetChildTransformByName(TargetUnits[0].transform.parent,"Forward");
            ActingUnit.StartMoveTo(targetTransform, () => Attack(ActingUnit, TargetUnits));
        }
        else{
            ActingUnit.StartMoveTo("Forward", () => Attack(ActingUnit, TargetUnits));
        }
    }

    //This is the only thinig that needs to be overrident
    public abstract void Attack(Unit ActingUnit, Unit[] TargetUnits);
}
