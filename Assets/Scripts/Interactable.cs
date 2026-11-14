using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string interactableName;

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
        DebugFunction();
    }

    private void DebugFunction()
    {
        Debug.Log(interactableName + " has been interacted with");
    }
}
