using System;
using UnityEngine;

//from https://github.com/lachlansleight/Lunity
namespace Lunity
{
    /// <summary>
    ///     Stick this on a method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class EditorButtonAttribute : PropertyAttribute { }
}