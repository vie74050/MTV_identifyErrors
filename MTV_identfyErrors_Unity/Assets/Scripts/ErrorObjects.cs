using UnityEngine;
using System.Collections.Generic;

/* Assign game objects that are variations of each other.  On activity start, randomly choose one from the set and deactivates the others. */

public class ErrorObjects : MonoBehaviour {
    
	[Tooltip("Assign game object variants. The first object should be the default (non-error)")]
	public List<GameObject> itemsForRandomEnable=new List<GameObject>();
   
    [Tooltip("The index from list to show. Set to -1 for random.")]
    public int desiredIndex = -1;

	void Awake () {
        // for editor testing
        SetDesiredIndex(desiredIndex);                
	}

    public void SetDesiredIndex(int i) {
        desiredIndex = i;

        if (desiredIndex>=0) {
            ShowSelectedIndex(desiredIndex);
        }else{
            RandomSelect();
        }
    }
    private void RandomSelect() {
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

    /** Activate first item in list, deactivate others.
    *   - expect first item in list to be non-error, index=0
    * @param {int} index The index to enable from itemsForRandomEnable
    */
    private void ShowSelectedIndex(int index = 0) {

        if (itemsForRandomEnable!=null && itemsForRandomEnable.Count>0)
         {
             for (int i=0; i<itemsForRandomEnable.Count;i++)
             {
                 if (i==index){itemsForRandomEnable[i].gameObject.SetActive(true);}
                 else{itemsForRandomEnable[i].gameObject.SetActive(false);}
             }
             Debug.Log("no errors");
         }
    }

}