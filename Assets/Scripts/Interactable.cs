using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] public string interactableName;
    [SerializeField] public MonoBehaviour scriptToActivate;

    [Header("Interaction Type")]
    [SerializeField] public bool onClick;
    [SerializeField] public bool onWalkIn;

    private void Awake()
    {
        if (!gameObject.GetComponent<BoxCollider2D>()) //adds box collider if it doesn't exist
        {
            gameObject.AddComponent<BoxCollider2D>();
            gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
        }
        else if (gameObject.GetComponent<BoxCollider2D>().isTrigger == false) gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
 
        if(interactableName == "")
        {
            interactableName = gameObject.name;
        }
    }

    private void Start()
    {
        if(!onClick && !onWalkIn)
        {
            Debug.LogWarning("Interactable --" + gameObject.name + "-- (X:" + gameObject.transform.position.x + "Y:" + gameObject.transform.position.y + ") has no set Interaction Type");
        }
    }

    public void OnInteract()
    {
        scriptToActivate.SendMessage("ActivateThis");
    }

    private void DebugFunction()
    {
        Debug.Log(interactableName + " has been interacted with");
    }
}
