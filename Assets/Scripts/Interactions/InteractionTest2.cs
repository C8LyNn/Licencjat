using UnityEngine;

public class InteractionTest2 : MonoBehaviour
{
    public void ActivateThis()
    {
        Debug.Log("It certainly works with InteractionTest2");
        Debug.Log(gameObject.GetComponent<Interactable>().interactableName + " has been interacted with");
    }
}
