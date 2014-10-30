using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(AbilitiesManager))]
public class AbilitiesManagerEditor : Editor {

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		AbilitiesManager myscript = (AbilitiesManager)target;
		if(GUILayout.Button("Save"))
		{
			AbilitiesSaveLoad.Save(AbilitiesSaveLoad.createContainer(myscript.abilities),"AbilitiesManager.xml");
		}
		if(GUILayout.Button("Load"))
		{
			loadAbilitiesToManager(myscript);
		}
	}

	public void loadAbilitiesToManager(AbilitiesManager am){
		am.abilities = new List<BaseAbility>();
		AbilitiesContainer ac = AbilitiesSaveLoad.Load("AbilitiesManager.xml");

		foreach(AbilityXML axml in ac.abilitiesXML)
		{
			am.abilities.Add(AbilitiesSaveLoad.createAbility(axml));
		}
	}
}
