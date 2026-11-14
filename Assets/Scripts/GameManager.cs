using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject teleportParent;

    public static List<SceneChanger> teleportList = new List<SceneChanger>();

    private void Awake()
    {
        if(teleportParent == null)
        {
            teleportParent = GameObject.Find("Teleports");
        }

        teleportList.Clear();
        if (teleportParent != null)
        {
            foreach (SceneChanger teleport in teleportParent.GetComponentsInChildren<SceneChanger>())
            {
                teleportList.Add(teleport);
                //Debug.Log("GAMEMANAGER: " + teleport.sceneToGo);
            }
        }
        else Debug.Log("Teleport parent object not set in GameManager");
    }
}
