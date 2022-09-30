using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject _exit; 

    //!oggetti attaccati al player
  
    void Start()
    {} 

    // Update is called once per frame
    void Update()
    {
        
    }


    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            if(Input.GetKey(KeyCode.E))
            {
                _exit.GetComponent<BoxCollider2D>().enabled = false;
                other.transform.position = _exit.transform.position;
                StartCoroutine(CloseDoor());
            }
        }
    }
    /// <summary>
    /// Disattivo il boxCollider tempo per uscire e poi rientrare
    /// </summary>
    /// <returns></returns>
    IEnumerator CloseDoor()
    {
        yield return new WaitForSeconds(0.5f);
        _exit.GetComponent<BoxCollider2D>().enabled = enabled;

    }

}
