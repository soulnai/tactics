using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class buttonController : MonoBehaviour {
	public Text text;
	public object assignedElement;
	// Use this for initialization
	void Start () {

	}

	public void setText(string t){
		text.text = t;
	}
}
