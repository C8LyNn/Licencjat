using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string interactableName;
    public MonoBehaviour scriptToActivate;

    private void Awake()
    {
        if (!gameObject.GetComponent<BoxCollider2D>()) //adds box collider if it doesn't exist
        {
            gameObject.AddComponent<BoxCollider2D>();
            gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
        }
        else if (gameObject.GetComponent<BoxCollider2D>().isTrigger == false) gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
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
