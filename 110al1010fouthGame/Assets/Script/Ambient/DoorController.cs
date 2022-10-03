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
        _bc2D = GetComponent<BoxCollider2D>();
    } 

    private void Update()
    {

    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {        
            _animator.SetBool("isOpen",true);
            _exit.GetComponent<Animator>().SetBool("isOpen",true);

            _player.GetComponent<PlayerController>()._enter = true;
            _player.GetComponent<PlayerController>()._door = _exit;
            
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Player" ) 
        {     
            other.GetComponent<PlayerController>()._enter = false;  
            StartCoroutine(Timer());   
        }        
    }
    IEnumerator Timer()
    {
        yield return new WaitForSeconds(10f);
        Debug.Log("Rimani aperta per 2 secondi e poi chiude");
        _animator.SetBool("isOpen",false);
        _exit.GetComponent<Animator>().SetBool("isOpen",false);
    }

}
