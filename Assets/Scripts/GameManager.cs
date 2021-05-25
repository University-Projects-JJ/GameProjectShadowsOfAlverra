using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static GameManager instance;

	void Awake() {
		Screen.SetResolution(1280, 720, false);

		if (instance == null)
			instance = this;
	}
	public bool isRunning;
	public GameObject enemyPrefab;
	public GameObject player;

	public GameObject spawnAudio;

	public GameObject audioManager;

	public int activatedRunes = 0;

	List<string[]> runeDialogueSentences = new List<string[]>();

	// Start is called before the first frame update
	void Start() {
		initializeRuneDialogueSentences();
	}

	public void startGame() {
		isRunning = true;
	}

	public void endGame(bool didWin) {
		isRunning = false;
		endGameAudio();
		UIManager.instance.showEndMenu(didWin);

	}

	void endGameAudio() {

		AudioClip ambient = audioManager.GetComponent<AudioSource>().clip;

		toggleAudio(false);

		AudioSource.PlayClipAtPoint(ambient, player.transform.position);
	}

	public void toggleAudio(bool isRunning = true) {


		foreach (Transform audio in audioManager.transform) {
			audio.gameObject.GetComponent<AudioSource>().mute = !isRunning ? true : !audio.gameObject.GetComponent<AudioSource>().mute;
			audio.gameObject.SetActive(!isRunning ? false : !audio.gameObject.activeInHierarchy);
		}

		audioManager.SetActive(!isRunning ? false : !audioManager.activeInHierarchy);
		audioManager.GetComponent<AudioSource>().mute = !isRunning ? true : !audioManager.GetComponent<AudioSource>().mute;

	}

	public IEnumerator spawnEnemies(GameObject room, int maxCount) {
		for (int i = 0; i < maxCount; i++) {
			Transform spawners = room.GetComponent<RuneScript>().spawners;
			Transform parent = room.GetComponent<RuneScript>().enemiesParent;

			spawnAudio.GetComponent<AudioSource>().Play();

			int rand = Random.Range(0, spawners.childCount);
			GameObject shadowCreature = Instantiate(enemyPrefab, spawners.GetChild(rand).position, Quaternion.identity, parent);

			shadowCreature.GetComponent<AIDestinationSetter>().target = player.transform;
		}

		yield return null;
	}




	public string[] getRuneDialogueSentences() {
		return runeDialogueSentences[activatedRunes];
	}

	void initializeRuneDialogueSentences() {

		string[] rune1 = {
			"I have journeyed to the temple, with the last remaining Light. ",
			"I carry the hopes and dreams of all those who have fallen and those few that remain. ",
			"Alverra’s fate rests upon my shoulders. Should I fail, the world will be... ",
			"Nothing but darkness... an irreversible and grim fate..."
		};

		string[] rune2 = {
			"The world of Alverra was not always consumed by darkness." ,
			"Years and years ago, it was a prosperous world, where peoples of all kinds",
			"existed in harmony and peace, guided by the Light. " ,
			"However, as with all good things, the age of Light came to an end. ",
			"A terrible and otherworldly Plague ravaged the lands, ",
			"and almost the entirety of Alverra was plunged into Darkness...",
		};

		string[] rune3 = {
			"We fought back the onslaught of shadows, with all the power we had. ",
			"But the Darkness was too strong... ",
			"and it eventually overtook anything that crossed its path, ",
			"and the death of Alverra was a slow one and unforgiving. ",
			"Many fell in the face of that chaos, ",
			"and terrible vistas of emptiness and darkness became of them...",
		};

		string[] rune4 = {
		"Hope was a thing of the past, and those few of us not yet corrupted by the darkness, ",
		"flocked together at the last remaining point of light in the world, ",
		"accepting our inevitable demise, understanding that nothing could be done. ",
		"The darkness had consumed the vast majority of the world ",
		"and the source of the world’s Light had all but flickered out..."
		};

		string[] rune5 = {
			"But There was still a chance to free the world from the shackles of the Dark Plague. ",
			"The ancient temple of True Light, long forgotten and unused, ",
			"now a center for the Darkness, festering abominations walking its halls. ",
			"I must restore all the runes, everyone is depending on me..."
		};


		string[] rune6 = {
		"The Light spreads across the now Darkened lands of Alverra, ",
		"bringing radiance and hope to all who remained, ",
		"as well as cleansing those who were corrupted. ",
		"Eventually, every bit of Darkness gradually disappears, and once again ",
		"its inhabitants live under the bright guidance of the Light. ",
		"I can rest now..."
		};

		runeDialogueSentences.Add(rune1);
		runeDialogueSentences.Add(rune2);
		runeDialogueSentences.Add(rune3);
		runeDialogueSentences.Add(rune4);
		runeDialogueSentences.Add(rune5);
		runeDialogueSentences.Add(rune6);
	}


}
