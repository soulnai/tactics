using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PassiveAbilitiesController : MonoBehaviour {
	public List<string> passiveAbilitiesList;
	public List<BasePassiveAbility> passiveAbilities = new List<BasePassiveAbility>();
	// Use this for initialization
	void Awake()
	{
		initPassiveAbilities();
	}

	void Start () {

	}
	
	public void initPassiveAbilities (){
		Unit u = GetComponent<Unit>();
		for(int i=0;i<passiveAbilitiesList.Count;i++)
		{
			passiveAbilities.Add(PassiveAbilitiesManager.instance.getPassiveAbility(passiveAbilitiesList[i]).Clone() as BasePassiveAbility);
		}
		foreach(BasePassiveAbility pa in passiveAbilities)
		{
			pa.owner = u;
		}
	}
}