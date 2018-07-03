using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUI : MonoBehaviour {

    public GameManager gameManager;
    public TextMeshProUGUI txtWaveNumber;
    public TextMeshProUGUI txtEnemiesLeft;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        txtEnemiesLeft.text = "Monsters: " + gameManager.GetEnemyCount().ToString();
        txtWaveNumber.text = "Wave: " + gameManager.GetWave();
	}
}
