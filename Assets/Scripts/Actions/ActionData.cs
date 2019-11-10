using System;
using UnityEngine;

[Flags]
[Serializable]
public enum TargetType{
    Ally = 1,
    Multiple = 2,
    SelfOnly = 4,
    PartnerOnly = 8,
    Enemy = 16,
    EnemyFrontOnly = 32,
    EnemyAirOnly = 64,
    EnemyGroundOnly = 128,
    EnemyCeiling = 256,
    Targetless = 512
}



public abstract class ActionData : ScriptableObject {
    public Sprite Icon;
    public CostType AttackCostType;
    public int AttackCost;

    public TargetType TargetType = TargetType.Ally;

    public string CostDisplay{
        get{
            if(AttackCost > 0){
                return string.Format("{0} {1}",AttackCost,AttackCostType.ToString("g"));
            }
            return "";
        }
    }

    public abstract void PerformAction(Unit ActingUnit,Unit[] TargetUnits);


    public virtual bool CanPerformAction(Unit ActingUnit){
        Player player;
        switch(this.AttackCostType){
            case CostType.FP:
                player = ActingUnit as Player;
                if (!player) return true;
                return player.FP >= this.AttackCost;
            case CostType.HP:
                return ActingUnit.HP > this.AttackCost;
            case CostType.SP:
                player = ActingUnit as Player;
                if (!player) return true;
                return player.SP >= this.AttackCost;
            default:
                return true;
        }
    }

    //a helper function for applying dama
    protected static void ApplyDamage(Unit actingUnit, Unit targetUnit, int baseDamage)
    {
        int DamageAmmount = actingUnit.Stats.BaseAttack + actingUnit.AttackBonus + baseDamage - targetUnit.BaseDefense - targetUnit.DefenseBonus; 
        DamageAmmount = Mathf.Max(0,DamageAmmount);
        targetUnit.ChangeHP(DamageAmmount * -1);
    }
}