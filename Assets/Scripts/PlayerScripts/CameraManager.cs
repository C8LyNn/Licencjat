using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cCamera;

    private CinemachineConfiner2D confiner;

    void Awake()
    {
        if(cCamera == null)
        {
            cCamera = gameObject.GetComponentInChildren<CinemachineCamera>();
        }
        confiner = cCamera.GetComponent<CinemachineConfiner2D>();
    }

    public void SetConfiner(Collider2D newConfiner)
    {
        confiner.BoundingShape2D = newConfiner;
        Debug.Log("Cinemachine Confiner set");
    }

    public void SetTrackingTarget(Transform newTracker)
    {
        cCamera.Follow = newTracker;
        Debug.Log("Cinemachine Tracking Target set");
    }
}
