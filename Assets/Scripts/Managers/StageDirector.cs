using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//A class that keeps track of important positions and units inside

//TO-DO: This class is not really necessary. It should not exists if refactored.


//Responsibilities:

//Keep track of positions
//Provide a list of enemies/ player units in order (not sure if necessary)
//Keeps track of the last selected unit (to remember where to attack)
//used as an intermediary to apply damage
public class StageDirector : MonoBehaviour
{
    public List<Unit> Units;

    public float SuccessiveDamageDelay = .2f;
    public Transform SwapRotator;
    private Animator SwapAnimator;
    public TurnManager TurnManagerRef;

    private void Reset() {
        RefreshUnitList();    
    }

    private void Start() {
        RefreshUnitList();
        if(SwapRotator)SwapAnimator = SwapRotator.GetComponent<Animator>();
    }

    public void RefreshUnitList(){
        this.Units = GetComponentsInChildren<Unit>().ToList();
    }

    //used to apply damage to all units
    public void ApplyDamageToAllUnits(int damageAmmount, bool ignoreDefense, List<Unit> targets = null){
        if(targets == null)targets = Units;
        StartCoroutine(ApplyDamageSuccessively(targets,damageAmmount,ignoreDefense));
    }

    private IEnumerator ApplyDamageSuccessively(List<Unit> targets, int damageAmmount,bool ignoreDefense){
        foreach(Unit unit in targets){
            if(ignoreDefense)
                unit.ChangeHP(damageAmmount);
            else
                unit.ChangeHP(Mathf.Min(damageAmmount + unit.BaseDefense,0));
            yield return new WaitForSeconds(SuccessiveDamageDelay);
        }
        
    }

    public bool CanSwap(){
        return SwapRotator != null; //to-do add additional checks for if a unit has moved
    }

    //swap should be done with the turn manager not the player
    public void Swap(){
        if(!CanSwap())return;
        
        SwapAnimator.SetTrigger("RotateBack");

        
    }

}
