using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaceInfoManager : MonoBehaviour
{
    public TextMeshProUGUI placeText;

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += TrimInfo;
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= TrimInfo;
    }
    private void TrimInfo(Scene oldScene, Scene newScene)
    {
        string[] stringParts = SceneManager.GetActiveScene().name.Split("_");

        string layerPart = stringParts[0].Substring(5);
        string roomPart = stringParts[1];

        placeText.text = $"Floor: {layerPart} | Room: {roomPart}";
    }
}
