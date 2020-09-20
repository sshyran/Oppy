﻿using System;
using Ultz.Oppy.Core;

namespace Ultz.Oppy.Content.Injection
{
    /// <summary>
    /// An attribute which instructs the needle to inject an instance of a session object, such as a <see cref="Host"/>
    /// or <see cref="Listener"/>, determined by the type of the property, method, or field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
    public class OppyInjectAttribute : Attribute
    {
        
    }
}