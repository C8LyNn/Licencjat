using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Teleporter : MonoBehaviour
{
    [Header("Wartoœci 0 bêd¹ ignorowane")]
    public int teleportID;
    public Vector3 teleportCoordinates;

    private void ActivateThis()
    {
        TeleportPlayer();
    }

    private void Start()
    {
        if(teleportID == 0 && teleportCoordinates == Vector3.zero)
        {
            Debug.LogWarning(gameObject.name + " " + "(X:" + gameObject.transform.position.x + " Y:" + gameObject.transform.position.y + ") has no second teleport data set");
        }
    }

    private void TeleportPlayer()
    {
        if (teleportID != 0)
        {
            foreach (Teleporter teleport in FindObjectsByType<Teleporter>(FindObjectsSortMode.None))
            {
                if (teleport.teleportID == teleportID && teleport.gameObject != gameObject)
                {
                    FindAnyObjectByType<PlayerController>().TeleportTo(teleport.gameObject, null);
                }
            }
        }
        else if (teleportCoordinates != Vector3.zero)
        {
            FindAnyObjectByType<PlayerController>().TeleportTo(null, teleportCoordinates);
        }

    }
}
