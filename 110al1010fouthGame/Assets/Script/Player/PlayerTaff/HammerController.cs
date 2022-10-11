using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerController : MonoBehaviour
{
    // Start is called before the first frame update
    //!Public components
    public int _damage = 1;

    public float _knockBackForce = 200f;

    public BoxCollider2D _bc2D;
    public GameObject _player;

    //!Private components
    SpriteRenderer _playerSprite;
    Vector2 _offetPosition;
    Animator _animator;
    Vector2 _localPosition;
    void Start()
    {
        _bc2D.enabled = false;
        _playerSprite = _player.GetComponent<SpriteRenderer>();
        _offetPosition = transform.localPosition;
        _animator = GetComponent<Animator>();
        _localPosition = transform.localPosition;
    }


    public void LeftAttack()
    {
        transform.localPosition = _localPosition;
    }

    public void RightAttack()
    {
        transform.localPosition = new Vector2(1.53f,0.0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            Vector2 directionKnockback;
            IDamageable damageableObject = other.GetComponent<IDamageable>();
            //se attacco da destra va a destra 
            if((Vector2)transform.localPosition == _localPosition)
                directionKnockback = new Vector2(-1,0);
            else
                directionKnockback = new Vector2(1,0);
            Vector2 knockBack = directionKnockback * _knockBackForce;
            damageableObject.OnHit(_damage,knockBack);
            
        }
        else
        {
        }  
    }

}
