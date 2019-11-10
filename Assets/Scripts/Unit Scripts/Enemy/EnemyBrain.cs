using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    #region Member Definitions
    [Range(0,100)]
    public int FrontUnitAttackProbability = 80;
    
    public EnemyActionList Attacks;

    public ActionData Item;

    public UnitRuntimeSet PlayerSet;
    public UnitRuntimeSet EnemySet;

    private Unit EnemyBody;

    #endregion

    private void Start() {
        this.EnemyBody = GetComponent<Unit>();
    }

    public void PerformTurn()
    {
        ActionData nextAction = DetermineActionToPerform();
        List<Unit> targets = DetermineActionTargets(nextAction);
        nextAction.PerformAction(EnemyBody,targets.ToArray());

    }

    protected virtual List<Unit> DetermineActionTargets(ActionData nextAction){
        if(nextAction.TargetType.HasFlag(TargetType.Ally)){
            //healing or buff item of some sort
            if(nextAction.TargetType.HasFlag(TargetType.Multiple)){
                return EnemySet.Units;
            }
            else{
                //select the highest priority enemy to buff
                //prioritize self over others using slightly higher weight
                //current formula uses hp difference to determine who to buff
                //this will favor enemies with a lot of hp since greater potential damage amountz
                

                Unit highestPriorTarget = EnemyBody;
                int maxPriority = Mathf.Abs(highestPriorTarget.MaxHP - highestPriorTarget.HP);
                maxPriority = (int)(maxPriority * 1.5f + 2);
                foreach(Unit unit in EnemySet.Units){
                    int priority = Mathf.Abs(unit.MaxHP - unit.HP);
                    if(priority > maxPriority){
                        maxPriority = priority;
                        highestPriorTarget = unit;
                    }
                }

                return new List<Unit>(){highestPriorTarget};
            }

        }
        else{
            //some sort of attack or attack item
            if(nextAction.TargetType.HasFlag(TargetType.Multiple)){
                return PlayerSet.Units;
            }
            else{
                Unit closestPlayer = PlayerSet.Units.OrderBy(u => Vector3.Magnitude(u.transform.position - transform.position)).First();
                List<Unit> targets = new List<Unit>();
                if(this.FrontUnitAttackProbability >= UnityEngine.Random.Range(0,101))
                    targets.Add(closestPlayer);
                else
                    targets.Add(PlayerSet.Units.Where(u => u != closestPlayer).First());
                return targets;
            }


        }
    }

    protected virtual ActionData DetermineActionToPerform(){

        //prioritizes item use of attack use
        if(Item != null){
            //check all the other enemy Units
            ItemAction itemAction = Item as ItemAction;
            if (itemAction.BuffType != BuffType.HP || itemAction.BuffAmmount < 0){
                //special effect item or attack item (always use without thinking)
                return itemAction;
            }
            else if(EnemySet.Units.Any(u => u.MaxHP - u.HP > ((itemAction.BuffAmmount + 1) / 2) || (u.MaxHP + 1) / u.HP > 1) ){
                //check to see if another unit is damaged enough to warrant use
                return itemAction;
            }
        }
        

        //select which action to perform based on assigned probability weights
        //defaults to the last one if all previous ones do not pass
        for(int i = 0; i < Attacks.Actions.Count - 1; i++){
            float weight = Attacks.Actions[i].Weight;
            if(weight >= UnityEngine.Random.Range(0,101))
                return Attacks.Actions[i].Action;
        }
        return Attacks.Actions.Last().Action;
    }
}
