using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
	public static UIManager instance;
	public GameObject mainMenu, pauseMenu, endMenu, winMenu, howToMenu,
	btnPause, lightBar, dialoguePopup;
	public Text dialogueText;

	public GameObject currentDialogueObject = null;
	public bool canDisplayNext = true;
	public const float fadeDuration = 0.4f;

	public Queue<string> sentencesQueue = new Queue<string>();

	void Awake() {
		if (instance == null)
			instance = this;
		else
			Destroy(gameObject);
	}

	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}




	public void onClickPlayGame() {
		showMainMenu(false);
		GameManager.instance.startGame();
	}

	public void onClickResumeGame() {
		togglePauseMenu(false);
		GameManager.instance.isRunning = true;
	}

	public void onClickPauseGame() {
		GameManager.instance.isRunning = !GameManager.instance.isRunning;
		togglePauseMenu(true);
	}

	public void onClickMainMenu() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		showMainMenu(true);
	}

	public void onClickExit() {
		GameManager.instance.endGame(false);
		Application.Quit();
	}

	public void onClickAudio() {
		GameManager.instance.toggleAudio();
	}

	public void onClickHowTo() {
		toggleHowToMenu();
	}


	public void showMainMenu(bool isVisible = true) {
		float start = isVisible ? 0 : 1;
		StartCoroutine(fadeCanvas(mainMenu, start, Mathf.Abs(1 - start)));
		btnPause.SetActive(!isVisible);
		lightBar.SetActive(!isVisible);
	}

	public void togglePauseMenu(bool isVisible = true) {
		bool isActive = pauseMenu.activeInHierarchy;
		if (!isVisible)
			pauseMenu.SetActive(false);
		else
			pauseMenu.SetActive(!isActive);
	}

	public void toggleHowToMenu() {
		howToMenu.SetActive(!howToMenu.activeInHierarchy);

	}

	public void showEndMenu(bool didWin) {
		GameObject menu = didWin ? winMenu : endMenu;
		StartCoroutine(fadeCanvas(menu, 0, 1, 1.0f));
		menu.SetActive(true);
		btnPause.SetActive(false);
		lightBar.SetActive(false);
	}

	public void showDialogue(string[] sentences, GameObject dialogueObject = null) {
		if (dialogueObject != null)
			currentDialogueObject = dialogueObject;

		// stop game
		GameManager.instance.isRunning = false;

		// fade in dialogue
		dialoguePopup.SetActive(true);

		foreach (string sentence in sentences) {
			sentencesQueue.Enqueue(sentence);
		}

		displayNextSentence();

	}

	void endDialogue() {

		dialogueText.text = "";
		dialoguePopup.SetActive(false);
		GameManager.instance.isRunning = true;

		if (currentDialogueObject != null) {
			DialogueObjectScript dialogueObjectScript = currentDialogueObject.GetComponent<DialogueObjectScript>();
			dialogueObjectScript.showingDialogue = false;
			dialogueObjectScript.canInteract = true;
			currentDialogueObject = null;
		}
	}

	void displayNextSentence() {
		canDisplayNext = false;
		string sentence = sentencesQueue.Dequeue();
		StopAllCoroutines();
		StartCoroutine(TypeSentence(sentence));
		StartCoroutine(checkForClick());

	}


	IEnumerator checkForClick() {
		while (dialoguePopup.activeInHierarchy) {
			if (canDisplayNext && (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.E))) {
				if (sentencesQueue.Count == 0) {
					endDialogue();
				}
				else
					displayNextSentence();
			}
			yield return null;
		}
	}

	IEnumerator TypeSentence(string sentence) {
		dialogueText.text = "";
		char[] characters = sentence.ToCharArray();
		for (int i = 0; i < characters.Length; i++) {
			dialogueText.text += characters[i];
			yield return null;
		}
		canDisplayNext = true;
	}

	public IEnumerator fadeCanvas(GameObject menu, float start, float end, float fadeDuration = fadeDuration) {
		CanvasGroup canvasGroup = menu.GetComponent<CanvasGroup>();
		float counter = 0f;
		while (counter < fadeDuration) {
			counter += Time.deltaTime;
			canvasGroup.alpha = Mathf.Lerp(start, end, counter / fadeDuration);
			yield return null;
		}
		if (start == 1)
			menu.SetActive(false);
	}

}
