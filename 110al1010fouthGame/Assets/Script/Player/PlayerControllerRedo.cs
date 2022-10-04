using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerRedo : ChecksController
{

    //!Componenti
    public float _speed = 10f;
    public float _jumpForce = 10f;
    public HammerController _myHammer;
    public bool _enter = false;
    public GameObject _door;

    //!For the perfect jump
    private float _maxHoldJump = 0.3f;
    public float _holdJumpTimer = 0.0f;
    public bool _isHolding = false;


    // Update is called once per frame
    void Update()
    {
        if (_canMove)
        {
            _animator.SetFloat("yVelocity", _rb2D.velocity.y);
            //Check if the player ray is touching the ground and jump is enable
            if (Input.GetKeyDown(KeyCode.Space) && Grounded())
            {
                _jumpInput = true;
                _isHolding = true;
                _rb2D.velocity = Vector2.up * _jumpForce;

            }
            if (Input.GetKey(KeyCode.Space) && _jumpInput)
            {
                if (_holdJumpTimer < _maxHoldJump)
                {
                    _rb2D.velocity = Vector2.up * _jumpForce;
                    _holdJumpTimer += Time.deltaTime;
                }
                else
                {
                    _isHolding = false;
                    _jumpInput = false;
                }

            }
            if (Grounded())
            {
                _holdJumpTimer = 0;
                _jumpInput = true;
            }
            _animator.SetBool("isJumping", !Grounded());

            if (Input.GetKey(KeyCode.E) && _enter)
            {
                Debug.Log("Entra ");
                //Todo animazione e tuttoi 
                transform.position = _door.transform.position;

            }
        }
    }

    private void FixedUpdate()
    {
        Cast();
        if (_canMove)
        {
            //Quindi se corro verso uno dei due lati cambio lo sprite
            if (_moveInput != Vector2.zero)
            {
                if (_moveInput.x < 0)
                {
                    _bc2D.offset = new Vector2(-1 * _offesetBC2D.x, _offesetBC2D.y);
                    _spriteRender.flipX = true;
                }
                else if (_moveInput.x > 0)
                {
                    _bc2D.offset = _offesetBC2D;
                    _spriteRender.flipX = false;
                }
                _jumpInput = false;
                if(!Input.GetKey(KeyCode.Space))
                    _animator.SetBool("isRunning", true);
            }
            else
            {
                _animator.SetBool("isRunning", false);
            }
            _rb2D.velocity = new Vector2(_moveInput.x * _speed, _rb2D.velocity.y);

            bool grounded = Grounded();
            //Check for fallFromPlatform input and start falling only if the player is touching the ground
            if (_fallFromPlatformInput && grounded)
            {
                if (_currentPlatform != null)
                {
                    //start falling from the platform 
                    _fallingFromPlatform = true;
                }
            }
            _fallFromPlatformInput = false;
            Debug.Log("Could :" + CloudPlatformCheck());
            //Check if the player is grounded on a platform and the should fall down
            if (CloudPlatformCheck() && _fallingFromPlatform)
            {
                Debug.Log(CloudPlatformCheck());
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

    /// <summary>
    /// Serve a prendere gli inpouit del mio personaggio per farlo muovere
    /// </summary>
    /// <param name="movementValue"></param>
    void OnMove(InputValue movementValue)
    {
        if (_canMove)
            _moveInput = movementValue.Get<Vector2>();
    }

    void OnGoDown()
    {
        Debug.Log("SCENDI");
        _fallFromPlatformInput = true;
    }
    void OnAttack()
    {
        _animator.SetTrigger("isAttacking");
        if (_spriteRender.flipX)
            _myHammer.LeftAttack();
        else
            _myHammer.RightAttack();
    }

    public void LockMovement()
    {
        _canMove = false;
        _myHammer.GetComponent<BoxCollider2D>().enabled = true;
    }
    public void UnLockMovement()
    {
        _canMove = true;
        _myHammer._bc2D.enabled = false;

    }

}
