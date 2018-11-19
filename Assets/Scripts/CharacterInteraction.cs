using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInteraction : MonoBehaviour {

	private float _globalCounter;

	private bool _die;
	private float _corpseCounter;

	private int _coins;
	public string _name;

	private GameObject _score;
	private GameObject _leaderboard;

	private GameObject _temp;

	private List<GameObject> _orderedPlayers; 
	private Animator _animator;

	public AudioClip BLC;
	public AudioClip DTH;

	void Awake () {
		_orderedPlayers = new List<GameObject> ();
		_leaderboard = GameObject.Find ("Canvas").transform.GetChild (3).gameObject;
		_animator = GetComponent<Animator> ();
	}

	// Use this for initialization
	void Start () {
		_die = false;
		_corpseCounter = 0f;
		_coins = 0;
		_temp = null;
		_score = GameObject.Find ("Score");
		UpdateLeaderboard ();
	}

	// Update is called once per frame
	void Update () {
		_globalCounter += Time.deltaTime;
		if (_die) {
			_corpseCounter += Time.deltaTime;
			if (_corpseCounter > 1.9f) {
				if (tag == "Finished")
					FindObjectOfType<PlayerSpawner> ().SpawnCharacter (false);
				Destroy (this.gameObject);
			}
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag.Equals ("Gold Coin")) {
			if (_temp == null) {
				_coins++;
				_temp = other.gameObject;
				Destroy (other.gameObject);
				if (tag == "Player")
					_score.GetComponent<Text> ().text = _coins + "";
				if (GameObject.FindGameObjectWithTag("Player") != null)
					GameObject.FindGameObjectWithTag ("Player").GetComponent<CharacterInteraction> ().UpdateLeaderboard ();
				if (_coins % 7 == 0)
					GetComponent<CharacterMovement> ().DecreaseModifier ();
			}
		}
	}

	void OnCollisionEnter (Collision collision) {
		int flag = 0;
		bool defending = false;
		if ((collision.gameObject.tag.Equals ("Player") || collision.gameObject.tag.Equals ("Character")) && !collision.gameObject.GetComponent<CharacterInteraction> ().Dying ()) {
			if (_animator != null) {
				if (_animator.GetInteger ("StateVariable") == 10) {
					if (_die)
						return;
					Bounds sword = transform.GetChild (5).GetChild (0).GetChild (3).GetComponent<BoxCollider> ().bounds;
					Bounds shield = collision.gameObject.transform.GetChild (4).GetChild (0).GetComponent<CapsuleCollider> ().bounds;
					if (collision.gameObject.GetComponent<Animator> ().GetInteger ("StateVariable") == 20)
						defending = true;
					for (int i = 0; i < collision.contacts.Length; i++) {
						if (defending) {
							if (sword.Intersects (shield)) {
								flag = 1;
								GetComponent<AudioSource> ().clip = BLC;
								GetComponent<AudioSource> ().Play ();
							}
						}
						if (sword.Contains (collision.contacts [i].point) && flag == 0)
							flag = 2;
					}
					if (flag == 2)
						collision.gameObject.GetComponent<CharacterInteraction> ().Die ();
					if (flag == 1 && collision.gameObject.tag == "Character") {
						collision.gameObject.GetComponent<AI> ().Think ();
					}
				}
			}
		}

		if ((collision.gameObject.tag.Equals ("Wall") || collision.gameObject.tag.Equals ("Finish")) && flag==0) {
			//if (GetComponent<CharacterMovement>() != null)
			//	GetComponent<CharacterMovement> ().Stop ();
			//if (GetComponent<Animator>() != null)
			//	GetComponent<Animator> ().SetInteger ("StateVariable", 0);
			if (tag == "Character") {
				GetComponent<AI> ().Think (true);
			}
		}
	}

	public int GetCoins () {
		return _coins;
	}

	public void SetName(string name) {
		_name = name;
	}

	public string GetName() {
		return _name;
	}

	public string GetText () {
		return _name + " / " + _coins + " coins";
	}

	public void UpdateLeaderboard() {
		_orderedPlayers.Clear ();
		if (tag == "Player")
			_orderedPlayers.Add (this.gameObject);
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("Character")) {
			int index = 0;
			for (; index < _orderedPlayers.Count; index++)
				if (_orderedPlayers [index].GetComponent<CharacterInteraction> ().GetCoins () < go.GetComponent<CharacterInteraction> ().GetCoins ())
					break;
			_orderedPlayers.Insert (index, go);
		}
		string str = "";
		for (int i=0 ;i<_orderedPlayers.Count; i++)
			str += (i+1) + " - " + _orderedPlayers[i].GetComponent<CharacterInteraction>().GetText() + "\n";
		_leaderboard.GetComponent<Text> ().text = str;
	}

	void Die () {
		GetComponent<AudioSource> ().clip = DTH;
		GetComponent<AudioSource> ().Play ();

		gameObject.layer = 12;

		if (tag == "Player") {
			Transform canvas = GameObject.Find ("Canvas").transform;
			canvas.GetChild (0).gameObject.SetActive (false);
			canvas.GetChild (1).gameObject.SetActive (false);
			canvas.GetChild (2).gameObject.SetActive (false);
			canvas.GetChild (3).gameObject.SetActive (false);
			canvas.GetChild (5).gameObject.SetActive (false);
			canvas.GetChild (6).gameObject.SetActive (true);
			canvas.GetChild (6).GetChild (0).GetComponent<Text> ().text = _coins + " coins are collected."; 
			canvas.GetChild (6).GetChild (1).GetComponent<Text> ().text = "You survived for " + (int)(_globalCounter*10f)/10f + " seconds.";
			FindObjectOfType<PlayerSpawner> ().Death ();
		}
		_die = true;
		Destroy (GetComponent<Rigidbody> ());
		if (tag == "Character")
			tag = "Finish";
		else
			tag = "Slaughtered";
		transform.GetChild (4).GetChild (0).SetParent (transform);
		transform.GetChild (5).GetChild (0).SetParent (transform);
		if (tag == "Finish") {
			if (GameObject.FindGameObjectWithTag("Player") != null)
				GameObject.FindGameObjectWithTag ("Player").GetComponent<CharacterInteraction> ().UpdateLeaderboard ();
			else if (GameObject.FindGameObjectWithTag ("Slaughtered") != null)
				GameObject.FindGameObjectWithTag ("Slaughtered").GetComponent<CharacterInteraction> ().UpdateLeaderboard ();
		} else
			GameObject.FindGameObjectWithTag ("Slaughtered").GetComponent<CharacterInteraction> ().UpdateLeaderboard ();
		for (int i = 0; i < transform.childCount; i++) {
			transform.GetChild (i).gameObject.AddComponent<Rigidbody> ();
			transform.GetChild(i).tag = "Finish";
		}
		Destroy (GetComponent<Animator> ());
		FindObjectOfType<CoinSpawner> ().DeathSpawn (transform.position, _coins);
	}

	public bool Dying () {
		return _die;
	}
}
