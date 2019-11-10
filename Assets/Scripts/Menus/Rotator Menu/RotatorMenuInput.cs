using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorMenuInput : MonoBehaviour
{
    private RotatorMenu RotatorMenu;
    

    void Start(){
        RotatorMenu = GetComponent<RotatorMenu>();
    }


    // Update is called once per frame
    public void Update() 
    {
        if(Input.GetAxis("Vertical") < 0){
            RotatorMenu.NextAction();
        }
        else if(Input.GetAxis("Vertical") > 0){
            RotatorMenu.PreviousAction();
        }

        if(!RotatorMenu.IsRotating && Input.GetButtonDown("Submit")){
            RotatorMenu.ActionContainers[RotatorMenu.SelectedActionIndex].ActivateSelectedAction();
        }

        if(!RotatorMenu.IsRotating && Input.GetButtonDown("SpinSwap")){
            Player player = (Player)RotatorMenu.ActingUnitRef.Value;
            if(player.IsPartnerAbleToAct())
                player.SwapUnitOrder(RotatorMenu);
            else
                Debug.Log("Other unit is not able to act. Aborting swap operation");
        }
    }
}
