namespace NetcodeXR.Utility.Attributes
{
    using System;
    using UnityEngine;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class ConditionalHideInInspectorAttribute : PropertyAttribute
    {
        public string ComparedPropertyName { get; private set; }

        public ConditionalHideInInspectorAttribute(string InComparedPropertyName)
        {
            ComparedPropertyName = InComparedPropertyName;
        }
    }

}
