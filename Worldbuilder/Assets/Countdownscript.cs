using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Countdownscript : MonoBehaviour {
    public GameObject three;
    public GameObject two;
    public GameObject one;
    public GameObject go;
    int timer = 100;
    int showthing = 0;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(RaceManager.currentstate == 0)
        {
            timer--;
            countdown();
        }
	}
    void countdown()
    {
        if (timer <= 0)
        {
            timer = 100;
            showthing++;
        }
        switch (showthing){
            case 0:
                three.SetActive(true);
                two.SetActive(false);
                one.SetActive(false);
                go.SetActive(false);
                break;
            case 1:
                three.SetActive(false);
                two.SetActive(true);
                one.SetActive(false);
                go.SetActive(false);
                break;
            case 2:
                three.SetActive(false);
                two.SetActive(false);
                one.SetActive(true);
                go.SetActive(false);
                break;
            case 3:
                three.SetActive(false);
                two.SetActive(false);
                one.SetActive(false);
                go.SetActive(true);
                break;
            default:
                three.SetActive(false);
                two.SetActive(false);
                one.SetActive(false);
                go.SetActive(false);
                RaceManager.currentstate = RaceManager.gamestate.playing;
                break;
        }
    }
}
