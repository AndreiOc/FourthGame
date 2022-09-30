using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerController : MonoBehaviour
{
    // Start is called before the first frame update
    public Collider2D _bc2D;
    void Start()
    {
        _bc2D.enabled = false;
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


}
