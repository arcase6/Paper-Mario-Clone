﻿using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[System.Serializable]
public enum BuffType
{
    HP,
    FP,
    SP,
    AttackPower,
    Defense
}

[System.Serializable]
public class AnimationRemapping
{
    public string OriginalName;
    public AnimationClip ReplacementClip;
}


[CreateAssetMenu(fileName = "ItemAction", menuName = "Paper Mario Clone/Actions/ItemAction", order = 0)]
public class ItemAction : ActionData
{


    [Tooltip("HP, FP, etc. (More advanced effects should create their own class)")]
    public BuffType BuffType;
    public int BuffAmmount = 5;
    //This is the name of a trigger in the units state machine (it will be played automatically once the unit is in position)
    public List<AnimationRemapping> AnimationRemappings;
    public string ActivationLocationName = "Forward"; //kept generic on purpose -- to-do change to enum to avoid errors
    
    [Tooltip("A delay to apply before the item is used. Delay starts after animation begins.")]
    public float startDelay = .1f;
    

    [Tooltip("An optional prefab to spawn. Not necessary for all items. The prefab should call the ApplyItemEffect method a second time. The normal animation will spawn the prefab not use the item when called first.")]
    public GameObject PrefabToSpawn;
    [Tooltip("How much to offset the prefab by")]
    public Vector3 PrefabOffset;
    [Tooltip("When to apply the item effect. This works on a delay so that animations can be played. Only used when a prefab is spawned")]
    public float EffectApplicationDelay = 2f;

    //entry point from the system action - Unit has not moved or done anything
    public override void PerformAction(Unit ActingUnit, Unit[] TargetUnits)
    { 
        Debug.Log("Perform Action Called");
        ActingUnit.StartMoveTo(ActivationLocationName, () => PlayItemAnimations(ActingUnit, TargetUnits));
    }


    //Unit is in position and ready to play item animations
    public void PlayItemAnimations(Unit actingUnit, Unit[] targetUnits)
    {
        actingUnit.UseItem(this.AnimationRemappings, () => ApplyItemEffect(actingUnit, targetUnits), this.startDelay);
    }



    //unit is playing item animations and an event has triggered asking for item effect to be carried out
    //when extending this is the only thing that will need to be changed except for possible animations
    public virtual void ApplyItemEffect(Unit actingUnit, Unit[] targetUnits)
    {
        if (PrefabToSpawn && !actingUnit.EffectSpawner.enabled)
        {
            actingUnit.EffectSpawner.ParentItem = this;
            actingUnit.EffectSpawner.enabled = true;
            actingUnit.EffectSpawner.ItemApplyCallback = () => ApplyItemEffectInternal(actingUnit,targetUnits);
            //return;
        }
        else{
            ApplyItemEffectInternal(actingUnit, targetUnits);
        }
    }

    private void ApplyItemEffectInternal(Unit actingUnit, Unit[] targetUnits)
    {
        if (actingUnit.UnitType == UnitType.Ally && targetUnits.First().UnitType != UnitType.Ally)
        {
            targetUnits.First().GetComponentInParent<StageDirector>().ApplyDamageToAllUnits(this.BuffAmmount, false, targetUnits.ToList());
        }
        else
        {
            DirectlyApplyEffect(targetUnits);
        }
        actingUnit.PollForEffectsToEnd(() => actingUnit.StartMoveTo("Standard", actingUnit.EndTurn));
    }

    private void DirectlyApplyEffect(Unit[] targetUnits)
    {
        Player player;
        foreach (Unit targetUnit in targetUnits)
        {
            switch (BuffType)
            {
                case BuffType.HP:
                    targetUnit.ChangeHP(this.BuffAmmount);
                    break;
                case BuffType.FP:
                    player = targetUnit as Player;
                    if(player)
                        player.ChangeFP(this.BuffAmmount);
                    break;
                case BuffType.SP:
                    player = targetUnit as Player;
                    if(player)
                        player.ChangeSP(this.BuffAmmount / 3); //sp is special in that it is fractional
                    break;
                case BuffType.AttackPower:
                    targetUnit.ChangeAttack(this.BuffAmmount);
                    break;
                case BuffType.Defense:
                    targetUnit.ChangeDefense(this.BuffAmmount);
                    break;
                default:
                    break;
            }
        }
    }




}


