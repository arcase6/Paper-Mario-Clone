﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    private int runtimeHP;
    public override int HP{
        get => runtimeHP;
        set => runtimeHP = value;
    }

    [Tooltip("Used for exponentially decreasing chance for successiveful power bounce. 100 is no decay while 50 is decay at (1/2) per bounce")]
    public int PowerBounceMultiplier = 100; //multiplier is used in infinite decay for multibounces -- 5

    private EnemyBrain Brain;

    protected override void OnEnable() {
        base.OnEnable();
        Brain = GetComponent<EnemyBrain>();
        runtimeHP = this.Stats.HP;
        RaiseHPChangedEvent();
    }

    public override void BeginTurn(){
        base.BeginTurn();
        this.Brain.PerformTurn();
    }

    public override void EndTurn(){
        this.CanAct = false;

        //the unit set is just a monobehavior that has a reference to the set
        //this is how we notify the turn manager (turn manager has one set for players and one for enemies)
        OrientationController.Orientation = startingOrientation;
        foreach(UnitSetHolder holder in UnitSet.Holders){
            holder.OnUnitEndTurn.Invoke(this);
        }
    }
}
