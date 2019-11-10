using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : Unit
{
    //Enemies do not have FP so all the FP related stuff is here
    public Transform HealingFlower;
    private StringMediator FlowerText;

    /// <summary>
    /// Event used to update the status bar 
    /// </summary>
    public event System.Action FPChanged;

    //stats that are shared between all partners

    public List<TopLevelActionData> MenuActions;

    #region Stat Properties - and SharedStats Reference
    public PlayerSharedData SharedStats;

    public int MaxBP{
        get => SharedStats.MaxBP;
        set => SharedStats.MaxBP = value;
        }
    public int BP{
        get => SharedStats.BP;
        set => SharedStats.BP = value;
        }
    
    public int FP{
        get => SharedStats.FP;
        set => SharedStats.FP = value;
        }
    public int MaxFP{
        get => SharedStats.MaxFP;
        set => SharedStats.MaxFP = value;
        }

    public float SP{
        get => SharedStats.SP;
        set => SharedStats.SP = value;
        }

    public float MaxSP{
        get => SharedStats.MaxSP;
        set => SharedStats.MaxSP = value;
        }

#endregion

    protected override void Start(){
        base.Start();
        FlowerText = HealingFlower?.GetComponentInChildren<StringMediator>();
    }

    #region Unit abstract methods
    public override void BeginTurn(){
        
    }

    public override void EndTurn(){
        this.CanAct = false;

        //the unit set is just a monobehavior that has a reference to the set
        //this is how we notify the turn manager (turn manager has one set for players and one for enemies)
        this.UnitSet.NotifyTurnEnded(this);
    }

#endregion
    
    //overriden from the base Unit class so that flower effects are check too
    protected override bool CheckEffectsActive()
    {
        return (HealingHeart && HealingHeart.gameObject.activeInHierarchy) || (HealingFlower && HealingFlower.gameObject.activeInHierarchy) || EffectSpawner.enabled;
    }

    public bool IsPartnerAbleToAct(){
        Player otherUnit = UnitSet.Units.Where(u => u != this).First() as Player;
        return otherUnit.CanAct;
    }


    public bool SwapUnitOrder(RotatorMenu rotatorMenuRef){
        //code for rotated the active unit to the back goes here (much of it is delegated to the stage director)

        StageDirector stageDirector = GetComponentInParent<StageDirector>();
        Player otherUnit = UnitSet.Units.Where(u => u != this).First() as Player;

        if(rotatorMenuRef.IsOpen)
            rotatorMenuRef.Hide(); //the swap action will call show when the animation finishes
        rotatorMenuRef.DynamicallyLoadedActions = otherUnit.MenuActions;
        rotatorMenuRef.ActingUnitRef.Value = otherUnit;
        stageDirector.Swap();
        return true;
            
    }

    public void ChangeFP(int amount)
    {
        if (amount > 0)
        {
            FP = Mathf.Min(FP + amount, MaxFP);
            //startflower effect
            if (FlowerText)
            {
                FlowerText.Value = amount.ToString();
                HealingFlower?.gameObject?.SetActive(true);
            }
            this.Animator.SetTrigger("Healing");
        }
        else
        {
            FP = Mathf.Max(FP + amount, 0);
        }
        FPChanged?.Invoke();
    }

    public void ChangeSP(float amount)
    {
        if (amount > 0)
        {
            SP = Mathf.Min(SP + amount, MaxSP);
        }
        else
        {
            SP = Mathf.Max(SP - amount, 0);
        }
    }

    protected override void OnDisable() {
        base.OnDisable();
    }

    protected override void OnEnable(){
        base.OnEnable();
    }
}
