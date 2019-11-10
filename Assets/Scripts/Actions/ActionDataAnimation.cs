using UnityEngine;

public enum CostType{
    FP,
    SP,
    HP
}

[System.Serializable]
public enum AnimationStepType{
    SetBool,
    SetTrigger,
    SetInt,
    QuickSwapAnimationClip
}

[System.Serializable]
public class AnimationStep{
    public AnimationStepType StepType;
    public string ParamterName;
    public int IntValue;
    public bool BoolValue;
    public AnimatorOverrideController QuickSwapController;


    public void PerformStep(Animator animator){
        switch(StepType){
            case AnimationStepType.SetBool:
                animator.SetBool(ParamterName,BoolValue);
                return;
            case AnimationStepType.SetTrigger:
                animator.SetTrigger(ParamterName);
                return;
            case AnimationStepType.SetInt:
                animator.SetInteger(ParamterName,IntValue);
                return;
            case AnimationStepType.QuickSwapAnimationClip:
                //not tested
                animator.runtimeAnimatorController = QuickSwapController;
                return;
            default:
                return;
        }
    }
}

[CreateAssetMenu(fileName = "ActionData", menuName = "Paper Mario Clone/Actions/AnimationStep", order = 0)]
public class ActionDataAnimation : ActionData {


    public AnimationStep[] ActionsToPerform;


    public override void PerformAction(Unit actingUnit, Unit[] targetUnits){
        if(actingUnit == null){
            Debug.Log("An empty target unit was received. Mistake likely.");
            return; 
        }
        //to-do: add functionality to trigger an animation on the player using some sort of keyword
        Animator animator = actingUnit.GetComponent<Animator>();
        foreach(AnimationStep step in ActionsToPerform){
            step.PerformStep(animator);
        }
    }

}