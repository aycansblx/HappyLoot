using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpawner : MonoBehaviour {

	public GameObject Character;

	private bool _menu;
	private bool _death;

	// Use this for initialization
	void Start () {
		_menu = true;
		_death = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (_menu == true && Input.GetKeyDown (KeyCode.Space)) {
			_menu = false;
			ChangeUI ();
			SpawnCharacter (true);
			SpawnCharacter (false);
			SpawnCharacter (false);
			SpawnCharacter (false);
			FindObjectOfType<CoinSpawner> ().SetAllowed (true);
		} else if (_menu == false) {
			int nop = transform.childCount - 4;
			if (nop < 4) {
				for (int i = nop; i < 4; i++)
					SpawnCharacter (false);
			}
		}
		if (_death && Input.GetKeyDown (KeyCode.Space)) {
			_death = false;
			ChangeUI ();
			SpawnCharacter (true);
		}
	}

	void ChangeUI() {
		Transform canvas = GameObject.Find ("Canvas").transform;
		canvas.GetChild (0).gameObject.SetActive (true);
		canvas.GetChild (1).gameObject.SetActive (true);
		canvas.GetChild (2).gameObject.SetActive (true);
		canvas.GetChild (3).gameObject.SetActive (true);
		canvas.GetChild (4).gameObject.SetActive (false);
		canvas.GetChild (5).gameObject.SetActive (true);
		canvas.GetChild (6).gameObject.SetActive (false);
		canvas.GetChild(0).GetComponent<Text>().text = "0";
	}

	public void SpawnCharacter(bool player) {
		GameObject character = Instantiate (Character);
		Vector3 position = Vector3.zero;
		int trial = 0;
		do {
			int ground = Random.Range (0, transform.GetChild (0).childCount);
			MeshCollider phys = transform.GetChild (0).GetChild (ground).GetComponent<MeshCollider> ();
			position.x = Random.Range (phys.bounds.min.x + 0.5f, phys.bounds.max.x - 0.5f);
			position.z = Random.Range (phys.bounds.min.z + 0.5f, phys.bounds.max.z - 0.5f);
			GameObject pl = GameObject.FindGameObjectWithTag ("Player");
			if (Vector3.Distance (pl.transform.position, position) < 2f)
				position = Vector3.zero;
			foreach (GameObject go in GameObject.FindGameObjectsWithTag("Character"))
				if (Vector3.Distance (go.transform.position, position) < 2f)
					position = Vector3.zero;
		} while (position == Vector3.zero && trial++ < 33);
		character.transform.position = position;
		character.transform.SetParent (transform);
		if (player) {
			character.tag = "Player";
			Camera.main.GetComponent<CameraBehaviour> ().AssignPlayer (character.transform);
		} else {
			character.tag = "Character";
			character.AddComponent<AI> ();
		}
	}

	public void Death () {
		_death = true;
	}
}
