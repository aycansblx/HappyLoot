using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour {

	public GameObject Coin;

	private float _timer;

	private bool _allowed;

	private int counter;

	// Use this for initialization
	void Start () {
		counter = 0;
		_timer = 0f;
		_allowed = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (_allowed) {
			_timer += Time.deltaTime;
			if (_timer > 0.7f && counter < 70) {
				_timer = 0f;
				GameObject coin = Instantiate (Coin);
				int ground = Random.Range (0, transform.GetChild (0).childCount);
				MeshCollider phys = transform.GetChild (0).GetChild (ground).GetComponent<MeshCollider> ();
				float x = Random.Range (phys.bounds.min.x + 0.5f, phys.bounds.max.x - 0.5f);
				float z = Random.Range (phys.bounds.min.z + 0.5f, phys.bounds.max.z - 0.5f);
				coin.transform.position = new Vector3 (x, 0.125f, z);
				coin.transform.SetParent (transform.GetChild (3));
				counter++;
			}
		}
	}

	public void SetAllowed(bool param) {
		_allowed = param;
	}

	public void DeathSpawn (Vector3 position, int coins) {
		for (int i = 0; i < coins; i++) {
			GameObject coin = Instantiate (Coin);
			float x = Random.Range (position.x - 1f, position.x + 1f);
			float z = Random.Range (position.z - 1f, position.z + 1f);
			coin.transform.position = new Vector3 (x, 0.125f, z);
			coin.transform.SetParent (transform.GetChild (3));
		}	
	}
}
