using UnityEngine;
using System.Collections.Generic;

public class PlayerGroup : MonoBehaviour
{
    //Position 1 is the farthest position to the opposing team: _ _ _ _ | _ _ _ _<- position 1
    //So 4th positions are always next to each other
    //It's done this way because the player is always on the position 1 in our team
    public List<GameObject> playerList;

    public static PlayerGroup playerGroup { get; private set; }

    private void Start()
    {
        playerGroup = this;
        DontDestroyOnLoad(playerGroup);
    }
}