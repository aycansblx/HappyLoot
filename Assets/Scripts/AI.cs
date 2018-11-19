using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour {

	private bool _help;

	private float _timer;
	private float _limit;
	private float _distance;

	private Vector3 _destination;

	private Transform _player;
	private Transform _target;

	private Animator _animator;

	void Awake () {
		_limit = Random.Range (3.3f, 7f);
	}

	// Use this for initialization
	void Start () {
		_help = false;
		_timer = _limit;
		_animator = GetComponent<Animator> ();
		GameObject go = GameObject.FindGameObjectWithTag ("Player");
		if (go != null)
			_player = go.transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (_animator == null)
			return;
		float state = _animator.GetInteger ("StateVariable");
		_timer += Time.deltaTime;
		if (_timer >= _limit && state < 2) {
			Decide ();
			_timer = 0f;
		} else if (_timer >= _limit) {
			_animator.SetInteger ("StateVariable", 0);
		}
		if (state == 1) {
			if (_target != null) {
				float currDistance = Vector3.Distance (transform.position, _target.position);
				float diff = Mathf.Abs (transform.eulerAngles.y - _target.eulerAngles.y);
				if (currDistance <= 1f && state < 2) {
					if (diff > 160f && diff < 200f && Random.value < 0.5f) {
						GetComponent<CharacterControl> ().Block ();
						GetComponent<CharacterControl> ().DelayedUnblock (0.7f);
					} else
						GetComponent<CharacterControl> ().Attack ();
				} else if (currDistance > 3f && currDistance < 4.5f && ((diff > 160f && diff < 200f) || (diff < 45f)) && Random.value < 0.33f)
					GetComponent<CharacterControl> ().Flash ();
			}
			if (_player != null) {
				float currDistance = Vector3.Distance (transform.position, _player.position);
				float diff = Mathf.Abs (transform.eulerAngles.y - _player.eulerAngles.y);
				if (currDistance <= 1f && state < 2) {
					if (diff > 160f && diff < 200f && Random.value < 0.5f) {
						GetComponent<CharacterControl> ().Block ();
						GetComponent<CharacterControl> ().DelayedUnblock (0.7f);
					} else
						GetComponent<CharacterControl> ().Attack ();
				} else if (currDistance > 3f && currDistance < 4.5f && ((diff > 160f && diff < 200f) || (diff < 45f)) && Random.value < 0.33f)
					GetComponent<CharacterControl> ().Flash ();
			}
		}
	}

	void Decide () {
		//Debug.Log ("KARAR:");
		_target = null;
		if (_player != null) {
			if (_player.GetComponent<Rigidbody> () != null) {
				_target = _player;
				_distance = Vector3.Distance (_target.position, transform.position);
			} else
				_distance = float.MaxValue;
		} else
			_distance = float.MaxValue;
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("Character")) {
			if (go == this.gameObject)
				continue;
			if (Vector3.Distance (go.transform.position, transform.position) < _distance) {
				_distance = Vector3.Distance (go.transform.position, transform.position);
				_target = go.transform;
			}
		}
		float goldDistance = float.MaxValue;
		Transform goldTarget = null;
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("Gold Coin")) {
			if (Vector3.Distance (go.transform.position, transform.position) < goldDistance) {
				goldDistance = Vector3.Distance (go.transform.position, transform.position);
				goldTarget = go.transform;
			}
		}

		int coinsTarget = 0;
		if (_target != null)
			coinsTarget = _target.GetComponent<CharacterInteraction> ().GetCoins ();

		if (_help == true || (goldTarget == null && _target == null)) {
			int ground = Random.Range (0, transform.GetChild (0).childCount);
			MeshCollider phys = transform.parent.GetChild (0).GetChild (ground).GetComponent<MeshCollider> ();
			float x = Random.Range (phys.bounds.min.x + 0.5f, phys.bounds.max.x - 0.5f);
			float z = Random.Range (phys.bounds.min.z + 0.5f, phys.bounds.max.z - 0.5f);
			_destination = new Vector3 (x, 0.125f, z);
			GetComponent<CharacterMovement> ().MoveTo (_destination);
			//Debug.Log ("Rastgele Gidiyorum");
			_help = false;
		} else if (coinsTarget > 10 && Random.value < 0.7f) {
			_destination = _target.position;
			GetComponent<CharacterMovement> ().MoveTo (_destination);
		} else if (goldTarget != null) {
			if ((goldDistance < _distance * 2f && Random.value < 0.7f) || _target == null) {
				_target = null;
				_destination = goldTarget.transform.position;
				//Debug.Log ("Altina Gidiyorum");
				GetComponent<CharacterMovement> ().MoveTo (_destination);
			} else if (_target != null){
				_destination = _target.position;
				GetComponent<CharacterMovement> ().MoveTo (_destination);
				//Debug.Log ("Sana Geliyorum");
			}
		} else if (_target != null) {
			_destination = _target.position;
			GetComponent<CharacterMovement> ().MoveTo (_destination);
			//Debug.Log ("Sana Geliyorum");
		}
	}

	public void Think (bool param = false) {
		_timer = _limit;
		_help = param;
	}
}
