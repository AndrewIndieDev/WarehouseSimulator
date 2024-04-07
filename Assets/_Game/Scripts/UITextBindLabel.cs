using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class UITextBindLabel : Label
{
    [SerializeField, DontCreateProperty]
    string internalText;
    
    [UxmlAttribute]
    string prefix;
    
    [UxmlAttribute, CreateProperty]
    public string Text
    {
        get => internalText;
        set
        {
            internalText = value;
            text = prefix + value;
            MarkDirtyRepaint();
        }
    }

    public UITextBindLabel()
    {
        
    }
}
