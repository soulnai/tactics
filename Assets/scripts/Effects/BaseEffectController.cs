using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseEffectController : MonoBehaviour {

	public List<string> effectsID;
	public List<BaseEffect> effects = new List<BaseEffect>();

	public List<BaseEffect> effectsApplied = new List<BaseEffect>();

	private Unit owner;
	void Awake () {
		owner = GetComponent<Unit>();
	}

	public void initStartEffects (){
        EventManager.OnUnitEffectChanged += checkIfNeedToRemove;
        EventManager.OnUnitEffectRemoved += deleteAppliedEffect;
		for(int i=0;i<effectsID.Count;i++)
		{
			if(BaseEffectsManager.instance.getEffect(effectsID[i]) != null){
				addEffect(BaseEffectsManager.instance.getEffect(effectsID[i]));
			}
		}
	}


	public void addEffect(BaseEffect ef,Unit targetUnit = null)
	{
        if(!effects.Contains(ef)){

            if (targetUnit == null)
            {
                ef.Init(owner);
                effects.Add(ef);
            }
            else
            {
                if (ef.CanBeAdded(targetUnit))
                {
                    ef.Init(owner, targetUnit);
                    effects.Add(ef);
                }
            }
			
		}
		else
			Debug.Log("effect already applied");
	}

	public void deleteEffect(BaseEffect ef)
	{
		if(effects.Contains(ef)){
			effects.Remove(ef);
            foreach (Unit u in ef.targets)
            {
                u.unitEffects.deleteAppliedEffect(u,ef);
            }

		}
		else
			Debug.Log("No such effect");
	}

	public void addAppliedEffect(BaseEffect ef)
	{
		if(!effectsApplied.Contains(ef)){
			effectsApplied.Add(ef);
			EventManager.UnitEffectAdded(owner,ef);
		}
		else
			Debug.Log("This effect already applied - "+ef.name);
	}

    public void checkIfNeedToRemove(Unit u, BaseEffect ef) {
        if (!ef.targets.Contains(owner))
        {
            deleteAppliedEffect(owner, ef);
        }
    }
	public void deleteAppliedEffect(Unit u,BaseEffect ef)
	{
        if ((u == owner)&&(effectsApplied.Contains(ef)))
        {
            effectsApplied.Remove(ef);
            EventManager.UnitEffectRemoved(owner, ef);
            foreach (BaseAttributeChanger ac in ef.affectedAttributes)
            {
                u.getAttribute(ac.attribute).removeMod(ac);
            }
        }
	}

    void OnDestroy()
    {
        EventManager.OnUnitEffectChanged -= checkIfNeedToRemove;
        EventManager.OnUnitEffectRemoved -= deleteAppliedEffect;
    }
}
