using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using EnumSpace;


public class AbilityXML{
	public string abilityID;
	public string abilityName;
	
	//Cost
	public int APcost;
	public int MPCost;
	public int CastTime;
	
	public bool endsUnitTurn = true;
	public attackTypes attackType;
	
	//target flags
	public bool requireTarget = true;
	
	public bool enemieUse = false;
	public bool allyUse = false;
	public bool selfUse = false;
	
	public int range;
	public bool areaDamage;
	public int areaDamageRadius;
	public areaPatterns areaPattern;
	
	public int baseDamage;
	public bool canBackstan = false;
	public damageTypes damageType;
	
	public List<string> effects;
	
	//FX
	public string hitFXprefab;
	public string rangedFXprefab;
}


[XmlRoot("AbilitiesList")]
public class AbilitiesContainer{
	[XmlArray("Abilities")]
	[XmlArrayItem("Ability")]
	public List<AbilityXML> abilitiesXML = new List<AbilityXML>();
}

public static class AbilitiesSaveLoad{
	public static AbilitiesContainer createContainer(List<BaseAbility> _abilities)
	{
		List<AbilityXML> abilitiesXML = new List<AbilityXML>();
		foreach(BaseAbility ab in _abilities){
			abilitiesXML.Add(createAbilityXML(ab));
		}
		return new AbilitiesContainer{abilitiesXML = abilitiesXML};
	}

	public static AbilityXML createAbilityXML(BaseAbility ab){
		AbilityXML tempAbility 		= 	new AbilityXML();
		tempAbility.abilityID 		= 	ab.abilityID;
		tempAbility.abilityName 	= 	ab.abilityName;
		tempAbility.allyUse 		=	ab.allyUse;
		tempAbility.APcost 			=	ab.APcost;
		tempAbility.areaDamage 		=	ab.areaDamage;
		tempAbility.areaDamageRadius=	ab.areaDamageRadius;
		tempAbility.areaPattern		=	ab.areaPattern;
		tempAbility.attackType		=	ab.attackType;
		tempAbility.baseDamage		=	ab.baseDamage;
		tempAbility.canBackstan		=	ab.canBackstan;
		tempAbility.CastTime 		=	ab.CastTime;
		tempAbility.damageType 		=	ab.damageType;
		tempAbility.effects 		=	ab.effects;
		tempAbility.endsUnitTurn 	=	ab.endsUnitTurn;
		tempAbility.enemieUse 		=	ab.enemieUse;
		tempAbility.MPCost 			=	ab.MPCost;
		tempAbility.range			=	ab.range;
		tempAbility.requireTarget 	=	ab.requireTarget;
		tempAbility.selfUse			=	ab.selfUse;

		if(ab.hitFXprefab != null){
//			FXmanager.instance.addFX(ab.hitFXprefab);
			tempAbility.hitFXprefab 	=	ab.hitFXprefab.name;
		}
		if(ab.rangedFXprefab != null){
//			FXmanager.instance.addFX(ab.rangedFXprefab);
			tempAbility.rangedFXprefab	=	ab.rangedFXprefab.name;
		}
		return tempAbility;
	}

	public static BaseAbility createAbility(AbilityXML tempAbility){
		BaseAbility ab = new BaseAbility();
		ab.abilityID			=	tempAbility.abilityID 		 	;
		ab.abilityName			=	tempAbility.abilityName 	 	;
		ab.allyUse				=	tempAbility.allyUse 			;
		ab.APcost				=	tempAbility.APcost 			;	
		ab.areaDamage			=	tempAbility.areaDamage 		;	
		ab.areaDamageRadius		=	tempAbility.areaDamageRadius	;
		ab.areaPattern			=	tempAbility.areaPattern		;	
		ab.attackType			=	tempAbility.attackType			;
		ab.baseDamage			=	tempAbility.baseDamage			;
		ab.canBackstan			=	tempAbility.canBackstan		;	
		ab.CastTime				=	tempAbility.CastTime 			;
		ab.damageType			=	tempAbility.damageType 		;	
		ab.effects				=	tempAbility.effects 			;
		ab.endsUnitTurn			=	tempAbility.endsUnitTurn 		;
		ab.enemieUse			=	tempAbility.enemieUse 			;
		ab.MPCost				=	tempAbility.MPCost 			;	
		ab.range				=	tempAbility.range				;
		ab.requireTarget		=	tempAbility.requireTarget 		;
		ab.selfUse 				=	tempAbility.selfUse			;	

//		if((tempAbility.hitFXprefab != null)||(tempAbility.hitFXprefab != ""))
//			ab.hitFXprefab = FXmanager.instance.getFX(tempAbility.hitFXprefab);
//		if((tempAbility.rangedFXprefab != null)||(tempAbility.rangedFXprefab != ""))
//			ab.rangedFXprefab = FXmanager.instance.getFX(tempAbility.rangedFXprefab);
		return ab;
	}

	public static void Save(AbilitiesContainer abilitiesContainer, string path)
	{
		var serializer = new XmlSerializer(typeof(AbilitiesContainer));
		using(var stream = new FileStream(path, FileMode.Create))
		{
			serializer.Serialize(stream, abilitiesContainer);
		}
	}
	
	public static AbilitiesContainer Load(string path)
	{
		var serializer = new XmlSerializer(typeof(AbilitiesContainer));
		using(var stream = new FileStream(path, FileMode.Open))
		{
			return serializer.Deserialize(stream) as AbilitiesContainer;
		}
	}
	
	//Loads the xml directly from the given string. Useful in combination with www.text.
	public static AbilitiesContainer LoadFromText(string text) 
	{
		var serializer = new XmlSerializer(typeof(AbilitiesContainer));
		return serializer.Deserialize(new StringReader(text)) as AbilitiesContainer;
	}
}
