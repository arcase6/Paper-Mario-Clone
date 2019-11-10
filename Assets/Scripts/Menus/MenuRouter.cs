using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MenuRouter", menuName = "Paper Mario Clone/MenuRouter", order = 0)]
public class MenuRouter : ScriptableObject {
    public Stack<Menu> OpenedMenus = new Stack<Menu>();

    public Dictionary<string, Menu> EnabledMenus = new Dictionary<string, Menu>();

    public void CloseAllMenus(){
        while(OpenedMenus.Count > 0){
            Menu top = OpenedMenus.Pop();
            if(top.IsOpen)
                top.Hide();
        }
    }

    
}