using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(FXmanager))]
public class FXmanagerEditor : Editor {

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		FXmanager myscript = (FXmanager)target;
		if(GUILayout.Button("Save"))
		{
//			AbilitiesSaveLoad.Save(AbilitiesSaveLoad.createContainer(myscript.abilities),"AbilitiesManager.xml");
		}
		if(GUILayout.Button("Load"))
		{
//			loadAbilitiesToManager(myscript);
		}
	}
}
