using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetingSystem : Menu
{
    

    public ActionData CurrentAction;
    public UnitVariable ActingUnit;
    public TransformList Targets;
    public Transform PointerPool;
    public StageDirector StageDirector;

    /// <summary>
    /// A variable indicating if the target can be changed to something else
    /// </summary>
    private int SelectedUnitIndex = 0;
    private Unit[] ValidTargets;

    [HideInInspector]
    public bool TargetingIsLocked;
    private TargetingSysInput TargetingSysInput;

    private void Start() {
        this.TargetingSysInput = GetComponent<TargetingSysInput>();
    }

    private void Reset() {
        this.StageDirector = GetComponent<StageDirector>();
    }

    public bool ValidTargetExists(){
        return StageDirector.Units.Any(unit => ValidateUnit(unit));
    }

    private bool SelectedTargetCanChange()
    {
        return !(CurrentAction.TargetType.HasFlag(TargetType.Multiple)
                || CurrentAction.TargetType.HasFlag(TargetType.Targetless)
                || CurrentAction.TargetType.HasFlag(TargetType.SelfOnly)
                || CurrentAction.TargetType.HasFlag(TargetType.PartnerOnly));
    }

    private bool IsSingleTarget(){
        return !(CurrentAction.TargetType.HasFlag(TargetType.Multiple)
                || CurrentAction.TargetType.HasFlag(TargetType.Targetless));
    }

    public Unit[] GetTargetedUnits(){
        if(this.IsSingleTarget()){
            return new Unit[]{StageDirector.Units[SelectedUnitIndex]};
        }
        else{
            return StageDirector.Units.ToArray();
        }
    }

    public void ExecuteCurrentlySelectedActions(){
        CurrentAction.PerformAction(ActingUnit.Value, GetTargetedUnits());
        this.CloseAllMenus();
    }

    public void DisplayPointers(){
        if(CurrentAction.TargetType.HasFlag(TargetType.Targetless))
            return; // nothing to display

        if(IsSingleTarget()){ //single target
            //make sure active
            Transform pointer = PointerPool.GetChild(0);
            pointer.gameObject.SetActive(true);
            pointer.position = ValidTargets[SelectedUnitIndex].transform.position; 
        }
        else{ //multiple targets
            for(int i = 0; i < ValidTargets.Length; i++){
                Transform target = ValidTargets[i].transform;
                Transform pointer = PointerPool.GetChild(i);
                pointer.gameObject.SetActive(true);
                pointer.position = target.position;
            }
        }
    }

    public void NextTarget(){
        SelectedUnitIndex = (SelectedUnitIndex + 1) % ValidTargets.Length; 
        DisplayPointers();
    }

    public void PrevTarget(){
        SelectedUnitIndex = (SelectedUnitIndex - 1); 
        if(SelectedUnitIndex < 0)SelectedUnitIndex = ValidTargets.Length - 1;
        DisplayPointers();
    }

    public bool ValidateUnit(Unit unit){
        return ValidateUnit(this.CurrentAction,unit);
    }

    public bool ValidateUnit(ActionData action,Unit unit){
        if(action.TargetType.HasFlag(TargetType.Targetless))
            return true;
        else if(action.TargetType.HasFlag(TargetType.Ally))
        {
            return ValidateAlly(action, unit);
        }
        else if(action.TargetType.HasFlag(TargetType.Enemy))
        {
            return ValidateEnemy(action, unit);
        }
        return true;
    }

    private bool ValidateEnemy(ActionData action, Unit unit)
    {
        if (action.TargetType.HasFlag(TargetType.EnemyFrontOnly))
            return unit == StageDirector.Units.Where(u => u.UnitType == UnitType.Ground).First();
        else if (action.TargetType.HasFlag(TargetType.EnemyGroundOnly))
            return unit.UnitType == UnitType.Ground;
        if (action.TargetType.HasFlag(TargetType.EnemyAirOnly))
            return unit.UnitType == UnitType.Air;
        if(unit.UnitType == UnitType.Ceiling)
            return action.TargetType.HasFlag(TargetType.EnemyCeiling);
        return true;
    }

    private bool ValidateAlly(ActionData action, Unit unit)
    {
        if (action.TargetType.HasFlag(TargetType.SelfOnly))
            return this.ActingUnit.Value == unit;
        else if (action.TargetType.HasFlag(TargetType.PartnerOnly))
            return this.ActingUnit.Value != unit;
        return true;
    }

    public override void DisableInput()
    {
        this.TargetingSysInput.enabled = false;
    }

    public override void EnableInput()
    {
        this.TargetingSysInput.enabled = true;
    }

    public override void Hide()
    {
        IsOpen = false;
        DisableInput();
        for(int i = 0; i < PointerPool.childCount;i++){
            PointerPool.GetChild(i).gameObject.SetActive(false);
        }   
    }

    public override void Show()
    {
        IsOpen = true;
        EnableInput();
        TargetingIsLocked = !SelectedTargetCanChange();
        ValidTargets =  StageDirector.Units.Where(unit => ValidateUnit(unit)).ToArray();
        if(ValidTargets.Length != 0)
            SelectedUnitIndex = Mathf.Min(SelectedUnitIndex, ValidTargets.Length - 1);
        DisplayPointers();
        
    }
}
