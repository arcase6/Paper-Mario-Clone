using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSpawner : MonoBehaviour
{
    public GameObject PrefabToSpawn;

    
    public Vector3 EffectOffset = Vector3.zero;
    // Start is called before the first frame update
    void OnEnable(){
        if(!PrefabToSpawn){
            this.enabled = false;
            return;
        }
        var instantiated = Instantiate(PrefabToSpawn,Vector3.zero,Quaternion.identity,transform);
        instantiated.transform.localPosition = Vector3.zero;
        instantiated.transform.position = instantiated.transform.position + EffectOffset;
    }

    public void EndEffect(){
        this.enabled = false;
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
