using UnityEngine;

public class PlayerInteractionDetection : MonoBehaviour
{
    public delegate void Interact();
    public static Interact interact;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.GetComponent<Interactable>())
        {
            Debug.Log("Interacting with: " + collider.GetComponent<Interactable>().interactableName);
            interact += collider.GetComponent<Interactable>().OnInteract;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.GetComponent<Interactable>())
        {
            interact -= collider.GetComponent<Interactable>().OnInteract;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            interact?.Invoke();
        }
    }
}
