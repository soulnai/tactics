using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class buttonTextController : MonoBehaviour {
	public Text text;
	// Use this for initialization
	void Start () {

	}

	public void setText(string t){
		text.text = t;
	}
}
