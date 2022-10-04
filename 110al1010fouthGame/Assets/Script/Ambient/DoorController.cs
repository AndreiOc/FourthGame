using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject _exit; 
    private GameObject _player;
    private Animator _animator;
    public bool changingRoom = false;
    private BoxCollider2D _bc2D;

    //!oggetti attaccati al player
  
    void Start()
    {
        _animator = GetComponent<Animator>();
        _player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log(_player);
        _bc2D = GetComponent<BoxCollider2D>();
    } 

    private void Update()
    {    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {        
            _animator.SetBool("isOpen",true);
            _exit.GetComponent<Animator>().SetBool("isOpen",true);

            _player.GetComponent<PlayerControllerRedo>()._enter = true;
            _player.GetComponent<PlayerControllerRedo>()._door = _exit;
            
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Player" ) 
        {     
            _animator.SetBool("isOpen",false);
            _exit.GetComponent<Animator>().SetBool("isOpen",false);
            
            other.GetComponent<PlayerControllerRedo>()._enter = false;  

        }        
    }

}
