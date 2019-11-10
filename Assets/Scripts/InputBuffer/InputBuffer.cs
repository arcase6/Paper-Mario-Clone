using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class InputBufferData{
    public string InputName = "!"; //This is a special null value
    public float Time;
}

public class InputBufferEnumerator : IEnumerator<InputBufferData>
{
    private InputBuffer BufferRef;
    private int CurrentPos;
    private int iterCount = 0;
    public InputBufferEnumerator(InputBuffer bufferRef){
        this.BufferRef = bufferRef;
        this.CurrentPos = BufferRef.Head;
        iterCount = 0;
    }


    public InputBufferData Current => BufferRef.PastInput[CurrentPos];

    object IEnumerator.Current => Current;

    public void Dispose()
    {
        
    }

    public bool MoveNext()
    {
        CurrentPos--;
        if(CurrentPos < 0)
            CurrentPos = InputBuffer.BufferSize - 1;
        iterCount++;
        bool valid = iterCount <= InputBuffer.BufferSize;
        valid = valid && !BufferRef.PastInput[CurrentPos].InputName.Equals("!");
        return valid;
    }

    public void Reset()
    {
        CurrentPos = BufferRef.Head;
        iterCount = 0;
    }
}


[CreateAssetMenu(fileName = "InputBuffer", menuName = "Paper Mario Clone/InputBuffer", order = 0)]
public class InputBuffer : ScriptableObject, IEnumerable<InputBufferData> {
    public const int BufferSize = 10;
    

    [HideInInspector]
    public InputBufferData[] PastInput = new InputBufferData[BufferSize];

    [HideInInspector]
    public int Head = BufferSize;

    public void RecordInput(InputBufferData inputData){
        PastInput[Head] = inputData;
        Head = (Head + 1) % BufferSize;
    }


    public void FlushBuffer(){
        for(int i = 0;i < BufferSize;i++){
            PastInput[i].InputName="!";
        }
        Head = 0;
    }

    public IEnumerator<InputBufferData> GetEnumerator()
    {
        return new InputBufferEnumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new InputBufferEnumerator(this);
    }
}