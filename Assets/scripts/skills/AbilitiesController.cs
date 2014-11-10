using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilitiesController : MonoBehaviour {

	public List<string> abilitiesList;
	public List<BaseAbility> abilities = new List<BaseAbility>();
	// Use this for initialization
	void Awake () {

	}

	void Start () {
		initAbilities();
		GUImanager.instance.initAbilities();
	}

	public void initAbilities (){
		for(int i=0;i<abilitiesList.Count;i++)
		{
			abilities.Add(AbilitiesManager.instance.getAbility(abilitiesList[i]));
		}
	}
	// Update is called once per frame
	void Update () {
	
	}
}
