using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDecorator : MonoBehaviour {

	public Material[] Shoes;
	public Material[] DressFront;
	public Material[] DressSides;

	public string [] Names;

	// Use this for initialization
	void Awake () {
		Material shoe = Shoes [Random.Range (0, Shoes.Length)];

		int randomDress = Random.Range (0, DressFront.Length);
		Material dressFront = DressFront [randomDress];
		Material dressSides = DressSides [randomDress];

		transform.GetChild (0).GetChild (0).GetComponent<MeshRenderer> ().material = dressFront;
		transform.GetChild (0).GetChild (1).GetComponent<MeshRenderer> ().material = dressSides;
		transform.GetChild (0).GetChild (2).GetComponent<MeshRenderer> ().material = dressSides;
		transform.GetChild (0).GetChild (3).GetComponent<MeshRenderer> ().material = dressSides;

		transform.GetChild (2).GetComponent<MeshRenderer> ().material = shoe;
		transform.GetChild (3).GetComponent<MeshRenderer> ().material = shoe;

		string name = "";
		do {
			name = Names [Random.Range (0, Names.Length)];
			foreach(GameObject go in GameObject.FindGameObjectsWithTag("Character"))
				if (go.GetComponent<CharacterInteraction>().GetName() == name)
					name = "";
			if (GameObject.FindGameObjectWithTag("Player") != null) {
				if (GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterInteraction>().GetName() == name)
					name = "";
			}
		} while (name == "");

		GetComponent<CharacterInteraction> ().SetName (name);
	}

	void Start () {
		GameObject go = GameObject.FindGameObjectWithTag ("Player");
		if (go != null)
			go.GetComponent<CharacterInteraction> ().UpdateLeaderboard ();
		if (tag == "Player")
			GameObject.Find ("Name").GetComponent<Text> ().text = GetComponent<CharacterInteraction> ().GetName ().ToUpper ();
		Destroy (this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
