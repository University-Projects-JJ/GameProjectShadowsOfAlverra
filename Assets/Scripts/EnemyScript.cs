using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class EnemyScript : MonoBehaviour {
	public AIPath aIPath;
	Animator animator;

	int MAX_HEALTH = 3;
	int health;
	float fadeAmount;
	float lastAttack;

	// Start is called before the first frame update
	void Start() {
		animator = GetComponent<Animator>();
		health = MAX_HEALTH;

		fadeAmount = Mathf.Ceil((float)1.0f / MAX_HEALTH);
	}

	// Update is called once per frame
	void Update() {
		setMovement();
	}

	void setMovement() {

		bool isRunning = GameManager.instance.isRunning;
		GetComponent<AIPath>().canMove = isRunning;

		animator.SetFloat("Horizontal", isRunning ? aIPath.desiredVelocity.x : 0);
		animator.SetFloat("Vertical", isRunning ? aIPath.desiredVelocity.y : 0);
		animator.SetFloat("Speed", isRunning ? aIPath.desiredVelocity.sqrMagnitude : 0);

	}

	void OnCollisionStay2D(Collision2D other) {
		GameObject target = other.gameObject;

		if (target.tag == "Player" && Time.time - lastAttack > 1f) {
			lastAttack = Time.time;
			GameManager.instance.player.GetComponent<PlayerScript>().takeDamage();
		}
	}

	public void takeDamage() {
		if (--health <= 0) {
			StartCoroutine(FadeAway());
		}

	}

	private IEnumerator FadeAway() {
		Color newColor = new Color(255, 255, 255, 0);

		for (float i = 1; i >= 0; i -= Time.deltaTime) {
			newColor.a = i;

			GetComponent<SpriteRenderer>().color = newColor;
		}

		yield return new WaitForSeconds(0.4f);

		Destroy(gameObject);
	}
}
