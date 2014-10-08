using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AbilitiesManager : MonoBehaviour {

	public static AbilitiesManager instance;
	public List<BaseAbility> abilities;

	public void Awake()
	{
		instance = this;
	}

	public BaseAbility getAbility(string ID)
	{
		return abilities.Find(BaseAttack => BaseAttack.attackID == ID); 
	}
}
