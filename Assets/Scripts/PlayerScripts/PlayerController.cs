using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    #region Movement Data
    [Header("Movement Data")]

    public float moveSpeed = 5f;
    public Transform movePoint;
    public LayerMask stopsMovement;

    public bool leftHitbox = false;
    public bool rightHitbox = false;
    public bool upHitbox = false;
    public bool downHitbox = false;

    #endregion

    #region Interaction Data
    [Header("Interaction Data")]
    public BoxCollider2D playerInteractor;
    public GameObject playerInteractorSprite;

    #endregion

    #region Teleport Data

    public static bool canTeleport = true;

    public static string previousScene;

    public static int previousID;

    #endregion

    #region Camera Data
    [Header("Camera Data")]

    [SerializeField] private CameraManager camManager;

    #endregion

    #region Debug Data
    [Header("Debug Data")]

    public Transform debugPlayerTransform;

    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        movePoint.parent = null;
        SceneManager.activeSceneChanged += SetPlayerPositionOnSceneChange;
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= SetPlayerPositionOnSceneChange;
    }
    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
    }

    #region Movement
    private void PlayerMovement()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, movePoint.position) == 0f)
        {
            CheckForObstacles();
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                if (playerInteractorSprite != null)
                {
                    playerInteractorSprite.transform.position = gameObject.transform.position + new Vector3(0.5f, 0f, 0f);
                }
                playerInteractor.offset = new Vector2(0.5f, 0f);

                if (!rightHitbox)
                {
                    MoveRight();
                }

            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                if (playerInteractorSprite != null)
                {
                    playerInteractorSprite.transform.position = gameObject.transform.position + new Vector3(-0.5f, 0f, 0f);
                }
                playerInteractor.offset = new Vector2(-0.5f, 0f);

                if (!leftHitbox)
                {
                    MoveLeft();
                }

            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                if (playerInteractorSprite != null)
                {
                    playerInteractorSprite.transform.position = gameObject.transform.position + new Vector3(0f, 0.5f, 0f);
                }
                playerInteractor.offset = new Vector2(0f, 0.5f);

                if (!upHitbox)
                {
                    MoveUp();
                }
            }

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                if (playerInteractorSprite != null)
                {
                    playerInteractorSprite.transform.position = gameObject.transform.position + new Vector3(0f, -0.5f, 0f);
                }
                playerInteractor.offset = new Vector2(0f, -0.5f);

                if (!downHitbox)
                {
                    MoveDown();
                }
            }
        }
    }

    private void MoveRight()
    {
        if (!Physics2D.OverlapCircle(movePoint.position + Vector3.right, .2f, stopsMovement))
        {
            movePoint.position += Vector3.right;
        }
    }

    private void MoveLeft()
    {
        if (!Physics2D.OverlapCircle(movePoint.position + Vector3.left, .2f, stopsMovement))
        {
            movePoint.position += Vector3.left;
        }
    }

    private void MoveUp()
    {

        if (!Physics2D.OverlapCircle(movePoint.position + Vector3.up, .2f, stopsMovement))
        {
            movePoint.position += Vector3.up;
        }
    }

    private void MoveDown()
    {

        if (!Physics2D.OverlapCircle(movePoint.position + Vector3.down, .2f, stopsMovement))
        {
            movePoint.position += Vector3.down;
        }
    }

    private void CheckForObstacles()
    {

        RaycastHit2D raycastLeft = Physics2D.Raycast(gameObject.transform.position, Vector3.left, 1f, stopsMovement);
        if (raycastLeft)
        {
            leftHitbox = true;
            //Debug.Log("left hit");
        }
        else
        {
            leftHitbox = false;
        }

        RaycastHit2D raycastRight = Physics2D.Raycast(gameObject.transform.position, Vector3.right, 1f, stopsMovement);
        if (raycastRight)
        {
            rightHitbox = true;
            //Debug.Log("right hit");
        }
        else
        {
            rightHitbox = false;
        }

        RaycastHit2D raycastUp = Physics2D.Raycast(gameObject.transform.position, Vector3.up, 1f, stopsMovement);
        if (raycastUp)
        {
            upHitbox = true;
            //Debug.Log("up hit");
        }
        else
        {
            upHitbox = false;
        }

        RaycastHit2D raycastDown = Physics2D.Raycast(gameObject.transform.position, Vector3.down, 1f, stopsMovement);
        if (raycastDown)
        {
            downHitbox = true;
            //Debug.Log("down hit");
        }
        else
        {
            downHitbox = false;
        }

        /*Debug.DrawRay(gameObject.transform.position, Vector3.down, Color.red);
        Debug.DrawRay(gameObject.transform.position, Vector3.up, Color.red);
        Debug.DrawRay(gameObject.transform.position, Vector3.left, Color.red);
        Debug.DrawRay(gameObject.transform.position, Vector3.right, Color.red);*/

    }

    #endregion

    #region On Scene Change Logic
    private void SetPlayerPositionOnSceneChange(Scene oldScene, Scene newScene)
    {
        StartCoroutine(SetupCamera());
        foreach (SceneChanger sc in GameManager.teleportList)
        {
            if (sc.sceneToGo == previousScene)
            {
                if (sc.teleportID == previousID)
                {
                    gameObject.transform.position = sc.transform.position;
                    movePoint.transform.position = sc.transform.position;
                }
            }
        }
    }

    IEnumerator TeleportCooldown()
    {
        yield return new WaitForSeconds(0.1f);
        canTeleport = true;
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.GetComponent<Interactable>() != null)
        {
            if (other.GetComponent<Interactable>().onWalkIn)
            {
                StartCoroutine(TeleportCooldown());
            }
        }
    }

    #endregion

    #region Interaction Logic

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Interactable>() && other.GetComponent<Interactable>().onWalkIn)
        {
            other.GetComponent<Interactable>().OnInteract();
        }
    }

    #endregion

    #region Camera Functions

    private void FindCameraManager()
    {
        if (camManager == null)
        {
            camManager = FindFirstObjectByType<CameraManager>();
        }
        else Debug.Log("Camera manager already set");
    }
    private void SetupCameraConfiner()
    {
        CameraConfinerProvider provider = FindFirstObjectByType<CameraConfinerProvider>();
        if (provider != null)
        {
            camManager.SetConfiner(provider.confiner);
        }
        else Debug.Log("No camera confiner in scene");
    }

    private void SetupCameraTarget()
    {
        Transform transform = gameObject.transform;
        debugPlayerTransform = transform;
        if (transform != null)
        {
            camManager.SetTrackingTarget(transform);
        }
        else Debug.Log("No available tracking target in scene");
    }

    private IEnumerator SetupCamera()
    {
        yield return null;
        FindCameraManager();
        SetupCameraConfiner();
        SetupCameraTarget();
    }

    #endregion

    #region Teleporter Logic

    public void TeleportTo(GameObject teleport, Vector3? coordinates)
    {
        if (canTeleport)
        {
            if (coordinates != null)
            {
                gameObject.transform.position = coordinates.Value;
                movePoint.transform.position = coordinates.Value;
                canTeleport = false;
            }
            else if (teleport != null)
            {
                gameObject.transform.position = teleport.transform.position;
                movePoint.transform.position = teleport.transform.position;
                canTeleport = false;
            }
            else
            {
                Debug.LogWarning("Empty teleporter. Set another with the same teleportID or set coordinates");
            }
        }
    }

    #endregion
}
