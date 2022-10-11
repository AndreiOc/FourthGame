using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public TextMeshProUGUI _diamondsScore;
    private int _diamondsScoreInt = 0;

    // Start is called before the first frame update
    void Start()
    {
        _diamondsScore.text = _diamondsScoreInt.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateDiamondsScore()
    {
        _diamondsScoreInt +=1;
        _diamondsScore.text = _diamondsScoreInt.ToString();
    }
}
