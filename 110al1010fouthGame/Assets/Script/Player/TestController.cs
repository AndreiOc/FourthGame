using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class TestController : MonoBehaviour
{

//==VARIABILI DELLO SCRIPT
    public float speed = 3f;
    public int kills = 0;
    public ContactFilter2D movementFilter;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    public float collisionOffset = 0.005f;
    private Vector2 movementInput = Vector2.zero;
    private bool success;
    public bool canMove = true;
    /// <summary>
    /// per correggere il box collider quando si shifta il personaggio nel caso si avesse dei problemi di grandezza
    /// </summary>
    private Vector2 boxColliderShift;

//==COMPONENTI DELL'OGGETTO
    private Rigidbody2D rb2D;
    private BoxCollider2D bc2D;
    private SpriteRenderer spriteRenderer;
    private Animator animator;




    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        bc2D = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        boxColliderShift = bc2D.offset;

        Debug.Log(boxColliderShift);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (canMove)
        {
            if (movementInput != Vector2.zero)
            {
                bool success = TryToMove(movementInput);
                if (!success && movementInput.x > 0)
                {
                    success = TryToMove(new Vector2(movementInput.x, 0));
                    if (!success && movementInput.y > 0)
                    {
                        success = TryToMove(new Vector2(0, movementInput.y));
                    }
                }
                animator.SetBool("isMoving", success);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }

            if (movementInput.x < 0)
            {
                bc2D.offset = new Vector2(-1 * boxColliderShift.x,boxColliderShift.y);
                spriteRenderer.flipX = true;
                Debug.Log(bc2D.offset);
            }
            else if (movementInput.x > 0)
            {
                bc2D.offset = boxColliderShift;
                spriteRenderer.flipX = false;
                Debug.Log(bc2D.offset);

            }
        }
    }

    private bool TryToMove(Vector2 direction)
    {
        int count = rb2D.Cast(direction, movementFilter, castCollisions, speed * Time.fixedDeltaTime + collisionOffset);
        foreach (var item in castCollisions)
        {
            if(item.collider.gameObject.CompareTag("Enemy"))
                count = 0 ;
        }

        if(count == 0)
        {
            rb2D.MovePosition(rb2D.position + direction * speed * Time.fixedDeltaTime);
            return true;
        }
        else
        {
            return false;
        }

    }   
    
    /// <summary>
    /// Funzione diretta dall'input system new 
    /// </summary>
    /// <param name="movementValue"></param>
    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }

    /// <summary>
    /// Bottone del mouse
    /// </summary>
    void OnFire()
    {   
        animator.SetTrigger("isAttacking");
    }


}