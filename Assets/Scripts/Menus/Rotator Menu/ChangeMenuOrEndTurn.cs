using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChangeMenuOrEndTurn : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        TurnManager turnManagerRef = animator.GetComponentInParent<StageDirector>().TurnManagerRef;

        //check if the unit turns are still remaining
        //End turn if no more units left to act
        if (!turnManagerRef.Players.Any(p => p.CanAct))
        {
            turnManagerRef.StartEnemyTurn();
        }
        else
        {

            RotatorMenu rotatorMenu = turnManagerRef.RotatorMenu;
            Stack<Menu> openedMenuStack = rotatorMenu.MenuRouter.OpenedMenus;
            if (openedMenuStack.Count == 0 || openedMenuStack.Peek() != rotatorMenu)
                rotatorMenu.ShowDialogue();
            else
                rotatorMenu.Show();
        }
    }
}
