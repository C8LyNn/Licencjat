using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    [SerializeField]
    private static GameObject[] persistentObjects = new GameObject[2];
    public int objectIndex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (persistentObjects[objectIndex] == null)
        {
            persistentObjects[objectIndex] = gameObject;
            DontDestroyOnLoad(gameObject);
        }
        else if (persistentObjects[objectIndex] != gameObject)
        {
            Destroy(gameObject);
        }
    }
}
