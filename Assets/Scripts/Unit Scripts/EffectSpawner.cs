using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSpawner : MonoBehaviour
{
    public ItemAction ParentItem; //stores a reference to the using item if necessary
    private float TimeElapsed;
    private bool EffectApplied;

    public Action ItemApplyCallback { get; internal set; }

    // Start is called before the first frame update
    void OnEnable(){
        if(!ParentItem?.PrefabToSpawn){
            this.enabled = false;
            return;
        }
        var instantiated = Instantiate(ParentItem?.PrefabToSpawn,Vector3.zero,Quaternion.identity,transform);
        instantiated.transform.localPosition = Vector3.zero;
        instantiated.transform.position = instantiated.transform.position + ParentItem.PrefabOffset;
        EffectApplied= false;
        TimeElapsed = 0;
    }
    public void EndEffect(){
        this.enabled = false;
    }

    private void OnDisable() {
        this.ParentItem = null; // clear parent item reference after use   
        if(!EffectApplied)
            ItemApplyCallback(); // delay was longer than item lifetime
    }

    // Update is called once per frame
    void Update()
    {
        TimeElapsed += Time.deltaTime;
        if(!EffectApplied && TimeElapsed > ParentItem.EffectApplicationDelay){
            EffectApplied = true;
            ItemApplyCallback();
        }
    }
}
