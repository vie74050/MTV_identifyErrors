using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop_SetGOActive : MonoBehaviour, IDropHandler
{
    [Tooltip("GOs to set active on drop")]
    public List<GameObjectsStatus> GOsToSetOnDrop;

    public void Init()
    {
        
    }
    public void eDrop(Transform target)
    {

        if (GOsToSetOnDrop.Count > 0)
        {
            foreach (GameObjectsStatus _gos in GOsToSetOnDrop)
            {
                _gos.SetActiveStatus();
            }
        }
    }

}
