using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ActionState{
    Inactive,
    Hover,
    Activated
}

public class TopLevelAction : MonoBehaviour
{

    public Animator TagExpansionAnimator;

    public TopLevelActionData ActionData;


    public ActionDataList SubMenuActionList;
    public UnityEvent OnActivate; 

    public SpriteRenderer IconRenderer;
    public StringMediator ActionNameDisplay;
    public Expander TagExpandingController;
    private RotatorMenu ParentMenu;
    
    // made public to adjust from the inspector
    public float MarginAmmount = 1.5f;
    public float LetterExpansionAmmount = .75f;

    private void OnEnable() {
        this.ParentMenu = transform.parent.parent.GetComponentInChildren<RotatorMenu>();
    }



    public void UpdateAction()
    {
        IconRenderer.sprite = ActionData.IconDisplay;
        ActionNameDisplay.Value = ActionData.ActionName;
        TagExpandingController.MaxExpansion =  ActionData.MaxTagExpansionAmount;
    }

    public void SetState(ActionState state){
        switch(state){
            case ActionState.Inactive:
            TagExpansionAnimator.SetBool("IsShown",false);
            break;
            case ActionState.Hover:
            TagExpansionAnimator.SetBool("IsShown",true);
            break;
            case ActionState.Activated:
            try{
                SubMenuActionList.List = ActionData.Actions.List;
                this.ParentMenu.MenuRouter.EnabledMenus["Scroll Menu"].ShowDialogue();
                OnActivate.Invoke();
            }catch{Debug.Log("The scroll menu could not be found in the scene an was not shown as a result (or the scroll menu has a problem in its Show method implentation");}  
            break;
            default:
            break;
        }
    }

    public void ActivateSelectedAction(){
        this.SetState(ActionState.Activated);
    }

}
