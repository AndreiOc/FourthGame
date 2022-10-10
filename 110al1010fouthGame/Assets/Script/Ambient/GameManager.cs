using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public int _level;
    [SerializeField] int _actualEnemy;
    [SerializeField] ArenaManager _arena;
    public int ActualEnemy
    {
        set
        {
            _actualEnemy = value;
        }
        get
        {
            return _actualEnemy;
        }
    }
    void Start()
    {
        _level = 7;
        _actualEnemy = _level;
    }

    // Update is called once per frame
    void Update()
    {
        if(_actualEnemy == 0)
        {
            _level++;
            ActualEnemy = _level;
            _arena._spawn = true;
        }
    }
}
