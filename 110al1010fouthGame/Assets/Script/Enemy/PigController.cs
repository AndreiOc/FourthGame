using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigController : ChecksController, IDamageable
{

    [SerializeField] private int health = 5;    
    [SerializeField] private GameManager _gamemanager;
    bool _isDead = false;
    public int Health
    {
        set
        {
            health = value;
            if(health<=0)
            {
                _animator.SetTrigger("isDying");
                _rb2D.bodyType = RigidbodyType2D.Static;
                _bc2D.isTrigger = true;
            }
        }
        get
        {
            return health;
        }
    }


    //!Component pubblic
    public int _damage = 1;
    public float _knockBackForce = 500f;
    public float _speed = 10f;
    public float laserLength = 4f;


    //!Componenti privati cambiali
    private Vector2 _directionPlayer;
    public Vector2 _startPosition =  new Vector2(10.8f,0.7f);
    private bool _Collide = false;
    
    int layerMask;
    // Start is called before the first frame update
    void Start()
    {
        _gamemanager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _rb2D = GetComponent<Rigidbody2D>();
        _spriteRender = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _bc2D = GetComponent<BoxCollider2D>();
       
        _offesetBC2D = _bc2D.offset;         
        layerMask = LayerMask.GetMask("Player");

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        if (!_isDead)
        {
            _animator.SetFloat("yVelocity", _rb2D.velocity.y);
            if(_rb2D.velocity.y!=0)
                _animator.SetBool("isJumping",true);
            else
                _animator.SetBool("isJumping",false);


            //Get the first object hit by the ray
            RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(transform.position.x - 0.4f, transform.position.y), Vector2.left, laserLength, layerMask);
            RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(transform.position.x + 0.4f, transform.position.y), Vector2.right, laserLength, layerMask);

            //If the collider of the object hit is not NUll
            if (hitLeft.collider != null)
            {
                if (hitLeft.collider.CompareTag("Player"))
                {
                    _spriteRender.flipX = false;
                    _bc2D.offset = _offesetBC2D;
                    _directionPlayer = (hitLeft.collider.transform.position - transform.position).normalized;
                    _animator.SetBool("isRunning", true);
                    _rb2D.velocity = new Vector2(_directionPlayer.x * _speed, _rb2D.velocity.y);
                }
            }
            if (hitRight.collider != null)
            {
                if (hitRight.collider.CompareTag("Player"))
                {
                    _spriteRender.flipX = true;
                    _bc2D.offset = new Vector2(_offesetBC2D.x * -1, _offesetBC2D.y);
                    _directionPlayer = (hitRight.collider.transform.position - transform.position).normalized;
                    _animator.SetBool("isRunning", true);
                    _rb2D.velocity = new Vector2(_directionPlayer.x * _speed, _rb2D.velocity.y);
                }
            }
            if (hitRight.collider == null && hitLeft.collider == null)
                _animator.SetBool("isRunning", false);
            //Method to draw the ray in scene for debug purpose
            Debug.DrawRay(new Vector2(transform.position.x - 0.4f, transform.position.y), Vector2.left * laserLength, Color.red);
            Debug.DrawRay(new Vector2(transform.position.x + 0.4f, transform.position.y), Vector2.right * laserLength, Color.blue);
        }
    }

    private void StayOnPlatform()
    {
        float laserLength = 0.0125f;
        //Left ray start X
        float left = transform.position.x - (_bc2D.size.x * transform.localScale.x / 2.0f) + (_bc2D.offset.x * transform.localScale.x) + 0.1f;
        //Right ray start X
        float right = transform.position.x + (_bc2D.size.x * transform.localScale.x / 2.0f) + (_bc2D.offset.x * transform.localScale.x) - 0.1f;
        //Hit only the objects of Platform layer
        int layerMask = LayerMask.GetMask("Platform");
        //Left ray start point
        Vector2 startPositionLeft = new Vector2(left, transform.position.y - (_bc2D.bounds.extents.y + 0.15f));
        //Check if the left laser hit something
        RaycastHit2D hitLeft = Physics2D.Raycast(startPositionLeft, Vector2.down, laserLength, layerMask);
        //Right ray start point
        Vector2 startPositionRight = new Vector2(right, transform.position.y - (_bc2D.bounds.extents.y + 0.15f));
        //Check if the right laser hit something
        RaycastHit2D hitRight = Physics2D.Raycast(startPositionRight, Vector2.down, laserLength, layerMask);
        //The color of the ray for debug purpose
        Collider2D col2DHit = null;

        if (hitLeft.collider != null || hitRight.collider != null)
        {
            col2DHit = hitLeft.collider != null ? hitLeft.collider : hitRight.collider;
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

    }
    public void OnHit(int damage, Vector2 knockback)
    {
        Health -= damage;  
        _rb2D.AddForce(knockback);
        _animator.SetTrigger("isHitting");
        Debug.Log(health);
    }
    public void OnHit(int damage)
    {
        Health -= damage;  
        Debug.Log("Da hit senza knock " + health);

    }
    public void DestroyEnemy()
    {
        //Destroy(gameObject);
        _isDead = true;
        _gamemanager.ActualEnemy -= 1;
    }
    private void OnCollisionStay2D(Collision2D other)
    {
        if(other.collider.CompareTag("Player"))
        {
            Vector2 directionKnockback;

            _animator.SetBool("isAttacking",true);
            if(!_spriteRender.flipX)
                directionKnockback = new Vector2(-1,0);
            else
                directionKnockback = new Vector2(1,0);
            Vector2 knockBack = directionKnockback * _knockBackForce;
            other.collider.GetComponent<PlayerControllerRedo>().OnHit(_damage,knockBack);
        }       
        if(other.collider.CompareTag("Enemy"))
        {
            Physics2D.IgnoreCollision(other.collider,GetComponent<Collider2D>(),true);
            
        }    
    }
    private void OnCollisionExit2D(Collision2D other) {
        if(other.collider.CompareTag("Player"))
        {
            _animator.SetBool("isAttacking",false);
        }          
    }
}

