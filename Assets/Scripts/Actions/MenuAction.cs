using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAction : MonoBehaviour
{
    [SerializeField]
    private ActionData actionData;

    public ActionData action{
        get{
            return actionData;
        }
        set{
            actionData = value;
            UpdateDisplay();
        }
    }

    public SpriteRenderer IconRenderer;

    public StringMediator NameDisplay;
    public StringMediator CostDisplay;

    [ContextMenu("Update Display")]
    public void UpdateDisplay(){
        IconRenderer.sprite = this.actionData?.Icon;
        NameDisplay.Value = this.actionData?.name;
        CostDisplay.Value = this.actionData?.CostDisplay;
    }


    // Start is called before the first frame update
    void Start()
    {;
        if(actionData)
            UpdateDisplay();
    }

    
}
