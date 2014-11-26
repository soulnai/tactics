using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectsPanelControllerGUI : MonoBehaviour {
	public List<EffectPanelGUI> effectPanels;

	private Unit owner;

	public void Init(Unit u){
		owner = u;
	    clearAllPanels();
	    foreach (BaseEffect ef in u.unitEffects.effectsApplied)
	    {
	        addEffectPanel(u,ef);
	    }
	}

    private void clearAllPanels()
    {
        foreach (EffectPanelGUI efPanel in effectPanels)
        {
            efPanel.Reset();
        }
    }

	// Use this for initialization
	void Start () {
		EventManager.OnUnitEffectChanged += updateEffectPanels;
		EventManager.OnUnitEffectAdded += addEffectPanel;
		EventManager.OnUnitEffectRemoved += removeEffectPanel;
	}

	void addEffectPanel(Unit u, BaseEffect ef)
	{
		if(u == owner){
            if (effectPanels.Find(EffectPanelGUI => EffectPanelGUI.effect == null) != null)
		    {
                effectPanels.Find(EffectPanelGUI => EffectPanelGUI.effect == null).Init(ef);    
		    }
            else
            {
                Debug.Log("No unused effects panel found");
            }
            
		}
	}

	void removeEffectPanel(Unit u, BaseEffect ef)
	{
		if(u == owner){
			EffectPanelGUI tempPanel = effectPanels.Find(EffectPanelGUI => EffectPanelGUI.effect == ef) as EffectPanelGUI;
			if(tempPanel != null){
				tempPanel.Reset();
			}
		}
	}

	void updateEffectPanels (Unit u, BaseEffect ef)
	{
        if (ef.targets.Contains(owner))
        {
            foreach (EffectPanelGUI p in effectPanels)
            {
                if (p.effect == ef)
                {
                    p.updateCounter();
                }
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDestroy(){
		EventManager.OnUnitEffectChanged -= updateEffectPanels;
		EventManager.OnUnitEffectAdded -= addEffectPanel;
		EventManager.OnUnitEffectRemoved -= removeEffectPanel;
	}
}
