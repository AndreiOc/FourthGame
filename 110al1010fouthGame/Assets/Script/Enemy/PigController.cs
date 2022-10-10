using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigController : ChecksController, IDamageable
{

    [SerializeField] private int health = 5;    
    [SerializeField] private GameManager _gamemanager;
    public int Health
    {
        set
        {
            health = value;
            if(health<=0)
            {
                _animator.SetTrigger("isDying");
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
    {}

    private void FixedUpdate()
    {
        
        _animator.SetFloat("yVelocity", _rb2D.velocity.y);
        if(!Grounded())
            _animator.SetBool("isJumping",true);
        else
            _animator.SetBool("isJumping",false);
        
        //Get the first object hit by the ray
        RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(transform.position.x - 0.4f,transform.position.y), Vector2.left, laserLength,layerMask);
        RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(transform.position.x + 0.4f,transform.position.y), Vector2.right, laserLength,layerMask);
   
        //If the collider of the object hit is not NUll
        if(hitLeft.collider != null )
        {

            if(hitLeft.collider.CompareTag("Player"))
            {
                _spriteRender.flipX = false;
                _bc2D.offset = _offesetBC2D;
                _directionPlayer = (hitLeft.collider.transform.position - transform.position).normalized; 
                _animator.SetBool("isRunning",true);
                _rb2D.velocity = new Vector2(_directionPlayer.x * _speed, _rb2D.velocity.y);
            }
        }
        if(hitRight.collider != null)
        {
            if(hitRight.collider.CompareTag("Player"))
            {
                _spriteRender.flipX = true;
                _bc2D.offset = new Vector2(_offesetBC2D.x * -1, _offesetBC2D.y);
                _directionPlayer = (hitRight.collider.transform.position - transform.position).normalized; 
                _animator.SetBool("isRunning",true);
                _rb2D.velocity = new Vector2(_directionPlayer.x * _speed, _rb2D.velocity.y);                
            }
        }   
        if(hitRight.collider == null && hitLeft.collider == null)
            _animator.SetBool("isRunning",false);


        //Method to draw the ray in scene for debug purpose
        Debug.DrawRay(new Vector2(transform.position.x - 0.4f,transform.position.y), Vector2.left * laserLength, Color.red);
        Debug.DrawRay(new Vector2(transform.position.x + 0.4f,transform.position.y), Vector2.right * laserLength, Color.blue);

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
        Destroy(gameObject);
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

