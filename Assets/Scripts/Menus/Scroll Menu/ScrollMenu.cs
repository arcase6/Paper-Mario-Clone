using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollMenu : MonoBehaviour
{
    public Transform SelectionIcon;
    public Vector2 SelectionIconOffset;
    public float Spacing = 4;
    public int DisplaySize = 4;
    public int PoolSize = 8;
    
    public MenuAction[] menuItems; //Only Pool Size loaded at a time

    public MenuRouter MenuRouter{
        get{
            return ParentHolder.MenuRouter;
        }
    }
    
    private int MinDisplayedIndex;
    private int MaxDisplayedIndex{
        get{
             return MinDisplayedIndex + Mathf.Min(AvailableActions.List.Count, DisplaySize) - 1;
        }
    }
    


    public int SelectedIndex = 0;

    //these variables are used to transition between indexes
    public float SelectedIndexContinuous = 0;
    public float MinDisplayedValueContinuous = 0;
    private bool isMoving = false;

    public float MoveTime = .2f;

    public ActionDataList AvailableActions;

    private bool CancelPressed = false;
    private bool SelectPressed = false;

    public UnitVariable ActingUnit;

    private ScrollMenuInput InputComponent;

    private ScrollMenuHolder ParentHolder;

    [ContextMenu("Reset Positions")]
    public void ResetMenuItemsToStart()
    {
        transform.localPosition = Vector3.zero; // reset to 0 because scrolling
        menuItems = GetComponentsInChildren<MenuAction>();
        for (int i = 0; i < menuItems.Length; i++)
        {
            menuItems[i].transform.localPosition = new Vector3(0, i * Spacing * -1, 0);
        }
        SetPointerIconPosition();
    }

    void OnEnable()
    {
        if(!InputComponent)
            InputComponent = GetComponent<ScrollMenuInput>();
        if(!InputComponent.enabled)
            InputComponent.enabled = true;
        //set menu to first item selected and reset all positions
        SelectedIndexContinuous = SelectedIndex = MinDisplayedIndex = 0;
        ResetMenuItemsToStart();

        //calculate values used for offsets
        int bufferSize = (PoolSize - DisplaySize) / 2;
        int ActionIndexOffset = MinDisplayedIndex - bufferSize; 

        //Disable extra actions so they don't display
        for (int i = 0; i < PoolSize; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i  < AvailableActions.List.Count + bufferSize);
            int ActionIndex = i + ActionIndexOffset;
            if(ActionIndex < AvailableActions.List.Count && ActionIndex >= 0){
                transform.GetChild(i).GetComponent<MenuAction>().action = AvailableActions.List[ActionIndex];
            }
        }

        //Move the pointer to the start of the list
        SetPointerIconPosition();
        ParentHolder = GetComponentInParent<ScrollMenuHolder>();
    }

    public void OnDisable(){

    }

    void SetPointerIconPosition()
    {
        Vector3 iconPosition = new Vector3(SelectionIconOffset.x, Spacing * -1 * (SelectedIndexContinuous - MinDisplayedIndex) + SelectionIconOffset.y, 0);
        SelectionIcon.localPosition = iconPosition;
    }



    // Update is called once per frame
    public void ProcessInput()
    {
        if(isMoving)
            return;
        CancelPressed = CancelPressed || Input.GetButtonDown("Cancel");
        if(CancelPressed && !isMoving){
            SelectPressed = CancelPressed = false;
            ParentHolder.CloseMenuAndReOoenCaller();
            return;
        }
        SelectPressed = SelectPressed || Input.GetButtonDown("Submit");
        if(SelectPressed && !isMoving)
        {
            SelectPressed = CancelPressed = false;
            ActionData selectedAction = this.AvailableActions.List[SelectedIndex];
            if (selectedAction.TargetType != TargetType.Targetless)
                ActivateTargettingSystem();
            else{
                selectedAction.PerformAction(ActingUnit.Value,null);
                this.ParentHolder.CloseAllMenus();
            }
            return;
        }

        float verticalAxis = Input.GetAxis("Vertical");
        HandleVerticalInput(verticalAxis);

    }

    private void ActivateTargettingSystem()
    {
        Unit actingUnit = this.ActingUnit.Value;
        string targetingSystemName = this.AvailableActions.List[SelectedIndex].TargetType.HasFlag(TargetType.Enemy) && actingUnit.UnitType.Equals(UnitType.Ally) ? "Enemy Formation" : "Player Formation";
        try
        {
            TargetingSystem targettingSystem = MenuRouter.EnabledMenus[targetingSystemName] as TargetingSystem;

            targettingSystem.CurrentAction = this.AvailableActions.List[SelectedIndex];

            targettingSystem.ShowDialogueAndHideLastMenu();
        }
        catch
        {
            Debug.Log("Failed to open the targettings system. Make sure that targetting systems exists with the anems Enemy Formation,  Player Formation AND both are enabled.");
            Debug.Log(MenuRouter == null);
            string existingKeys = "";
            foreach(string key in MenuRouter.EnabledMenus.Keys){
                existingKeys += key + ", ";
            }
            Debug.Log($"Searched for key {targetingSystemName}. Keys recorded by Menu Router : {existingKeys}");
        }
    }

    private void HandleVerticalInput(float verticalAxis)
    {
        int indexOffset = verticalAxis < 0 ? 1 : 0;
        indexOffset = verticalAxis > 0 ? -1 : indexOffset;

        if (indexOffset != 0)
        {
            int newIndex = SelectedIndex + indexOffset;
            if (newIndex == AvailableActions.List.Count || newIndex < 0)
                return; 
            isMoving = true;
            SelectedIndex = newIndex;
            if (newIndex >= MinDisplayedIndex && newIndex <= MaxDisplayedIndex)
            {  
                //Pointer new position is within bounds. Only the pointer is moved
                StartCoroutine(SmoothMovePointer(newIndex - indexOffset));
            }
            else
            {
                //Shift the entire menu while shifting buffers on top and bottom
                if (indexOffset < 0)
                    IncreaseTopBuffer();
                else
                    IncreaseBottomBuffer();
                StartCoroutine(SmoothMoveMenu(indexOffset));
            }
        }
    }

    //one of the action gameobjects is taken from below the menu and moved to the top
    //done before a move. There should always be 2 actions loaded both above and below the menu
    private void IncreaseTopBuffer()
    {
        MinDisplayedIndex--;
        Transform lastChild = transform.GetChild(transform.childCount - 1);
        if (MinDisplayedIndex - 2 >= 0)
            lastChild.GetComponent<MenuAction>().action = this.AvailableActions.List[MinDisplayedIndex - 2];
        lastChild.SetAsFirstSibling();
        lastChild.localPosition = transform.GetChild(1).localPosition + Vector3.up * Spacing;
    }

    private void IncreaseBottomBuffer()
    {
        MinDisplayedIndex++;
        Transform firstChild = transform.GetChild(0);
        if (MaxDisplayedIndex + 2 < AvailableActions.List.Count)
            firstChild.GetComponent<MenuAction>().action = this.AvailableActions.List[MaxDisplayedIndex + 2];
        firstChild.SetAsLastSibling();
        firstChild.localPosition = transform.GetChild(PoolSize - 2).localPosition - Vector3.up * Spacing;
    }

    public IEnumerator SmoothMovePointer(int oldIndex)
    {
        SelectedIndexContinuous = oldIndex;
        while (true)
        {
            float UpdateAmmount = (SelectedIndex - oldIndex) * Time.deltaTime / MoveTime;
            SelectedIndexContinuous += UpdateAmmount;
            if (Mathf.Abs(SelectedIndexContinuous - SelectedIndex) < Mathf.Abs(UpdateAmmount))
            {
                SelectedIndexContinuous = SelectedIndex;
                SetPointerIconPosition();
                this.isMoving = false;
                break;
            }
            else
            { 
                SetPointerIconPosition();
                yield return null;
            }
        }

    }


    public IEnumerator SmoothMoveMenu(int offset){
        float ammountMoved = 0;
        while(true){
            float UpdateAmmount = Spacing * Time.deltaTime / MoveTime;
            transform.localPosition += Vector3.up * UpdateAmmount * offset;
            ammountMoved += UpdateAmmount;
            if(ammountMoved + UpdateAmmount > Spacing){
                transform.localPosition += Vector3.up * (Spacing - ammountMoved) * offset;
                this.isMoving = false;
                break;
            }
            else{
                yield return null;
            }
            
        }
    }

}
