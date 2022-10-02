using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IDamageable
{

    //!Public attributes
    public float _speed = 10f;
    public float _jumpForce = 10f;
    public GameObject _myHammer;
    public bool _enter = false;    
    public GameObject _door;

    //!Components
    private Rigidbody2D _rb2D;
    private SpriteRenderer _spriteRender;
    private BoxCollider2D _bc2D;
    private Animator _animator;

    //!Private attributes
    private bool _jumpInput = false;
    private bool _fallFromPlatformInput = false;
    private bool _fallingFromPlatform = false;
    private Collider2D _currentPlatform;
    private ContactFilter2D _filter2D;
    private Vector2 _offesetBC2D;
    //Allocate an array with just one element capacity to store the floor when hit
    RaycastHit2D[] hits = new RaycastHit2D[1];
    private bool _canMove = true;
    private float _moveHorizontal;
    private Vector2 _moveInput;
    public float Health { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    // Start is called before the first frame update
    void Start()
    {
        //Get components
        _rb2D = GetComponent<Rigidbody2D>();
        _spriteRender = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _bc2D = GetComponent<BoxCollider2D>();
        _offesetBC2D = _bc2D.offset;
        //Create a contactFilter configuration for the rays to check if the player is grounded
        _filter2D = new ContactFilter2D
        {
            //Ignore trigger colliders
            useTriggers = false,
            //Use a layer mask
            useLayerMask = true
        };
        //Set the layer mask to hit only Floor and Platform layer
        _filter2D.SetLayerMask(LayerMask.GetMask("Floor", "Platform"));
        
    }
    void OnAttack()
    {
        _animator.SetTrigger("isAttacking");
    }


    private void Update()
    {
        if (_canMove)
        {
            _moveHorizontal = Input.GetAxis("Horizontal");
            _animator.SetFloat("yVelocity", _rb2D.velocity.y);
            
            //Keep down arrow pressed and press space
            if (Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.Space))
            {
                //Enable falling down from a platform
                _fallFromPlatformInput = true;
            }
            //Press only Space instead
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                //Let the player jump
                _jumpInput = true;
            }
            if(Input.GetKey(KeyCode.E) && _enter)
            {
                EnterExitDoor();
            }
        }
        Debug.Log(_bc2D.offset);
    }
    void OnMove(InputValue movementValue)
    {
        _moveInput = movementValue.Get<Vector2>();
    }    
    
    /// <summary>
    /// funzione per triggerare l'animazione e poi il 
    /// </summary>
    
    void EnterExitDoor()
    {
        _animator.SetTrigger("isOpening");
    }
    /// <summary>
    /// Funzione che lancio quando viene attivato l'animazione
    /// </summary>
    public void ChangeRoom()
    {   
        transform.position = _door.transform.position;
    }
    void FixedUpdate()
    {
        if (_canMove)
        {
            //Store the current horizontal input in the float_moveHorizontal.

            //Flip the sprite according to the direction
            if (_moveHorizontal > 0)
            {
                _animator.SetBool("isRunning", true);
                _spriteRender.flipX = false;
                _bc2D.offset = _offesetBC2D;

            }
            else if (_moveHorizontal < 0)
            {
                _animator.SetBool("isRunning", true);
                _spriteRender.flipX = true;
                _bc2D.offset = new Vector2(-1 * _offesetBC2D.x, _offesetBC2D.y );

            }
            else if(_moveHorizontal == 0)
            {
                _animator.SetBool("isRunning", false);
                if(_spriteRender.flipX)
                    _bc2D.offset = new Vector2(-1 * _offesetBC2D.x, _offesetBC2D.y );
                else
                    _bc2D.offset = _offesetBC2D;
            }

            //Move the player through its body
            _rb2D.velocity = new Vector2(_moveHorizontal * _speed, _rb2D.velocity.y);
            bool grounded = Grounded();
            //Check if the player ray is touching the ground and jump is enable
            if (_jumpInput && grounded)
            {
                _rb2D.velocity = new Vector2(_rb2D.velocity.x, _jumpForce);
                _animator.SetBool("isJumping", true);
            }

            _animator.SetBool("isJumping", !Grounded());
            _jumpInput = false;

            //Check for fallFromPlatform input and start falling only if the player is touching the ground
            if (_fallFromPlatformInput && grounded)
            {
                if (_currentPlatform != null)
                {
                    //start falling from the platform 
                    _fallingFromPlatform = true;
                }
            }

            //Reset the fall input
            _fallFromPlatformInput = false;
            //Check if the player is grounded on a platform and the should fall down
            if (CloudPlatformCheck() && _fallingFromPlatform)
            {
                //Cast the ray above the player head to check 
                FallingFromPlatformCheck();
                if (_currentPlatform != null && !_currentPlatform.isTrigger)
                {
                    //Reset the cloud platform to initial state (as trigger)
                    _currentPlatform.isTrigger = true;
                    SpriteRenderer sprite = _currentPlatform.gameObject.GetComponent<SpriteRenderer>();
                    sprite.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                    _currentPlatform = null;
                }
                //If the platform has become a trigger now the updated Grounded will return false because the playes is falling
                //When it returns true again it means the player is touching the floor so disable the fallingFromPlatform check
                if (Grounded())
                {
                    //disable the fallingFromPlatform check
                    _fallingFromPlatform = false;
                }
            }
            else
            {
                //Disable the fallingFromPlatform check
                _fallingFromPlatform = false;
            }
        }
    }

    void FixedUpdate2()
    {
        //Laser length
        float laserLength = 0.0250f;
        //Right ray start X
        float startPositionX = transform.position.x + (_bc2D.size.x * transform.localScale.x / 2.0f) + (_bc2D.offset.x * transform.localScale.x) - 0.1f;
        //Hit only the objects of Platform layer
        int layerMask = LayerMask.GetMask("Bonus");
        //Left ray start point
        Vector2 startPosition = new Vector2(startPositionX, transform.position.y - (_bc2D.bounds.extents.y + 0.05f));
        //The color of the ray for debug purpose
        Color rayColor = Color.red;
        //Check if the left laser hits something
        int totalObjectsHit = Physics2D.Raycast(startPosition, Vector2.down, _filter2D, hits, laserLength);

        //Iterate the objects hit by the laser
        for (int i = 0; i < totalObjectsHit; i++)
        {
            //Get the object hit
            RaycastHit2D hit = hits[i];
            //Do something
            if (hit.collider != null)
            {
                SpriteRenderer sprite = hit.collider.GetComponent<SpriteRenderer>();
                sprite.color = Color.green;

            }
            
        }
    }



    bool Grounded()
    {
        //Laser length
        float laserLength =0.0250f;
        //Left ray start X
        float left = transform.position.x - (_bc2D.size.x * transform.localScale.x / 2.0f) + (_bc2D.offset.x * transform.localScale.x) + 0.1f;
        //Right ray start X
        float right = transform.position.x + (_bc2D.size.x * transform.localScale.x / 2.0f) + (_bc2D.offset.x * transform.localScale.x) - 0.1f;
        //Hit only the objects of Platform layer
        int layerMask = LayerMask.GetMask("Floor", "Platform");
        //Left ray start point
        Vector2 startPositionLeft = new Vector2(left, transform.position.y - (_bc2D.bounds.extents.y + 0.05f));
        //Right ray start point
        Vector2 startPositionRight = new Vector2(right, transform.position.y - (_bc2D.bounds.extents.y + 0.05f));
        //The color of the ray for debug purpose
        Color rayColor = Color.red;
        //Check if the left laser hits something
        int leftCount = Physics2D.Raycast(startPositionLeft, Vector2.down, _filter2D, hits, laserLength);
        //Check if the right laser hits something
        int rightCount = Physics2D.Raycast(startPositionRight, Vector2.down, _filter2D, hits, laserLength);

        Collider2D col2DHit = null;
        //If one of the lasers hits the floor
        //if ((leftCount > 0 && hitsLeft[0].collider != null) || (rightCount > 0 && hitsRight[0].collider != null))
        if ((leftCount > 0 || rightCount > 0) && hits[0].collider != null)
        {

            //Get the object hits collider
            col2DHit = hits[0].collider;
            //Change the color of the ray for debug purpose
            rayColor = Color.green;
        }
        //Draw the ray for debug purpose
        Debug.DrawRay(startPositionLeft, Vector2.down * laserLength, rayColor);
        Debug.DrawRay(startPositionRight, Vector2.down * laserLength, rayColor);
        //If the ray hits the floor returns true, false otherwise
        return col2DHit != null;
    }

    bool CloudPlatformCheck()
    {
        //While the player is checking from falling from a platform invalidate this check
        if (_fallingFromPlatform) return true;
        //Laser length
        float laserLength = 0.0250f;
        //Left ray start X
        float left = transform.position.x - (_bc2D.size.x * transform.localScale.x / 2.0f) + (_bc2D.offset.x * transform.localScale.x) + 0.1f;
        //Right ray start X
        float right = transform.position.x + (_bc2D.size.x * transform.localScale.x / 2.0f) + (_bc2D.offset.x * transform.localScale.x) - 0.1f;
        //Hit only the objects of Platform layer
        int layerMask = LayerMask.GetMask("Platform");
        //Left ray start point
        Vector2 startPositionLeft = new Vector2(left, transform.position.y - (_bc2D.bounds.extents.y + 0.05f));
        //Check if the left laser hit something
        RaycastHit2D hitLeft = Physics2D.Raycast(startPositionLeft, Vector2.down, laserLength, layerMask);
        //Right ray start point
        Vector2 startPositionRight = new Vector2(right, transform.position.y - (_bc2D.bounds.extents.y + 0.05f));
        //Check if the right laser hit something
        RaycastHit2D hitRight = Physics2D.Raycast(startPositionRight, Vector2.down, laserLength, layerMask);
        //The color of the ray for debug purpose
        Color rayColor = Color.red;

        Collider2D col2DHit = null;
        //If one of the lasers hit a cloud platform
        if (hitLeft.collider != null || hitRight.collider != null)
        {
            //Get the object hit collider
            col2DHit = hitLeft.collider != null ? hitLeft.collider : hitRight.collider;
            //Change the color of the ray for debug purpose
            rayColor = Color.green;
            //If the cloud platform collider is trigger
            if (col2DHit.isTrigger)
            {
                //Store the platform to reset later
                _currentPlatform = col2DHit;
                //Disable trigger behaviour of collider
                _currentPlatform.isTrigger = false;
                //Color the sprite of the cloud platform for debug purpose
                SpriteRenderer sprite = _currentPlatform.gameObject.GetComponent<SpriteRenderer>();
                sprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
        }
        else
        {
            //Change the color of the ray for debug purpose
            rayColor = Color.red;
            //If we stored previously a platform
            if (_currentPlatform != null)
            {
                //Reset the platform properties
                _currentPlatform.isTrigger = true;
                SpriteRenderer sprite = _currentPlatform.gameObject.GetComponent<SpriteRenderer>();
                sprite.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                _currentPlatform = null;
            }
        }

        //Draw the ray for debug purpose
        Debug.DrawRay(startPositionLeft, Vector2.down * laserLength, rayColor);
        Debug.DrawRay(startPositionRight, Vector2.down * laserLength, rayColor);
        //If the ray hits a platform returns true, false otherwise
        return col2DHit != null;
    }

    bool FallingFromPlatformCheck()
    {
        //Laser length
        float laserLength = 0.0250f;
        //Ray start point
        Vector2 startPosition = new Vector2(transform.position.x, transform.position.y - (_bc2D.bounds.extents.y));
        //Hit only the objects of Platform layer
        int layerMask = LayerMask.GetMask("Platform");
        //Check if the laser hit something
        RaycastHit2D hit = Physics2D.Raycast(startPosition, Vector2.down, laserLength, layerMask);
        //The color of the ray for debug purpose
        Color rayColor = Color.red;
        if (hit.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
            //the player overcame the platform falling down, so disable the fallingFromPlatform check
            _fallingFromPlatform = false;
        }

        //Draw the ray for debug purpose
        Debug.DrawRay(startPosition, Vector2.down * laserLength, rayColor);
        return hit.collider != null;
    }

    public void LockMovement()
    {
        StartCoroutine(BlockMovements());
    }
    IEnumerator BlockMovements()
    {
        _canMove = false;
        _myHammer.GetComponent<HammerController>().ActiveHammer();
        yield return new WaitForSeconds(0.5f);
    }
    public void UnLockMovement()
    {
        _canMove = true;
        _myHammer.GetComponent<HammerController>().DisactiveHammer();

    }


    public void OpeningTheDoor()
    {
        _animator.SetTrigger("isOpening");
    }
    public void ClosingTheDoor()
    {
        _animator.SetTrigger("isClosing");
    }   
    public void OnHit(float damage,Vector2 knockback)
    {
        _rb2D.AddForce(knockback);
    }


    public void OnHit(float damage)
    {
        Health -= damage;
    }


}