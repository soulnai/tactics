using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("EffectsContainer")]
public class EffectsContainer {
	[XmlArray("Effects")]
	[XmlArrayItem("Effect")]
	public List<BaseEffect> effectsList;

	public void Save(string path,List<BaseEffect> ef)
	{
		effectsList = ef;
		var serializer = new XmlSerializer(typeof(EffectsContainer));
		using(var stream = new FileStream(path, FileMode.Create))
		{
			serializer.Serialize(stream, this);
		}
	}
	
	public static EffectsContainer Load(string path)
	{
		var serializer = new XmlSerializer(typeof(EffectsContainer));
		using(var stream = new FileStream(path, FileMode.Open))
		{
			return serializer.Deserialize(stream) as EffectsContainer;
		}
	}
	
	//Loads the xml directly from the given string. Useful in combination with www.text.
	public static EffectsContainer LoadFromText(string text) 
	{
		var serializer = new XmlSerializer(typeof(EffectsContainer));
		return serializer.Deserialize(new StringReader(text)) as EffectsContainer;
	}
}
