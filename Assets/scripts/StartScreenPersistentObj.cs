using UnityEngine;
using System.Collections;

public class StartScreenPersistentObj : MonoBehaviour {

	public GameObject[] UserUnitPrefab;
	public static StartScreenPersistentObj instance;

	// Use this for initialization
	void Awake () {
		instance = this;
		DontDestroyOnLoad (this);
	}

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
