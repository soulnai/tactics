using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(UnitAttributesHelper))]
public class UnitAttributesHelperEditor : Editor {

	public override void OnInspectorGUI()
	{
		UnitAttributesHelper myscript = (UnitAttributesHelper)target;
		if(GUILayout.Button("Create Attributes"))
		{
			myscript.createAttributesList();
		}
	}
}
