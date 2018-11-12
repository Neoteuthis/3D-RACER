using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class sliderscript : MonoBehaviour {
   public Slider mySlider;
   public GameObject player;
	// Use this for initialization
	void Start () {
		mySlider = GetComponent<Slider>();
    //    player = GameObject.FindWithTag("player");
	}

    // Update is called once per frame
    void Update() {
        mySlider.value = playerscript.playeracceleration; 
    }
}
