using UnityEngine;

[CreateAssetMenu(fileName = "DoNothingAction", menuName = "Paper Mario Clone/Actions/Do Nothing Action", order = 0)]
public class DoNothingAction : ActionData
{
    public override void PerformAction(Unit ActingUnit, Unit[] TargetUnits)
    {
        ActingUnit.EndTurn(); //just end the turn
    }
}