using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestScript : MonoBehaviour {
	public bool isOpen = false;
	public Sprite chestOpen, chestClosed;

	public Dialogue dialogue;
	public GameObject openingChest;
	// Start is called before the first frame update
	void Start() {
		GetComponent<SpriteRenderer>().sprite = chestClosed;
	}

	// Update is called once per frame
	void Update() {

	}


}
