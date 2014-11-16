using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class AbilitiesManager : MonoBehaviour {

	public static AbilitiesManager instance;

	public List<BaseAbility> abilities;

	public void Awake()
	{
		instance = this;
	}

	public BaseAbility getAbility(string ID)
	{
	    BaseAbility tempAbility = null;
	    if (ID == "")
	    {
            Debug.Log("ID not set");
	    }
	    else
	    {
            if (abilities.Find(BaseAttack => BaseAttack.abilityID == ID) != null)
	        {
	            tempAbility = abilities.Find(BaseAttack => BaseAttack.abilityID == ID).Clone() as BaseAbility;
	        }
	        else
	        {
	            Debug.Log("No such ability - " + ID);
	        }
	    }
	    return tempAbility;
	}


}
