using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    public string sceneToGo;
    public int teleportID;

    public enum layer {Floor0, Floor1, Floor2, Floor3};
    [SerializeField] layer layerToGo;

    [HideInInspector]
    public string sceneName;

    private void Awake()
    {
        sceneName = SceneManager.GetActiveScene().name;

        if (sceneToGo == "" && SceneManager.GetActiveScene().name != "LayerNull_NoSceneSet")
        {
            sceneToGo = "LayerNull_NoSceneSet";
            Debug.LogWarning("Teleport -" + gameObject.name + "- at: (X:" + transform.position.x + " Y:" + transform.position.y + ") is not set");
        }
        else if (sceneToGo == "") sceneToGo = PlayerController.previousScene;

        if (!gameObject.GetComponent<BoxCollider2D>()) //adds box collider if it doesn't exist
        {
            gameObject.AddComponent<BoxCollider2D>();
            gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
        }
        else if (gameObject.GetComponent<BoxCollider2D>().isTrigger == false) gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void ActivateThis()
    {
        StartCoroutine(TeleportToScene());
    }

    private IEnumerator TeleportToScene()
    {
        if (PlayerController.canTeleport)
        {
            StartCoroutine(FindFirstObjectByType<PlayerController>().FadeInImage());
            yield return new WaitForSeconds(FindFirstObjectByType<PlayerController>().fadeTimeInSeconds / 2 + 0.5f);
            PlayerController.canTeleport = false;
            PlayerController.previousScene = sceneName;
            PlayerController.previousID = teleportID;
            SceneManager.LoadScene(sceneToGo);
        }
    }
}
