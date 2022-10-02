using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerController : MonoBehaviour
{
    // Start is called before the first frame update
    public BoxCollider2D _bc2D;
    public GameObject _player;

    //!Private components
    SpriteRenderer _playerSprite;
    Vector2 _offetPosition;
    Animator _animator;
    void Start()
    {
        _bc2D.enabled = false;
        _playerSprite = _player.GetComponent<SpriteRenderer>();
        _offetPosition = transform.localPosition;
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ActiveHammer()
    {
        _bc2D.enabled = true;

    }
    public void DisactiveHammer()
    {
        _bc2D.enabled = false;
        
    }

    public void OffesetBoxColliderHammer(bool off)
    {
        if(off)
            _animator.SetTrigger("isLeft");
        else
            _animator.SetTrigger("isRight");


    }


}
