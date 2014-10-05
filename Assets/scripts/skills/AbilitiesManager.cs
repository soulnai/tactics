using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AbilitiesManager : MonoBehaviour {

	public static AbilitiesManager instance;
	public List<BaseAttack> attacks;

	public void Awake()
	{
		instance = this;
	}

	public BaseAttack getAbility(string ID)
	{
		return attacks.Find(BaseAttack => BaseAttack.attackID == ID); 
	}
}
