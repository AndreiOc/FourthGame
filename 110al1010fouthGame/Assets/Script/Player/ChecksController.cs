using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChecksController : MonoBehaviour
{

    //!Componenti attaccati all'oggetto
    protected Rigidbody2D _rb2D;
    protected SpriteRenderer _spriteRender;
    protected BoxCollider2D _bc2D;
    protected Animator _animator;

    //!Attributi per andare a definire salto e movenze sulle piattaforme 
    protected bool _jumpInput = false;
    [SerializeField] protected bool _fallFromPlatformInput = false;
    [SerializeField] protected bool _fallingFromPlatform = false;
    [SerializeField] protected Collider2D _currentPlatform;
    protected ContactFilter2D _filter2D; 
    protected RaycastHit2D[] _hits = new RaycastHit2D[1];
    
    //!Componenti privati cambiali
    protected Vector2 _moveInput;
    protected Vector2 _offesetBC2D;
    protected bool _canMove = true;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _spriteRender = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _bc2D = GetComponent<BoxCollider2D>();
        _offesetBC2D = _bc2D.offset;        
 
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


    protected bool Grounded()
    {
        //Laser length
        float laserLength =0.0125f;
        //Left ray start X
        float left = transform.position.x - (_bc2D.size.x * transform.localScale.x / 2.0f) + (_bc2D.offset.x * transform.localScale.x) + 0.1f;
        //Right ray start X
        float right = transform.position.x + (_bc2D.size.x * transform.localScale.x / 2.0f) + (_bc2D.offset.x * transform.localScale.x) - 0.1f;
        //Hit only the objects of Platform layer
        int layerMask = LayerMask.GetMask("Floor", "Platform");
        //Left ray start point
        Vector2 startPositionLeft = new Vector2(left, transform.position.y - (_bc2D.bounds.extents.y + 0.15f));
        //Right ray start point
        Vector2 startPositionRight = new Vector2(right, transform.position.y - (_bc2D.bounds.extents.y + 0.15f));
        //The color of the ray for debug purpose
        Color rayColor = Color.red;
        //Check if the left laser _hits something
        int leftCount = Physics2D.Raycast(startPositionLeft, Vector2.down, _filter2D, _hits, laserLength);
        //Check if the right laser _hits something
        int rightCount = Physics2D.Raycast(startPositionRight, Vector2.down, _filter2D, _hits, laserLength);

        Collider2D col2DHit = null;
        //If one of the lasers _hits the floor
        //if ((leftCount > 0 && hitsLeft[0].collider != null) || (rightCount > 0 && hitsRight[0].collider != null))
        if ((leftCount > 0 || rightCount > 0) && _hits[0].collider != null)
        {

            //Get the object _hits collider
            col2DHit = _hits[0].collider;
            //Change the color of the ray for debug purpose
            rayColor = Color.green;
        }
        //Draw the ray for debug purpose
        Debug.DrawRay(startPositionLeft, Vector2.down * laserLength, rayColor);
        Debug.DrawRay(startPositionRight, Vector2.down * laserLength, rayColor);
        
        //If the ray _hits the floor returns true, false otherwise
        return col2DHit != null;
    }

    protected bool CloudPlatformCheck()
    {
        //While the player is checking from falling from a platform invalidate this check
        if (_fallingFromPlatform) return true;
        //Laser length
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
        //If the ray _hits a platform returns true, false otherwise
        return col2DHit != null;
    }

    //Tecnicamente dovrei far disattivare il 
    protected bool FallingFromPlatformCheck()
    {
        //Laser length
        float laserLength = 0.25f;
        //Ray start point
        Vector2 startPosition = new Vector2(transform.position.x, transform.position.y - (_bc2D.bounds.extents.y));
        //Hit only the objects of Platform layer
        int layerMask = LayerMask.GetMask("Platform");
        //Check if the laser hit something
        RaycastHit2D hit = Physics2D.Raycast(startPosition, Vector2.down, laserLength, layerMask);
        //The color of the ray for debug purpose
        Color rayColor = Color.yellow;
        if (hit.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.blue;
            //the player overcame the platform falling down, so disable the fallingFromPlatform check
            _fallingFromPlatform = false;
        }

        //Draw the ray for debug purpose
        Debug.DrawRay(startPosition, Vector2.down * laserLength, rayColor);
        return hit.collider != null;
    }

    protected void Cast()
    {
        if(_canMove)
        {
        //Laser length
            float laserLength = 0.5f;
            float startPositionX;
            int layerMask = LayerMask.GetMask("Floor");

            //Right ray start X
            if(!_spriteRender.flipX)
                startPositionX = (transform.position.x + 0.1f) + (_bc2D.size.x * transform.localScale.x / 2.0f) + (_bc2D.offset.x * transform.localScale.x) ;
            else
                startPositionX = (transform.position.x - 0.7f) + (_bc2D.size.x * transform.localScale.x / 2.0f) + (_bc2D.offset.x * transform.localScale.x) ;
            //Hit only the objects of Platform layer
            //Left ray start point
            Vector2 startPosition = new Vector2(startPositionX, transform.position.y - (_bc2D.bounds.extents.y + 0.05f));
            //The color of the ray for debug purpose
            Color rayColor = Color.yellow;
            RaycastHit2D hit2D;
            if(!_spriteRender.flipX)
            //Check if the left laser _hits something
                hit2D = Physics2D.Raycast(startPosition, Vector2.right, laserLength,layerMask);
            else
                hit2D = Physics2D.Raycast(startPosition, Vector2.left, laserLength,layerMask);
                //Do something
            if (hit2D.collider != null)
            {
            }

            if(!_spriteRender.flipX)
                Debug.DrawRay(startPosition, Vector2.right * laserLength, Color.red);
            else
                Debug.DrawRay(startPosition, Vector2.left * laserLength, Color.red);

        }  
    } 
}
