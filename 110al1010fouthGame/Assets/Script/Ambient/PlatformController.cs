using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    private PlatformEffector2D _effector;
    
    // Start is called before the first frame update
    void Start()
    {
        _effector = GetComponent<PlatformEffector2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            _effector.rotationalOffset = 0;
        }
        if(Input.GetKey(KeyCode.S))
        {
            _effector.rotationalOffset = 180;
        }
    }
}
