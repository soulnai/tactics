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

	public bool deleteAfterOwnerDeath = true;
	public bool infinite = false;
	public int duration = 1;

	public Unit owner;
//	[HideInInspector]
	public List<Unit> targets;

	public GameObject FX;

	private bool useUpdateTargets = true;

	private GameManager gm{
		get{
			return GameManager.instance;
		}
	}

	public void Init(Unit _owner = null,Unit _target = null)
	{
//		gm.OnUnitTurnStart += OnTurnStart;
//		gm.OnPlayerTurnStart += OnTurnStart;
		gm.OnPlayerTurnEnd += PlayerTurnEnd;
		gm.OnRoundStart += OnRoundStart;
		gm.OnRoundEnd += OnRoundEnd;
		if(_owner != null)
			owner = _owner;
		if(_target!=null){
			targets = new List<Unit>();
			useUpdateTargets = false;
			targets.Add(_target);
			addToAppliedEffects ();
		}
	}

	void OnRoundStart ()
	{
		//Duration check
//		if(!infinite){
//			duration--;
//			if(duration<=0)
//				Delete();
//		}
	}

	void PlayerTurnEnd (Player p)
	{
		//Duration check
		if((!infinite)&&(owner.playerOwner != p)){
			duration--;
			UnitEvents.UnitEffectChanged(owner,this);
			if(duration<=0)
				Delete();
		}
	}

	void OnRoundEnd()
	{

	}

	void OnTurnStart (Unit currentUnit)
	{
//		if((infinite)||(duration>0)){
//			updateTargets();
//			if(targets.Contains(gm.currentUnit)){
//				applyTo(gm.currentUnit);
//			}
//		}
	}

	public void updateTargets(Unit u = null){
		removeFromAppliedEffects();
		if(useUpdateTargets){
			targets.Clear();
			if(useRadius)
			{
				targets = gm.findTargets(owner,radius,enemieUse,allyUse,selfUse);
			}
			else
			{
				if(selfUse)
					targets.Add(owner);
				if(u != null){
					if(allyUse)
						if(u.playerOwner == owner.playerOwner)
							targets.Add(u);
					if(enemieUse)
						if(u.playerOwner != owner.playerOwner)
							targets.Add(u);
				}
			}
		}
		addToAppliedEffects ();
	}

	public void applyTo(Unit u)
	{
		if(targets.Contains(u)){
			if((duration>0)||(infinite)){
				foreach(BaseAttributeChanger ac in affectedAttributes)
				{
					int valueTemp = getValue(u,ac);

					if((ac.applyEachTurn)&&(!ac.mod)){
						u.getAttribute(ac.attribute).value += valueTemp;
						if(ac.attribute == unitAttributes.HP)
							u.checkHP();
						Debug.Log("Attribute Value changed");
					}
					else if(ac.mod)
					{
						if(u == gm.currentUnit){
							u.getAttribute(ac.attribute).addMod(valueTemp);
							Debug.Log("Mods list updated");
						}
					}
				}
			}
		}
	}

	public int getValue (Unit u,BaseAttributeChanger ac)
	{
		int valueTemp = 0;
		int valueMod = u.getAttribute (ac.attribute).valueMod;
		int value = u.getAttribute (ac.attribute).value;
		if (ac.multiply)
			valueTemp = UnityEngine.Mathf.RoundToInt ((ac.value * value) - value);
		else
			valueTemp = UnityEngine.Mathf.RoundToInt ((ac.value + value) - value);
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

	public void setTarget (Unit target)
	{
		targets.Clear();
		targets.Add(target);
		useUpdateTargets = false;
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
