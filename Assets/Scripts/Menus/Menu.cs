using System;
using UnityEngine;

public abstract class Menu : MonoBehaviour {
    public MenuRouter MenuRouter;

    public virtual void OnEnable(){
        this.MenuRouter.EnabledMenus.Add(this.name,this);
    }

    public virtual void OnDisable(){
        this.MenuRouter.EnabledMenus.Remove(this.name);
    }

    public bool IsOpen{get;set;}

    /// <summary>
    /// Disable Input while keeping the window visible
    /// </summary>
    public abstract void DisableInput();

    /// <summary>
    /// Enable Input while changing nothing else
    /// </summary>
    public abstract void EnableInput();

    /// <summary>
    /// Hide the window and disable input
    /// </summary>
    public abstract void Hide();

    /// <summary>
    /// Show the window and enable input
    /// </summary>
    public abstract void Show();

    public void ShowDialogue(){
        if(MenuRouter.OpenedMenus.Count > 0){
            MenuRouter.OpenedMenus.Peek().DisableInput();
        }
        Show();
        this.MenuRouter.OpenedMenus.Push(this);
    }

    public void ShowDialogueAndHideLastMenu(){
        if(MenuRouter.OpenedMenus.Count > 0){
            MenuRouter.OpenedMenus.Peek().Hide();
        }
        Show();
        this.MenuRouter.OpenedMenus.Push(this);
    }

    /// <summary>
    /// Closes the current menu and reopens the caller menu
    /// </summary>
    public void CloseMenuAndReOoenCaller(){
        if(this.MenuRouter.OpenedMenus.Peek() != this){
            throw new Exception("Error! Menu was told to close but is not the most recently opened menu.");
        }
        this.MenuRouter.OpenedMenus.Pop();
        Menu callerMenu = this.MenuRouter.OpenedMenus.Peek();
        if(callerMenu != null)
            callerMenu.Show();
        this.Hide();
    }

    /// <summary>
    /// Closes all open menus including the current menu. Useful for when you want to make sure all menus stay closed
    /// </summary>
    public void CloseAllMenus(){
        this.MenuRouter.CloseAllMenus();
    }

}
