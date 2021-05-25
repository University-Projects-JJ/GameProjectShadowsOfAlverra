using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour {
	public float moveSpeed = 0.5f;
	Rigidbody2D rb;
	Animator animator;
	Vector2 movement;
	float lastAttack;

	public float attackRange;
	public LayerMask enemyLayer;

	float lightHealth = 1.0f;
	public float healthPowerup = 0.25f;
	public float healthFill = 0.3f;
	public enum HealthType { refill, powerup };
	public Image imgLightFG;
	public Light surroundLight;

	public GameObject orb;

	public GameObject sliding, slash, enemyHit;

	public bool isSafe;

	// Start is called before the first frame update
	void Start() {
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		movement = new Vector2(0, 0);

		StartCoroutine(decrementLight());
	}

	// Update is called once per frame
	void Update() {
		setPlayerMovement();
		setPlayerAttack();
	}

	void FixedUpdate() {
		rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
	}

	// void OnTriggerEnter2D(Collider2D other) {
	// 	GameObject target = other.gameObject;

	// 	if (target.tag == "Room") {
	// 		isSafe = target.GetComponent<RuneScript>().isActive;
	// 	}

	// }
	void OnTriggerStay2D(Collider2D other) {
		GameObject target = other.gameObject;

		if (target.tag == "Room") {
			isSafe = target.GetComponentInParent<RuneScript>().isActive;

			surroundLight.enabled = !isSafe || target.GetComponentInParent<RuneScript>().battleIsRunning;

		}

	}

	void OnTriggerExit2D(Collider2D other) {
		GameObject target = other.gameObject;

		if (target.tag == "Room") {
			isSafe = false;
			surroundLight.enabled = true;
		}
	}

	void OnCollisionEnter2D(Collision2D other) {
		rb.velocity = new Vector2(0, 0);
		GameObject target = other.gameObject;

	}

	void OnCollisionStay2D(Collision2D other) {
		rb.velocity = new Vector2(0, 0);
		GameObject target = other.gameObject;

		if (target.tag == "Box" && target.GetComponent<Rigidbody2D>().bodyType != RigidbodyType2D.Static) {
			target.GetComponent<Rigidbody2D>().velocity = new Vector2(movement.x * 2, movement.y * 2);
			sliding.GetComponent<AudioSource>().mute = false;
		}
	}

	void OnCollisionExit2D(Collision2D other) {
		GameObject target = other.gameObject;

		if (target.tag == "Box" && target.GetComponent<Rigidbody2D>().bodyType != RigidbodyType2D.Static) {
			target.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
		}
		if (target.tag == "Box") {
			sliding.GetComponent<AudioSource>().mute = true;
		}
	}


	void setPlayerMovement() {
		// if game is running
		movement.x = movement.y = 0;

		if (GameManager.instance.isRunning && Input.GetButton("Horizontal")) {
			movement.x = Input.GetAxisRaw("Horizontal");
			movement.y = 0;
		}

		if (GameManager.instance.isRunning && Input.GetButton("Vertical")) {
			movement.x = 0;
			movement.y = Input.GetAxisRaw("Vertical");
		}

		animator.SetFloat("Horizontal", movement.x);
		animator.SetFloat("Vertical", movement.y);
		animator.SetFloat("Speed", movement.sqrMagnitude);
	}

	void setPlayerAttack() {

		if (GameManager.instance.isRunning && !EventSystem.current.IsPointerOverGameObject()) {
			if (Input.GetButtonDown("Fire1") && (Time.time - lastAttack) > 0.45f) {
				lastAttack = Time.time;


				animator.SetTrigger("isAttacking");
				Vector3 positionMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

				// make the Z position equal to the player for a fully 2D comparison
				positionMouse.z = transform.position.z;

				Vector3 towardsMouseFromPlayer = positionMouse - transform.position;

				Vector3 vectorAttack = towardsMouseFromPlayer.normalized;

				animator.SetFloat("AttackHorizontal", vectorAttack.x);
				animator.SetFloat("AttackVertical", vectorAttack.y);

				slash.GetComponent<AudioSource>().Play();

				attack();
			}
		}
	}

	public void takeDamage() {
		lightHealth -= 0.1f;
	}

	void attack() {
		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange);


		foreach (Collider2D col in colliders) {
			GameObject enemy = col.gameObject;
			if (enemy.tag == "Enemy") {
				enemy.GetComponent<EnemyScript>().takeDamage();

				enemyHit.GetComponent<AudioSource>().Play();

				Vector3 moveDirectionPush = transform.position - enemy.transform.position;
				enemy.GetComponent<Rigidbody2D>().AddForce(-moveDirectionPush.normalized * 40000f);
			}
		}
	}

	void OnDrawGizmos() {
		Gizmos.DrawWireSphere(transform.position, attackRange);
	}

	void updateLightUI() {
		imgLightFG.fillAmount = lightHealth;
	}

	IEnumerator decrementLight() {

		while (lightHealth > 0) {
			updateLightUI();
			if (!isSafe && GameManager.instance.isRunning)
				lightHealth -= 0.002f;
			surroundLight.intensity = lightHealth * 10;
			yield return new WaitForSeconds(0.1f);
		}

		if (lightHealth <= 0) {
			GameManager.instance.endGame(false);
		}

		yield break;

	}

	public void refillHealth(HealthType healthType) {
		float addHealth = healthType == HealthType.refill ? healthFill : healthPowerup;
		float newHealth = lightHealth + addHealth;
		lightHealth = newHealth > 1 ? 1 : newHealth;
	}


}
