using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnumSpace;
using System;
using System.Xml;
using System.Xml.Serialization;

[System.Serializable]
public class BaseEffect : ICloneable {
	public string ID;
	public string name;
	
	public List<BaseAttributeChanger> affectedAttributes;

	//radius use
	public bool useRadius = true;
	public int radius;

	//target filters
	public bool requireTarget = true;
	public bool enemieUse = false;
	public bool allyUse = false;
	public bool selfUse = false;

	public bool infinite = false;
	public int duration = 1;

	public Unit owner;
//	[HideInInspector]
	public List<Unit> targets;

	public GameObject FX;

	private bool useUpdateTargets = true;
	private int mod;

	private GameManager gm{
		get{
			return GameManager.instance;
		}
	}

	public void Init(Unit _owner = null,Unit _target = null)
	{
		gm.OnTurnStart += OnTurnStart;
		gm.OnRoundStart += OnRoundStart;
		if(_owner != null)
			owner = _owner;
		if(_target!=null){
			useUpdateTargets = false;
			targets.Clear();
			targets.Add(_target);
			addToAppliedEffects ();
		}
	}

	void OnRoundStart ()
	{
		//Duration check
		if(!infinite){
			duration--;
			if(duration<=0)
				Delete();
		}
	}

	void OnTurnStart (Unit currentUnit)
	{
		if((infinite)||(duration>0)){
			updateTargets();
			if(targets.Contains(gm.currentUnit)){
				applyTo(gm.currentUnit);
			}
		}
	}

	public void updateTargets(Unit u = null){
		if(useUpdateTargets){
			removeFromAppliedEffects();
			targets.Clear();
			if(useRadius)
			{
				targets = gm.findTargets(owner,radius,enemieUse,allyUse,selfUse);
			}
			else
			{
				if(selfUse)
					targets.Add(owner);
				if(allyUse)
					if(u.playerOwner == owner.playerOwner)
						targets.Add(u);
				if(enemieUse)
					if(u.playerOwner != owner.playerOwner)
						targets.Add(u);
			}
			addToAppliedEffects ();
		}
	}

	public void applyToAllTargets()
	{
		updateTargets();
		foreach(Unit u in targets)
		{
			applyTo(u);
		}
	}

	public void applyTo(Unit u)
	{
		if((duration>0)||(infinite)){
			foreach(BaseAttributeChanger ac in affectedAttributes)
			{
				int valueTemp;
				valueTemp = calculateValue(u,ac);

				if(ac.applyEachTurn){
					u.getAttribute(ac.attribute).value += valueTemp;
				}
				else
				{
					u.getAttribute(ac.attribute).addMod(valueTemp);
				}
			}
		}
	}

	public int calculateValue (Unit u,BaseAttributeChanger ac)
	{
		int valueTemp = 0;
		int valueMod = u.getAttribute (ac.attribute).valueMod;
		int value = u.getAttribute (ac.attribute).value;
		if (ac.multiply)
			valueTemp = UnityEngine.Mathf.RoundToInt ((ac.value * valueMod) - value);
		else
			valueTemp = UnityEngine.Mathf.RoundToInt ((ac.value + valueMod) - value);
		return valueTemp;
	}

	void addToAppliedEffects ()
	{
		foreach (Unit t in targets) {
			if (!t.unitBaseEffects.effectsAppliedToUnit.Contains (this))
				t.unitBaseEffects.addAppliedEffect (this);
		}
	}

	void removeFromAppliedEffects ()
	{
		foreach (Unit t in targets) {
			if (t.unitBaseEffects.effectsAppliedToUnit.Contains (this))
				t.unitBaseEffects.removeAppliedEffect (this);
		}
	}

	public void Delete()
	{
		removeFromAppliedEffects ();
		owner.unitBaseEffects.delete(this);
	}
	public object Clone()
	{
		return this.MemberwiseClone();
	}
}
