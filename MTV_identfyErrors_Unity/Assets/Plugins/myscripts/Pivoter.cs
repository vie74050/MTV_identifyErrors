using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Extends Selectable Object: add click to pivot specified items about {axis}, {point}
 *  - distinguish between click to toggle pivot items vs base select/drag by mousedown duration
 *  - add kinematic rigidvody to have all colliders behave as one
 * 
 */
public class Pivoter : SO_linkedObject
{
    public bool isOpen = false;

    [Tooltip("Items to rotate")]
    public PivotItem[] pvObjs;

    private float mouseDowndT = 0;
    private int mouseDowndT_limit = 10;
    private bool initOpenState = false;

    void Awake()
    {
        int i = 0;
        linkedObjects = new GameObject[pvObjs.Length];

        // add all pv objs as linked objects
        foreach (PivotItem p in pvObjs)
        {
            GameObject pgo = p.t.gameObject;

            linkedObjects[i] = (pgo);
            i++;
        }
        initOpenState = isOpen;
        //base.Init();       
    }
    public override void ResetPosition()
    {
        base.ResetPosition();
        
        if (isOpen != initOpenState)
        {
            ToggleOpen();
        }
        
    }
    public override bool IsTaskComplete()
    {
        isTaskComplete = isOpen != initOpenState;

        return base.IsTaskComplete();
    }
    private new void OnMouseDrag()
    {
        mouseDowndT++;
        if (gameObject.tag == "Draggable")
        {
            base.OnMouseDrag();

            //print("MOUSE drag");
        }

    }
    private new void OnMouseUp()
    {
        if (mouseDowndT < mouseDowndT_limit)
        {
            ToggleOpen();
        }
        else
        {
            base.OnMouseUp();
        }
       
        mouseDowndT = 0;
    }

    private void ToggleOpen()
    {
       foreach (PivotItem pi in pvObjs)
        {
            if (isOpen) pi.PivotAround(-pi.max);
            else pi.PivotAround(pi.max);
        }

        isOpen = !isOpen;
    }

}

[System.Serializable]
public class PivotItem
{
    public float max = 30;
    public Transform t;
    public Transform pivot; // optional
    public Vector3 rotaxis = Vector3.forward;
       
    public void PivotAround(float amt)
    {
        Vector3 pivotPt = (pivot != null)? pivot.position : t.position;
        Vector3 axis;
        
        if (pivot != null)
        {
            axis = pivot.right * rotaxis.x + pivot.up * rotaxis.y + pivot.forward * rotaxis.z;
        }
        else
        {
            axis = t.right * rotaxis.x + t.up * rotaxis.y + t.forward * rotaxis.z;
        }
        
        t.RotateAround(pivotPt, axis, amt );
    }
}