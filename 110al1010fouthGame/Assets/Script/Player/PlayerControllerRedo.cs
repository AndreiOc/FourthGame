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
    private bool _isHoldingJump = false;
    private float _maxHoldJump = 0.4f;
    private float _holdJumpTimer = 0.0f;
    public float _jumpGroundThreshold = 1;


    // Update is called once per frame
    void Update()
    {
        if(_canMove)
        {
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
            if(_moveInput!=Vector2.zero)
            {
                if (_moveInput.x < 0)
                {
                    _bc2D.offset = new Vector2(-1 * _offesetBC2D.x,_offesetBC2D.y);
                    _spriteRender.flipX = true;
                }
                else if (_moveInput.x > 0)
                {
                    _bc2D.offset = _offesetBC2D;
                    _spriteRender.flipX = false;
                }
                _animator.SetBool("isRunning",true);
            }    
            else
            {
                _animator.SetBool("isRunning",false);
            }
            _rb2D.velocity = new Vector2(_moveInput.x * _speed, _rb2D.velocity.y);
            bool grounded = Grounded();
            //Check if the player ray is touching the ground and jump is enable
            if (_jumpInput && grounded)
            {
                _rb2D.velocity = new Vector2(_rb2D.velocity.x, _jumpForce);
                _animator.SetBool("isJumping",true);
            }
            _animator.SetBool("isJumping",!Grounded());
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
        
        } 
    }
   
    /// <summary>
    /// Serve a prendere gli inpouit del mio personaggio per farlo muovere
    /// </summary>
    /// <param name="movementValue"></param>
    void OnMove(InputValue movementValue)
    {
        if(_canMove)
            _moveInput = movementValue.Get<Vector2>();
    }  

    void OnAttack()
    {
        _animator.SetTrigger("isAttacking");
        if(_spriteRender.flipX)
            _myHammer.LeftAttack();
        else
            _myHammer.RightAttack();
    }

    public void LockMovement()
    {
        _canMove = false;
        _myHammer._bc2D.enabled = true;
    }
    public void UnLockMovement()
    {
        _canMove = true;
        _myHammer._bc2D.enabled = false;

    }

}
