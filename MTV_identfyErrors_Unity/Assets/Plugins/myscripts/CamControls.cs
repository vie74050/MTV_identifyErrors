using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControls : MonoBehaviour
{
    // add additional keyboard am controls
    private KGFOrbitCam camsettings;
    public KeyCode keyZoomIn;
    public KeyCode keyZoomOut;

    void Start()
    {
        camsettings = Camera.main.GetComponent<KGFOrbitCam>();
    }

    void Update()
    {
        float z = camsettings.GetZoomCurrent();
        float sensitivity = camsettings.GetZoomAxisSensitivity(0)*0.1f;
       // Debug.Log(z + ", " + sensitivity);
        
        if (Input.GetKey(keyZoomIn))
        {
            
            if (z - sensitivity > camsettings.GetZoomMinLimit())
            {
                camsettings.SetZoom(z-sensitivity);
            }
            
        }

        if (Input.GetKey(keyZoomOut))
        {

            if (z + sensitivity < camsettings.GetZoomMaxLimit())
            {
                camsettings.SetZoom(z + sensitivity);
            }

        }
        /*      
        if ( Input.GetMouseButton(0)
             || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.UpArrow)
             || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow)
            )
        {
            camsettings.SetPanningEnable(true); Debug.Log(Event.current);
        }
        else
        {
            camsettings.SetPanningEnable(false);
        } */
    }
   
}
