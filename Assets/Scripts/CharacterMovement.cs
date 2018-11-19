using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour {

	private bool _moving;
	private bool _rotating;

	private float _speed;
	private float _agility;

	private float _prevDistance;

	private float _angle;
	private float _direction;
	private Vector3 _dest;

	private float _modifier;

	// Use this for initialization
	void Start () {
		_moving = false;
		_rotating = false;
		if (tag == "Player") {
			_speed = 777f;
			_agility = 720f;
		} else {
			_speed = 500f;
			_agility = 540f;
		}
		_modifier = 1f;
	}
	
	// Update is called once per frame
	void Update () {
		if (GetComponent<Animator> () == null)
			return;
		int state = GetComponent<Animator> ().GetInteger ("StateVariable");
		if (Input.GetMouseButtonDown (1) && state < 2 && tag == "Player") {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit))
				if (hit.transform.tag.Equals ("Ground"))
					MoveTo (hit.point);
		}
		if (_moving) {
			float dist = Vector3.Distance (transform.position, _dest);
			if (dist > _prevDistance && dist < 0.1f) {
				transform.position = _dest;
				_moving = false;
				GetComponent<Animator> ().SetInteger ("StateVariable", 0);
				GetComponent<Rigidbody> ().velocity = Vector3.zero;
				if (tag == "Character")
					GetComponent<AI> ().Think ();
			} else {
				GetComponent<Rigidbody> ().AddForce (new Vector3 (Mathf.Cos (_angle) * _speed * _modifier, 0f, Mathf.Sin (_angle) * _speed * _modifier));
				_prevDistance = dist;
			}
		} else if (_rotating) {
			Vector3 euler = transform.eulerAngles;
			if (Mathf.Abs (_direction - euler.y) > 180f && euler.y > 0f)
				euler.y = euler.y - 360f;
			else if (Mathf.Abs (_direction - euler.y) > 180f && euler.y < 0f)
				euler.y = 360f + euler.y;
			if (Mathf.Abs (_direction - euler.y) < _agility * Time.deltaTime * _modifier) {
				_rotating = false;
				_moving = true;
				GetComponent<Animator> ().SetInteger ("StateVariable", 1);
				euler.y = _direction;
			} else if (_direction - euler.y < 0f)
				euler.y -= _agility * Time.deltaTime * _modifier;
			else
				euler.y += _agility * Time.deltaTime * _modifier;
			transform.eulerAngles = euler;
		}
	}

	public void MoveTo (Vector3 position) {
		_dest = position;
		_angle = Mathf.Atan2 (_dest.z - transform.position.z, _dest.x - transform.position.x);
		if (_moving) {
			_moving = false;
		}
		_rotating = true;
		_direction = -_angle * Mathf.Rad2Deg;
		if (_direction > 180f)
			_direction = _direction - 360f;
		if (_direction < -180f)
			_direction = 360f + _direction;
		_prevDistance = float.MaxValue;
	}

	public void Stop () {
		_moving = false;
		_rotating = false;
		if (GetComponent<Rigidbody>() != null)
			GetComponent<Rigidbody> ().velocity = Vector3.zero;
	}

	public void DecreaseModifier () {
		if (_modifier * 0.9 < 0.4f)
			return;
		_modifier *= 0.9f;
	}

	public float GetModifier () {
		return _modifier;
	}
}
