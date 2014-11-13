using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseEffect : ICloneable {
	public string ID;
	public string name;
	public Sprite icon;
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
    private List<Unit> targetsToDelete = new List<Unit>();
	public GameObject FX;

	private GameManager gm{
		get{
			return GameManager.instance;
		}
	}

	public void Init(Unit u,Unit target = null)
	{
        targets = new List<Unit>();
        owner = u;
        if (!infinite)
            UnitEvents.OnPlayerTurnEnd += checkDuration;
        UnitEvents.OnPlayerTurnEnd += deleteUnusedTargets;
        if (useRadius)
        {
            UnitEvents.OnUnitPosChange += updateTargetsInRadius;
            updateTargetsInRadius();
        }
        else if (target != null)
        {
            targets.Clear();
            targets.Add(target);
            addApliedEffect(targets);
            applyAttributeMods(targets);
        }
        UnitEvents.OnUnitDead += deleteFromTargets;
        UnitEvents.OnPlayerTurnStart += ActivateEffect;
	}

    private void deleteUnusedTargets(Player player)
    {
        foreach (Unit u in targetsToDelete)
            targets.Remove(u);
    }

    void addToDeleteList(Unit u)
    {
        if (targets.Contains(u))
            targetsToDelete.Add(u);
    }
    void deleteFromTargets(Unit u)
    {
        if (targets.Contains(u))
            addToDeleteList(u);
    }

	void checkDuration (Player p)
	{
		//Duration check
		if(owner.playerOwner != p) {
			duration--;
            UnitEvents.UnitEffectChanged(owner, this);
            if (duration <= 0) {
                owner.unitEffects.deleteEffect(this);
            }
		}
	}
	
	public void updateTargetsInRadius(Unit u = null){
        if (useRadius)
        {
            targets = gm.findTargets(owner, radius, enemieUse, allyUse, selfUse);
            addApliedEffect(targets);
            applyAttributeMods(targets);
            UnitEvents.UnitEffectChanged(owner, this);
        }
	}

    public void addApliedEffect(List<Unit> targets)
    {
        foreach (Unit u in targets)
        {
            if(CanBeAdded(u))
                u.unitEffects.addAppliedEffect(this);
        }
    }

	public void applyAttributeMods(List<Unit> targetList)
	{
        if (targets.Count > 0)
        {
            if ((duration > 0) || (infinite))
            {
                foreach (BaseAttributeChanger ac in affectedAttributes)
                {
                    ac.applyAttributeMod(targetList);
                    Debug.Log("Attribute Value changed");
                }
            }
        }
	}

	public object Clone()
	{
		return this.MemberwiseClone();
	}

	public bool CanBeAdded(Unit u){
		List<BaseEffect> effects = u.unitEffects.effectsApplied;
		//trying to add same copy of the effect to unit - false
		if(effects.Contains (this))
			return false;
		//trying to add same effect but not same copy(another owner / etc.) - check more details
		if(effects.Find(BaseEffect => BaseEffect.ID == ID)!= null){
		if(effects.Find(BaseEffect => BaseEffect.ID == ID).ID == ID){
			BaseEffect appliedEf = effects.Find(BaseEffect => BaseEffect.ID == ID) as BaseEffect;
			if(!infinite)
			{
				//update duration if new duration > old
				if((duration > appliedEf.duration)&&(!appliedEf.infinite)){
					appliedEf.duration = duration;
                    UnitEvents.UnitEffectChanged(owner, appliedEf);
					return false;
				}
				//infinite - false
				else
					return false;
			}
			//if new is infinite - false
			else{
				//remove old apply this
				return true;
			}
		}
		}
		return true;
	}

    public void ActivateEffect(Player p) {
        foreach (BaseAttributeChanger ac in affectedAttributes)
        {
            ac.applyAttributeChanger(targets);
        }
    }

	void OnDestroy(){
        UnitEvents.UnitEffectRemoved(owner, this);
		UnitEvents.OnPlayerTurnEnd -= checkDuration;
	}
}
