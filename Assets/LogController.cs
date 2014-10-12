using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LogController : MonoBehaviour {
	public Text textLog;
	public Scrollbar scroll;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setScroll(float val = 0)
	{
		scroll.value = val;
	}

	public void addText(string t)
	{
		textLog.text += t+"\n";
		setScroll();
	}
}
