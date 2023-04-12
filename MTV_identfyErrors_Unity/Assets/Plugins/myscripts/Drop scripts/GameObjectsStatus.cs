﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameObjectsStatus
{
    [Tooltip("Game Object to set active/inactive")]
    public GameObject go;
    [Tooltip("Active setting to set")]
    public bool status;

    public void SetActiveStatus()
    {
        go.SetActive(status);
       
    }
    public void ColliderEnable()
    {
        go.GetComponent<Collider>().enabled = status;

    }
}
