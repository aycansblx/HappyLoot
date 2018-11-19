using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterControl : MonoBehaviour {

	private bool _flash;
	private bool _block;

	private float _flashSpeed;

	private float _tempTimer;

	private float _attackCoolDown;
	private float _flashCoolDown;

	public AudioClip ATT;
	public AudioClip FLS;

	private GameObject _hud;

	// Use this for initialization
	void Start () {
		_flash = false;
		_block = false;
		_flashSpeed = 19791f;
		_hud = GameObject.Find ("HUD");
	}
	
	// Update is called once per frame
	void Update () {
		if (GetComponent<Animator> () == null)
			return;
		int state = GetComponent<Animator> ().GetInteger ("StateVariable");
		if (Input.GetMouseButtonDown (0) && state < 2 && tag == "Player")
			Attack ();
		if (Input.GetKeyDown (KeyCode.X) && state < 3 && tag == "Player")
			Flash ();
		if (Input.GetKeyDown (KeyCode.Z) && state < 3 && _block == false && tag == "Player")
			Block ();
		if (Input.GetKeyUp (KeyCode.Z) && _block == true && tag == "Player")
			UnBlock ();
		if (_flash && GetComponent<Rigidbody> ().velocity == Vector3.zero) {
			_flash = false;
			GetComponent<Animator> ().SetInteger ("StateVariable", 0);
			if (tag == "Character")
				GetComponent<AI> ().Think ();
			_flashCoolDown = 7f;
		}
		if (_tempTimer >= 0) {
			_tempTimer -= Time.deltaTime;
			if (_tempTimer < 0f)
				UnBlock ();
			if (tag == "Character")
				GetComponent<AI> ().Think ();
		}
		if (tag == "Player") {
			if (_attackCoolDown > 0f) {
				_attackCoolDown -= Time.deltaTime;
				_hud.transform.GetChild (0).GetComponent<Text> ().text = "Attack (RMB): " + ((int)(_attackCoolDown * 10)) / 10f;
			} else if (_attackCoolDown < 0f) {
				_attackCoolDown = 0f;
				_hud.transform.GetChild (0).GetComponent<Text> ().text = "Attack (RMB): ready";
			}
			if (_flashCoolDown > 0f) {
				_flashCoolDown -= Time.deltaTime;
				_hud.transform.GetChild (1).GetComponent<Text> ().text = "Flash (X): " + ((int)(_flashCoolDown * 10)) / 10f;
			} else if (_flashCoolDown < 0f) {
				_flashCoolDown = 0f;
				_hud.transform.GetChild (1).GetComponent<Text> ().text = "Flash (X): ready";
			}
		}
	}

	public void Block () {
		_block = true;
		GetComponent<Animator> ().SetInteger ("StateVariable", 4);
		GetComponent<CharacterMovement> ().Stop ();
		if (tag == "Player")
			_hud.transform.GetChild (2).GetComponent<Text> ().text = "Block (Z): blocking";
	}

	public void UnBlock () {
		_block = false;
		GetComponent<Animator> ().SetInteger ("StateVariable", 0);
		if (tag == "Player")
			_hud.transform.GetChild (2).GetComponent<Text> ().text = "Block (Z): ready";
	}

	public void Attack () {
		if (_attackCoolDown > 0f)
			return;
		GetComponent<Animator> ().SetInteger ("StateVariable", 2);
		GetComponent<CharacterMovement> ().Stop ();
		GetComponent<AudioSource> ().clip = ATT;
		GetComponent<AudioSource> ().Play ();
	}

	public void Flash () {
		if (_flashCoolDown > 0f)
			return;
		_flash = true;
		float angle = -transform.eulerAngles.y * Mathf.Deg2Rad;
		float modifier = GetComponent<CharacterMovement> ().GetModifier ();
		GetComponent<Animator> ().SetInteger ("StateVariable", 3);
		GetComponent<CharacterMovement> ().Stop ();
		GetComponent<Rigidbody> ().AddForce (new Vector3 (Mathf.Cos (angle) * _flashSpeed * modifier, 0f, Mathf.Sin (angle) * _flashSpeed * modifier));
		GetComponent<AudioSource> ().clip = FLS;
		GetComponent<AudioSource> ().Play ();
	}

	void Reset () {
		GetComponent<Animator> ().SetInteger ("StateVariable", 0);
		if (tag == "Character")
			GetComponent<AI> ().Think ();
		_attackCoolDown = 0.7f;
	}

	void Test () {
		GetComponent<Animator> ().SetInteger ("StateVariable", 10);
	}

	void What () {
		GetComponent<Animator> ().SetInteger ("StateVariable", 20);
	}

	public void DelayedUnblock(float duration = 0.33f) {
		_tempTimer = duration;
	}
}
