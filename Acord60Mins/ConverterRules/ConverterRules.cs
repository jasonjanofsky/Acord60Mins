
/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class ConverterRules
{

	private ConverterRule[] converterRuleField;

	/// <remarks/>
	[System.Xml.Serialization.XmlElementAttribute("ConverterRule")]
	public ConverterRule[] ConverterRule
	{
		get
		{
			return this.converterRuleField;
		}
		set
		{
			this.converterRuleField = value;
		}
	}
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class ConverterRule
{

	private string nameField;

	private string classField;

	/// <remarks/>
	public string Name
	{
		get
		{
			return this.nameField;
		}
		set
		{
			this.nameField = value;
		}
	}

	/// <remarks/>
	public string Class
	{
		get
		{
			return this.classField;
		}
		set
		{
			this.classField = value;
		}
	}
}

