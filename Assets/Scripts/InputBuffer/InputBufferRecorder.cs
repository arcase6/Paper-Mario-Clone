using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBufferRecorder : MonoBehaviour
{
    public InputBuffer InputBuffer;

    public List<string> InputNames;
    private void OnEnable() {
        InputBuffer.FlushBuffer();
    }

    //Listen for input and push to buffer if input occurs
    private void Update() {
        float recordTime = Time.time;
        foreach(string name in InputNames){
            if(Input.GetButtonDown(name)){
                InputBuffer.RecordInput(new InputBufferData(){
                    InputName = name,
                    Time = recordTime
                });
            }
        }
    }
}
