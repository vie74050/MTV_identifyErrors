using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop_TrocarSeal : MonoBehaviour, IDropHandler
{
    public Collider dropzone_trocar;

    private bool assemblyComplete = false;
    private SelectableObject parentSO;

    public void Init()
    {
        dropzone_trocar.enabled = false;
    }

    public void eDrop(Transform target)
    {
        // check if parent is complete
        parentSO = target.parent.GetComponent<SelectableObject>();

        dropzone_trocar.enabled = parentSO.isTaskComplete;
        
        //print(parentSO.transform.name + ": " + parentSO.isTaskComplete);
    }

    void Update()
    {
        if (parentSO != null && !dropzone_trocar.enabled)
        {
            if (parentSO.isTaskComplete)
            {
                dropzone_trocar.enabled = true;
                //print(parentSO.name);
            }
        }
    }

}
