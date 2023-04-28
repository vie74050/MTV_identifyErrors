using UnityEngine;
using System.Collections.Generic;

/* Assign game objects that are variations of each other.  On activity start, randomly choose one from the set and deactivates the others. */

public class ErrorObjects : MonoBehaviour {
    
	[Tooltip("Assign game object variants")]
	public List<GameObject> itemsForRandomEnable=new List<GameObject>();

	void Awake () {
		// randomly choose one version to use
		if (itemsForRandomEnable!=null && itemsForRandomEnable.Count>0)
         {
             int itemId=new System.Random().Next(itemsForRandomEnable.Count);
             for (int i=0; i<itemsForRandomEnable.Count;i++)
             {
                 if (i==itemId){itemsForRandomEnable[i].gameObject.SetActive(true);}
                 else{itemsForRandomEnable[i].gameObject.SetActive(false);}
             }
         }
	}

}