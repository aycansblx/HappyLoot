using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour {

	private Transform _player;

	public AudioClip [] PlayList;

	private AudioSource _source;
	private int index;

	// Use this for initialization
	void Start () {
		GameObject go = GameObject.FindGameObjectWithTag ("Player");
		if (go != null)
			_player = go.transform;
		_source = GetComponent<AudioSource> ();
		index = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (_player != null) {
			Vector3 target = _player.position + new Vector3 (-1.1f, 4.5f, -2.2f);
			transform.position = target;
		}
		if (!_source.isPlaying) {
			_source.clip = PlayList [index];
			_source.Play ();
			index = (index + 1) % PlayList.Length;
		}
	}

	public void AssignPlayer (Transform param) {
		_player = param;
	}
}
