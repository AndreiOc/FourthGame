using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject _exit; 
    private Animator _animator;

    //!oggetti attaccati al player
  
    void Start()
    {
        _animator = GetComponent<Animator>();
    } 


    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {        
            _animator.SetBool("isOpen",true);
            _exit.GetComponent<Animator>().SetBool("isOpen",true);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Player")
        {        
            _animator.SetBool("isOpen",false);
            _exit.GetComponent<Animator>().SetBool("isOpen",false);
        }        
    }


    /// <summary>
    /// Disattivo il boxCollider tempo per uscire e poi rientrare
    /// </summary>
    /// <returns></returns>
    IEnumerator CloseDoor()
    {
        yield return new WaitForSeconds(0.5f);
        _animator.SetBool("isOpen",false);
        _exit.GetComponent<BoxCollider2D>().enabled = enabled;

    }

    IEnumerator Timer(Collider2D other)
    {
        yield return new WaitForSeconds(1f);
        //other.transform.position = _exit.transform.position;

    }

}
