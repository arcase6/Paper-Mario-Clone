using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollMenuHolder : Menu
{
    private ScrollMenuInput InputComponent;

    void Start(){
        InputComponent = GetComponentInChildren<ScrollMenuInput>(true);
    }
    public override void Hide()
    {
        IsOpen = false;
        this.transform.GetChild(0).gameObject.SetActive(false);
        DisableInput();
    }

    public override void DisableInput()
    {
        this.InputComponent.enabled = false;
    }

    public override void EnableInput()
    {
        this.InputComponent.enabled = true;
    }

    public override void Show()
    {
        IsOpen = true;
        EnableInput();
        this.transform.GetChild(0).gameObject.SetActive(true);
        
    }
}
