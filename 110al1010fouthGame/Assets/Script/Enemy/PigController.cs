using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigController : MonoBehaviour, IDamageable
{

    private float health = 3;    
    public float Health
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

    public float _speed = 10f;
    public float laserLength = 4f;

    //!Componenti attaccati all'oggetto
    private Rigidbody2D _rb2D;
    private SpriteRenderer _spriteRender;
    private BoxCollider2D _bc2D;
    private CircleCollider2D _cc2D;
    private Animator _animator;    

    //!Componenti privati cambiali
    private Vector2 _offesetBC2D;
    private Vector2 _directionPlayer;
    public Vector2 _startPosition =  new Vector2(10.8f,0.7f);
    private bool _canMove = true;
    private bool _Collide = false;
    
    int layerMask;
    // Start is called before the first frame update
    void Start()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _spriteRender = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _bc2D = GetComponent<BoxCollider2D>();
       
        _offesetBC2D = _bc2D.offset;         
        Debug.Log(transform.position);
        layerMask = LayerMask.GetMask("Player");
    }

    // Update is called once per frame
    void Update()
    {}

    private void FixedUpdate()
    {
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
    public void OnHit(float damage, Vector2 knockback)
    {
        Health -= damage;
        _rb2D.AddForce(knockback);
    }
    public void OnHit(float damage)
    {
        Health -= damage;
    }

}

