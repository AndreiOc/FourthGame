using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    // Start is called before the first frame update
    private BoxCollider2D _bc2D;
    private GameObject _uimanager;
    private Animator _animator;
    void Start()
    {
        _bc2D = GetComponent<BoxCollider2D>();
        _uimanager = GameObject.Find("UIManager");
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("PlayerObject"))
        {
            _uimanager.GetComponent<UIManager>().UpdateDiamondsScore();
            _animator.SetTrigger("isConsumed");
        }    
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
