using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerSettings
{
    public static readonly int Default = LayerMask.NameToLayer("Default");
    public static readonly int Ingame = LayerMask.NameToLayer("Ingame");
    public static readonly int UI = LayerMask.NameToLayer("UI");
    public static readonly int Background = LayerMask.NameToLayer("Background");
}
