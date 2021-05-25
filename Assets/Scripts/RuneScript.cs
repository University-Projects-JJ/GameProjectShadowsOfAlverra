using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RuneScript : MonoBehaviour {
	public Transform symbols, lights, spawners, enemiesParent;
	public float[] maxLightIntensities, intensitiesPerEnemy;
	public int spawnCount;
	public int lastCount;

	public bool battleIsRunning = false;
	public bool isActive = false;
	public bool isFinalRune = false;

	public float lightFadeDuration = 1.0f;

	public GameObject orb, box, activateLight;

	public GameObject roomBounds;
	// Start is called before the first frame update
	void Start() {
		lastCount = spawnCount;
		setRoomBoundsActive(false);

		maxLightIntensities = new float[lights.childCount];
		intensitiesPerEnemy = new float[lights.childCount];
		for (int i = 0; i < lights.childCount; i++) {
			Light light = lights.GetChild(i).GetComponent<Light>();
			maxLightIntensities[i] = light.intensity;
			intensitiesPerEnemy[i] = light.intensity * 1.0f / spawnCount;
			light.intensity = 0;
		}
	}

	// Update is called once per frame
	void Update() {
		if (battleIsRunning) {
			isActive = true;
			updateLights();
			if (enemiesParent.childCount == 0) {
				endBattle();
			}
		}
	}

	public void startBattle() {

		if (isFinalRune && GameManager.instance.activatedRunes < 5) {
			String[] sentences = { "I need to light up the other runes first." };
			UIManager.instance.showDialogue(sentences);
			return;
		}

		battleIsRunning = true;
		// block room
		setRoomBoundsActive(true);

		// spawn enemies
		StartCoroutine(GameManager.instance.spawnEnemies(gameObject, spawnCount));

		// lock box and fade in light
		StartCoroutine(Fade(false));
		box.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

		GameManager.instance.player.GetComponent<PlayerScript>().orb.SetActive(false);
	}

	public void endBattle() {
		battleIsRunning = false;
		activateLight.GetComponent<AudioSource>().Play();
		setRoomBoundsActive(false);
		orb.SetActive(false);
		GameManager.instance.player.GetComponent<PlayerScript>().orb.SetActive(true);
		GameManager.instance.player.GetComponent<PlayerScript>().refillHealth(PlayerScript.HealthType.refill);

		UIManager.instance.showDialogue(GameManager.instance.getRuneDialogueSentences(), gameObject);

		GameManager.instance.activatedRunes++;

		if (isFinalRune) {
			StartCoroutine(checkIfGameDialogueHasEndedAndEndGame());
		}
	}

	IEnumerator checkIfGameDialogueHasEndedAndEndGame() {
		while (!GameManager.instance.isRunning)
			yield return null;
		GameManager.instance.endGame(true);
	}

	void setRoomBoundsActive(bool isActive) {
		roomBounds.GetComponent<TilemapCollider2D>().enabled = isActive;
		roomBounds.GetComponent<CompositeCollider2D>().enabled = isActive;
		roomBounds.SetActive(isActive);
	}

	void updateLights() {


		for (int i = 0; i < lights.childCount; i++) {
			Light light = lights.GetChild(i).GetComponent<Light>();

			float intensity = light.intensity;

			if (isFinalRune)
				intensity = maxLightIntensities[i];
			else if (enemiesParent.childCount == spawnCount)
				intensity = 0;
			else if (enemiesParent.childCount == 0)
				intensity = maxLightIntensities[i];
			else if (lastCount > enemiesParent.childCount) {
				lastCount = enemiesParent.childCount;
				intensity += intensitiesPerEnemy[i];
			}

			light.intensity = intensity;
		}

	}

	IEnumerator fadeInLight(Light light, float start, float end) {
		float counter = 0f;
		while (counter < lightFadeDuration) {
			counter += Time.deltaTime;
			light.intensity = Mathf.Lerp(start, end, counter / lightFadeDuration);
			yield return null;
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		GameObject target = other.gameObject;

		if (target.tag == "Box") {
			box = target;
			startBattle();
		}

	}


	// void OnTriggerExit2D(Collider2D other) {
	// 	GameObject target = other.gameObject;

	// 	if (target.tag == "Box") {
	// 		StartCoroutine(Fade(true));
	// 	}
	// }

	private IEnumerator Fade(bool fadeAway) {
		Color newColor = new Color(255, 255, 255, 0);

		if (fadeAway) {
			for (float i = 1; i >= 0; i -= Time.deltaTime) {
				newColor.a = i;

				orb.GetComponent<SpriteRenderer>().color = newColor;

				foreach (Transform symbol in symbols)
					symbol.gameObject.GetComponent<SpriteRenderer>().color = newColor;

				yield return null;
			}
		}
		else {
			for (float i = 0; i <= 1; i += Time.deltaTime) {
				newColor.a = i;

				orb.GetComponent<SpriteRenderer>().color = newColor;

				foreach (Transform symbol in symbols)
					symbol.gameObject.GetComponent<SpriteRenderer>().color = newColor;

				yield return null;
			}
		}

	}
}
