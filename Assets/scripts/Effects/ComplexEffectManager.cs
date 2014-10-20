using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.IO;


public class ComplexEffectManager : MonoBehaviour {
	public static ComplexEffectManager instance;
	
	public List<ComplexEffect> complexEffectsList;
	
	void Awake()
	{
		instance = this;
		
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Delete(ComplexEffect a)
	{
		a.Delete();
		complexEffectsList.Remove(a);
	}
	
	public ComplexEffect getComplexEffect(string ID)
	{
		return complexEffectsList.Find(complexEffect => complexEffect.ID == ID);
	}
}
