using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum  PlaneOrientation
{
    XY,
    YZ,
    XZ
}

public class MaskTextMeshPro : MonoBehaviour
{
    public Material[] MaskableMaterials;
    public PlaneOrientation Orientation;


    // Start is called before the first frame update
    void Start()
    {
        UpdateMask();
    }



    [ContextMenu("Update the mask")]
    public void UpdateMask(){
        int propertyId = Shader.PropertyToID("_AlphaClipping");
        
        Vector4 bounds = Vector4.zero;
        Vector3 startPoint = this.transform.position;
        Vector3 endPoint =  this.transform.position + new Vector3(this.transform.localScale.x,this.transform.localScale.y,0);

        switch(Orientation){
            case PlaneOrientation.XY:
               bounds =  new Vector4(startPoint.x,startPoint.y,endPoint.x,endPoint.y);
            break;
            case PlaneOrientation.YZ:
               bounds =  new Vector4(startPoint.y,startPoint.z,endPoint.y,endPoint.z);
            break;
            case PlaneOrientation.XZ:
               bounds =  new Vector4(startPoint.x,startPoint.z,endPoint.x,endPoint.z);
            break;
        }
        
        
        foreach(Material mat in MaskableMaterials){
            mat.SetVector(propertyId,bounds);
        }

    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + transform.localScale/2, transform.localScale);    
    }
}
