using UnityEngine;

public class InteractionTester : MonoBehaviour
{
    public void ActivateThis()
    {
        Debug.Log("It certainly works with InteractionTester");
        Debug.Log(gameObject.GetComponent<Interactable>().interactableName + " has been interacted with");
    }
}
