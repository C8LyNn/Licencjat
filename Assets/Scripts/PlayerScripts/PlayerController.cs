using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    #region Movement Variables

    public float moveSpeed = 5f;
    public Transform movePoint;
    public LayerMask stopsMovement;

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

    private void PlayerMovement()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, movePoint.position) == 0f)
        {
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                playerInteractor.offset = new Vector2(0.5f, 0f);
                if (!Physics2D.OverlapCircle(movePoint.position + Vector3.right, .2f, stopsMovement))
                {
                    movePoint.position += Vector3.right;
                }

            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                playerInteractor.offset = new Vector2(-0.5f, 0f);
                if (!Physics2D.OverlapCircle(movePoint.position + Vector3.left, .2f, stopsMovement))
                {
                    movePoint.position += Vector3.left;
                }
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                playerInteractor.offset = new Vector2(0f, 0.5f);
                if (!Physics2D.OverlapCircle(movePoint.position + Vector3.up, .2f, stopsMovement))
                {
                    movePoint.position += Vector3.up;
                }
            }

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                playerInteractor.offset = new Vector2(0f, -0.5f);
                if (!Physics2D.OverlapCircle(movePoint.position + Vector3.down, .2f, stopsMovement))
                {
                    movePoint.position += Vector3.down;
                }
            }
        }
    }

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
