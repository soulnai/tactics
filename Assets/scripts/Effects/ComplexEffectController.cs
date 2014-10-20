using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComplexEffectController : MonoBehaviour {
	public List<string> complexEffectList;
	public List<ComplexEffect> complexEffects = new List<ComplexEffect>();
	// Use this for initialization
	void Awake()
	{
		initComplexEffects();
	}
	
	void Start () {
		
	}

	public void Delete (ComplexEffect ce)
	{
		complexEffects.Remove(ce);
	}
	
	public void initComplexEffects (){
		Unit u = GetComponent<Unit>();
		for(int i=0;i<complexEffects.Count;i++)
		{
			complexEffects.Add(ComplexEffectManager.instance.getComplexEffect(complexEffectList[i]).Clone() as ComplexEffect);
		}
		foreach(ComplexEffect ce in complexEffects)
		{
			ce.owner = u;
			ce.controller = this;
		}
	}
}