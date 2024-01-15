using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeOrder  
{
    public readonly int order;
    public readonly Type type;

    public TypeOrder(int order, Type type)
    {
        this.order = order;
        this.type = type;
    }
}
