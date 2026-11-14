using UnityEngine;

public class CameraConfinerProvider : MonoBehaviour
{
    public Collider2D confiner;

    private void Awake()
    {
        if (confiner == null)
        {
            confiner = gameObject.GetComponent<Collider2D>();
        }
    }
}
