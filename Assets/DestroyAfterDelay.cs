using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterDelay : MonoBehaviour
{
    public float Delay = 5f;
    private float TimeSinceAwake;
    private void Awake() {
        TimeSinceAwake = 0;
    }

    void Update(){
        TimeSinceAwake += Time.deltaTime;
        if(TimeSinceAwake > Delay)
            Object.Destroy(this.gameObject);
    }
}
