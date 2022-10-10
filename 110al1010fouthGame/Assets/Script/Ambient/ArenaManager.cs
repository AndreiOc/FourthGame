using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ArenaManager : MonoBehaviour
{

    public GameObject [] _enemies;
    public Transform [] _spawnPoints;
    public GameManager _gamamanager;
    public bool _spawn = true;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(_spawn)
        {
            SpawnEnemies();
            _spawn = false;
        }

    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < _gamamanager._level; i++)
        {
            float x = Random.Range(_spawnPoints[0].position.x,_spawnPoints[1].position.x);
            float y = Random.Range(_spawnPoints[2].position.y,_spawnPoints[3].position.y);
            Debug.Log(x + " " + y );
            Instantiate(_enemies[0],new Vector3(x,y,0),Quaternion.identity);
        }
    }
}
