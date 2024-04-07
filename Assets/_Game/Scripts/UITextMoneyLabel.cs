using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class UITextMoneyLabel : Label
{
    [SerializeField, DontCreateProperty]
    string internalText;
    
    [UxmlAttribute, CreateProperty]
    public string Text
    {
        get => internalText;
        set
        {
            internalText = value;
            if (long.TryParse(value, out long parsedValue))
            {
                text = "$" + (parsedValue/100).ToString("#,##0") + "." + (parsedValue % 100).ToString("D2");
            }
            else
            {
                text = "$0.00";
            }
            MarkDirtyRepaint();
        }
    }

    public UITextMoneyLabel()
    {
        
    }
}
