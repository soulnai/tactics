using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PassiveAbilitiesManager : MonoBehaviour {
	
	public static PassiveAbilitiesManager instance;
	public List<BasePassiveAbility> passiveAbilities;
	
	public void Awake()
	{
		instance = this;
	}
	
	public BasePassiveAbility getPassiveAbility(string ID)
	{
		return passiveAbilities.Find(BasePassiveAbility => BasePassiveAbility.abilityID == ID); 
	}
}
