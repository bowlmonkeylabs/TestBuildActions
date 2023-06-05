using System;
using BML.ScriptableObjectCore.Scripts.Variables.ValueReferences;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BML.ScriptableObjectCore.Scripts.Variables
{
    [Required]
    [CreateAssetMenu(fileName = "BoolVariable", menuName = "BML/Variables/BoolVariable", order = 0)]
    public class BoolVariable : Variable<bool>, IFloatValue
    {
        public string GetName() => name;
        public string GetDescription() => Description;
        public float GetFloat() => Value ? 1f : 0f;
        public float GetValue(System.Type type) => Value ? 1f : 0f;
    }
}