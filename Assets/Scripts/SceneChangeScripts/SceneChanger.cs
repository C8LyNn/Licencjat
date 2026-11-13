using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string sceneToGo;

    [HideInInspector]
    public string sceneName;

    private void Awake()
    {
        sceneName = SceneManager.GetActiveScene().name;

        if (sceneToGo == "" && SceneManager.GetActiveScene().name != "NoSceneSet")
        {
            sceneToGo = "NoSceneSet";
        }
        else if (sceneToGo == "" ) sceneToGo = PlayerController.previousScene;

        if (!gameObject.GetComponent<BoxCollider2D>()) //adds box collider if it doesn't exist
        {
            gameObject.AddComponent<BoxCollider2D>();
            gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
        }
        else if (gameObject.GetComponent<BoxCollider2D>().isTrigger == false) gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
    }
}
