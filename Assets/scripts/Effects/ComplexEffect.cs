using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnumSpace;
using System;
using System.Xml;
using System.Xml.Serialization;

[System.Serializable]
public class ComplexEffect : ICloneable {
	public string ID;
	public string name;
	
	public List<BaseEffect> effects;

	public bool useRadius = true;
	public int radius;

	public bool requireTarget = true;
	public bool enemieUse = false;
	public bool allyUse = false;
	public bool selfUse = false;
	
	public bool infinite = false;
	public int duration = 1;

	[HideInInspector]
	public Unit owner;

	public List<Unit> targets;
	public GameObject FX;

	[HideInInspector]
	public ComplexEffectController controller;

	private GameManager gm;
	public void Init()
	{
		gm = GameManager.instance;
		foreach(BaseEffect ef in effects)
		{
			ef.requireTarget = requireTarget;
			ef.enemieUse = enemieUse;
			ef.allyUse = allyUse;
			ef.selfUse = selfUse;
			ef.infinite = infinite;
			ef.duration = duration;
			ef.targets = targets;
		}
	}

	public void ActivateAura()
	{
		targets = gm.findTargets(owner,radius,enemieUse,allyUse,selfUse);
		if(!infinite){
			duration--;
			if(duration <=0)
				Delete();
		}
		foreach(Unit u in targets)
		{
			foreach(BaseEffect ef in effects)
			{
				ef.applyTo(u);
			}
		}
	}

	public void Delete()
	{
		foreach(BaseEffect ef in effects)
		{
			ef.Delete();
		}
		effects.Clear();
		controller.Delete(this);
	}

	public object Clone()
	{
		return this.MemberwiseClone();
	}
}
