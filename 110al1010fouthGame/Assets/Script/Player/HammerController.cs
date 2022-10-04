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
    Vector2 _localPosition;
    void Start()
    {
        _bc2D.enabled = false;
        _playerSprite = _player.GetComponent<SpriteRenderer>();
        _offetPosition = transform.localPosition;
        _animator = GetComponent<Animator>();
        _localPosition = transform.localPosition;
    }

    // Update is called once per frame

    public void LeftAttack()
    {
        Debug.Log("Attacco a sinistra");
        transform.localPosition = _localPosition;
    }

    public void RightAttack()
    {
        Debug.Log("Attacco a destra");
        transform.localPosition = new Vector2(1.53f,0.0f);
    }

}
