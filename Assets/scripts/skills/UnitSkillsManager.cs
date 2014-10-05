using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitSkillsManager : MonoBehaviour {


	public List<string> skillsList;
	private List<BaseAttack> attacks = new List<BaseAttack>();
	// Use this for initialization
	void Awake () {

	}
	void Start () {
		for(int i=0;i<skillsList.Count;i++)
		{
			attacks.Add(AbilitiesManager.instance.getAbility(skillsList[i]));
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}