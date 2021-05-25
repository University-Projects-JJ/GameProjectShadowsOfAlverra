using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueObjectScript : MonoBehaviour {
	public enum DialogueObjectType { chest, storyRune, rune };
	public DialogueObjectType type;
	public bool canInteract;

	public string[] chestDialogueSentences = {
		 "I might be able to open this.",
	"This looks like a chest. There might be something inside."
	};

	public string[] storyRuneDialogueSentences = {
	};

	public bool showingDialogue = false;

	void Update() {
		if (canInteract)
			interact();

	}



	void OnCollisionEnter2D(Collision2D other) {
		GameObject target = other.gameObject;
		if (target.tag == "Player") {
			if (!showingDialogue) {

				if (type == DialogueObjectType.chest && !GetComponent<ChestScript>().isOpen) {
					showingDialogue = true;
					UIManager.instance.showDialogue(selectSentence(chestDialogueSentences), gameObject);
				}

				canInteract = true;
			}
		}
	}
	void OnCollisionExit2D(Collision2D other) {
		GameObject target = other.gameObject;
		if (target.tag == "Player")
			canInteract = false;
	}

	void interact() {
		if (Input.GetKeyDown(KeyCode.E) && canInteract) {
			canInteract =false;
			if (type == DialogueObjectType.chest && !GetComponent<ChestScript>().isOpen) {
				ChestScript chestScript = GetComponent<ChestScript>();
				chestScript.isOpen = true;

				GetComponent<SpriteRenderer>().sprite = GetComponent<ChestScript>().chestOpen;

				chestScript.openingChest.GetComponent<AudioSource>().Play();

				// Restore some light to player
				GameManager.instance.player.GetComponent<PlayerScript>().refillHealth(PlayerScript.HealthType.powerup);

				UIManager.instance.showDialogue(chestScript.dialogue.sentences);
			}
			if (type == DialogueObjectType.storyRune) {
				UIManager.instance.showDialogue(storyRuneDialogueSentences, gameObject);
			}
		}
	}




	string[] selectSentence(string[] dialogueSentences) {
		int rand = Random.Range(0, dialogueSentences.Length);
		string[] sentences = { dialogueSentences[rand] };

		return sentences;
	}


}
