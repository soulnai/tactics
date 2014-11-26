using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AttributeSliderController : MonoBehaviour
{
    public Slider ValueSlider;
    public Text ValueText;
    public Vector2 Value = new Vector2(0,0);
    public Color SliderColor;
    public BaseAttribute Attribute;
	// Use this for initialization
	void Awake ()
	{
	    ValueSlider = GetComponent<Slider>();
	}

    public void Init(BaseAttribute _attribute)
    {
        if (Attribute != _attribute)
        {
            Attribute = _attribute;
            EventManager.onAttributeChanged += ChangeValue;    
            ChangeValue(_attribute.getOwner(),_attribute);
        }
    }

    public void ChangeValue(Unit u,BaseAttribute at)
    {
        if (at == Attribute)
        {
            Value.x = at.valueMod;
            Value.y = BaseAttribute.GetMaxAttributeValue(at);
            SliderColor = ColorHolder.instance.GetAttributeColor(at);

            ValueSlider.value = ((float) Value.x/(float) Value.y);
            if (ValueText != null)
            {
                ValueText.text = Value.x + "/" + Value.y;
            }
        }
    }

    private void OnDestroy()
    {
        EventManager.onAttributeChanged -= ChangeValue;
    }
}
