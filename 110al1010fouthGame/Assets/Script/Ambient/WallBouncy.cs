using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBouncy : MonoBehaviour
{
    private Rigidbody2D _rb2D;
    public float _knockBackForce = 100f;
    public GameObject _player;
    private SpriteRenderer _playerSprite;

    private void Start()
    {
        _playerSprite = _player.GetComponent<SpriteRenderer>();
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("cc");
        IDamageable damageableObject = other.collider.GetComponent<IDamageable>();

        if(other.collider.CompareTag("Enemy"))
        {
            Debug.Log("Collision");
           //Vettori utili al knockback
            Vector2 directionKnockback;
            if(_playerSprite.flipX)
                directionKnockback = new Vector2(1,0);
            else
                directionKnockback = new Vector2(-1,0);

            //se attacco da destra va a destra 
            Vector2 knockBack = directionKnockback * _knockBackForce;
            damageableObject.OnHit(0,knockBack);
               
        }
    }
}
