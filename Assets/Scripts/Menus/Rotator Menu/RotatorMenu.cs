using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorMenu : Menu
{
    //center of the rotation
    [Tooltip("This list is changed during runtime. To do: Either use a scriptable object or tags to find the rotation menu during runtime.")]

    public List<TopLevelActionData> DynamicallyLoadedActions;

    [HideInInspector]
    public TopLevelAction[] ActionContainers;
    [HideInInspector]
    public int SelectedActionIndex;
    public float Radius = 150f;
    [Range(0, 90)]
    public int AngularSpacing = 30;

    [Range(0, 360)]
    public int startAngle = 0;
    public Transform ActionContainer;

    public Animator Rotator;
    public bool IsRotating;
    public AudioSource audioSource;
    public AudioClip RotateSound;
    public AudioClip StallSound;

    private RotatorMenuInput InputComponent;

    public UnitVariable ActingUnitRef;

    private void Reset() {
        this.ActionContainer = transform.GetChild(0).GetChild(0);
        this.startAngle = 60;
        this.AngularSpacing = 30;
        
    }


    //used for resetting the menu action positions in the editor
    [ContextMenu("Reset Action Positions")]
    public virtual void ResetActionPositions()
    {
        if(this.ActionContainers.Length == 0)
            this.ActionContainers = this.GetComponentsInChildren<TopLevelAction>();
        this.ActionContainers = ActionContainer.GetComponentsInChildren<TopLevelAction>();
        for (int i = 0; i < ActionContainers.Length; i++)
        {
            float nextAngle = startAngle - (AngularSpacing * i);
            //Quaternion rotation = Quaternion.Euler(0, 0, nextAngle); -- not used anymore (may use later though)
            Quaternion rotation = Quaternion.identity;            
            nextAngle *= Mathf.Deg2Rad;
            Vector3 position = new Vector3(Radius * Mathf.Cos(nextAngle), Radius * Mathf.Sin(nextAngle), 0);
            
            ActionContainers[i].transform.localPosition = position;
            ActionContainers[i].transform.localRotation = rotation;
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        if(this.ActionContainers.Length == 0)
            this.ActionContainers = this.GetComponentsInChildren<TopLevelAction>(true);
        
        if(!this.InputComponent)
            this.InputComponent = this.GetComponent<RotatorMenuInput>(); 
    }

    public void RefreshMenuActions()
    {
        int i = 0;
        foreach (TopLevelAction ActionContainer in ActionContainers)
        {
            bool actionExists = i < DynamicallyLoadedActions.Count;
            if (actionExists)
            {
                ActionContainer.ActionData = DynamicallyLoadedActions[i];
                ActionContainer.UpdateAction();
            }
            ActionContainer.gameObject.SetActive(actionExists);
            i++;
        }
    }

    private void DisplayMenu()
    {
        this.transform.GetChild(0).gameObject.SetActive(true);
        int i;
        SelectedActionIndex = 0;
        this.ActionContainers[SelectedActionIndex].SetState(ActionState.Hover);
        for (i = 1; i < DynamicallyLoadedActions.Count; i++)
        {
            this.ActionContainers[i].SetState(ActionState.Inactive);
        }
        Rotator.SetInteger("Action Index", SelectedActionIndex);
        Rotator.SetTrigger("ResetRotation");
    }

    public override void OnDisable() {
        base.OnDisable();
        this.GetComponent<RotatorMenuInput>().gameObject.SetActive(false);
    }

    public void SetSelectedToHoverState(){
        this.ActionContainers[SelectedActionIndex].SetState(ActionState.Hover);
    }

    public void NextAction(){
        if(this.IsRotating)
            return;
        IsRotating = true;
        if(this.SelectedActionIndex < this.DynamicallyLoadedActions.Count - 1){
            this.ActionContainers[SelectedActionIndex].SetState(ActionState.Inactive);
            this.SelectedActionIndex++;
            Rotator.SetInteger("Action Index",SelectedActionIndex);
            audioSource.PlayOneShot(RotateSound);
        }
        else{
            Rotator.SetTrigger("Stall Next");
            audioSource.PlayOneShot(StallSound);
        }
    }

    public void PreviousAction(){
        if(this.IsRotating)
            return;
        IsRotating = true;
        if(this.SelectedActionIndex > 0){
            this.ActionContainers[SelectedActionIndex].SetState(ActionState.Inactive);
            this.SelectedActionIndex--;
            Rotator.SetInteger("Action Index",SelectedActionIndex);
            audioSource.PlayOneShot(RotateSound);
        }
        else{
            Rotator.SetTrigger("Stall Prev");
            audioSource.PlayOneShot(StallSound);
        }
    }



    #region Menu abstract class implementation
    //A menu must have a the display elements on the game object below the root
    //This is because the menu needs to be enabled in order to register itself to the menu router
    
    public override void DisableInput()
    {
        this.InputComponent.enabled = false;
    }

    public override void EnableInput()
    {
        this.InputComponent.enabled = true;
    }

    public override void Hide()
    {
        IsOpen = false;
        DisableInput();
        this.transform.GetChild(0).gameObject.SetActive(false);
    }

    public override void Show()
    {
        IsOpen = true;
        RefreshMenuActions();
        DisplayMenu();
        EnableInput();
    }

    #endregion
}


