using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class BaseMelee : Skills {
	

	// Use this for initialization
	void Start () {
		Name = "Melee attack";
		Range = 1;
		BaseDamage = 50;
		Description = "melee bla-bla";

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
