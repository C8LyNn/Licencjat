using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    #region Movement Variables

    public float moveSpeed = 5f;
    public Transform movePoint;
    public LayerMask stopsMovement;

    public bool leftHitbox = false;
    public bool rightHitbox = false;
    public bool upHitbox = false;
    public bool downHitbox = false;

    #endregion

    #region Interaction Variables

    public BoxCollider2D playerInteractor;

    #endregion

    #region Teleport Variables

    [SerializeField]
    private bool canTeleport = true;

    [HideInInspector]
    public static string previousScene;

    [HideInInspector]
    public static int previousID;

    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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
            if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && !rightHitbox)
            {
                MoveRight();
            }

            if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && !leftHitbox)
            {
                MoveLeft();
            }

            if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && !upHitbox)
            {
                MoveUp();
            }

            if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && !downHitbox)
            {
                MoveDown();
            }
        }
    }

    private void MoveRight()
    {
        playerInteractor.offset = new Vector2(0.5f, 0f);
        if (!Physics2D.OverlapCircle(movePoint.position + Vector3.right, .2f, stopsMovement))
        {
            movePoint.position += Vector3.right;
        }
    }

    private void MoveLeft()
    {
        playerInteractor.offset = new Vector2(-0.5f, 0f);
        if (!Physics2D.OverlapCircle(movePoint.position + Vector3.left, .2f, stopsMovement))
        {
            movePoint.position += Vector3.left;
        }
    }

    private void MoveUp()
    {
        playerInteractor.offset = new Vector2(0f, 0.5f);
        if (!Physics2D.OverlapCircle(movePoint.position + Vector3.up, .2f, stopsMovement))
        {
            movePoint.position += Vector3.up;
        }
    }

    private void MoveDown()
    {
        playerInteractor.offset = new Vector2(0f, -0.5f);
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

    #region Teleport Logic
    private void SetPlayerPositionOnSceneChange(Scene oldScene, Scene newScene)
    {
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<SceneChanger>() && canTeleport)
        {
            canTeleport = false;
            previousScene = other.GetComponent<SceneChanger>().sceneName;
            previousID = other.GetComponent<SceneChanger>().teleportID;
            SceneManager.LoadScene(other.GetComponent<SceneChanger>().sceneToGo);
        }
    }

    IEnumerator TeleportCooldown()
    {
        yield return new WaitForSeconds(0.1f);
        canTeleport = true;
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<SceneChanger>())
        {
            StartCoroutine(TeleportCooldown());
        }
    }

    #endregion
}
