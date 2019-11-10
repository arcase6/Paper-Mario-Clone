using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Monobehavior that handles setting rotater menu items
public class RotationSetter : MonoBehaviour
{

    public RotatorMenu RotaterMenu;

    public float SelectedItem;

    public float StallAmmount = 0;

    public float StallAngle = 10;

    private void Reset() {
        RotaterMenu = transform.parent.GetComponent<RotatorMenu>();

    }


    public void SetRotation(){
        Quaternion rotation = Quaternion.Euler(0,0, (SelectedItem * RotaterMenu.AngularSpacing) + (StallAmmount * StallAngle));
        this.transform.localRotation = rotation;
        foreach(TopLevelAction action in RotaterMenu.ActionContainers){
            action.transform.rotation = Quaternion.identity;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        this.SelectedItem = RotaterMenu.SelectedActionIndex;
        SetRotation();
    }

    // Update is called once per frame
    void Update()
    {
        SetRotation();
    }
}
