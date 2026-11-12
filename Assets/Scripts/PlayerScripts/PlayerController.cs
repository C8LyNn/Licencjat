using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform movePoint;
    public LayerMask stopsMovement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movePoint.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        if(Vector3.Distance(transform.position, movePoint.position) == 0f)
        {
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                if(!Physics2D.OverlapCircle(movePoint.position + Vector3.right, .2f, stopsMovement))
                {
                    movePoint.position += Vector3.right;
                }
                
            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                if (!Physics2D.OverlapCircle(movePoint.position + Vector3.left, .2f, stopsMovement))
                {
                    movePoint.position += Vector3.left;
                }
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                if (!Physics2D.OverlapCircle(movePoint.position + Vector3.up, .2f, stopsMovement))
                {
                    movePoint.position += Vector3.up;
                }
            }

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                if (!Physics2D.OverlapCircle(movePoint.position + Vector3.down, .2f, stopsMovement))
                {
                    movePoint.position += Vector3.down;
                }
            }
        }

    }
}
