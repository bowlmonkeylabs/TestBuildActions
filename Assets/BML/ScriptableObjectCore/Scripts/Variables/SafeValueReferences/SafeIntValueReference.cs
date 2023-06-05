using System;
using System.Collections;
using System.Data.SqlTypes;
using System.Linq;
using BML.ScriptableObjectCore.Scripts.CustomAttributes;
using BML.ScriptableObjectCore.Scripts.Variables;
using Mono.CSharp;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace BML.ScriptableObjectCore.Scripts.Variables.SafeValueReferences
{
    [InlineProperty]
    [SynchronizedHeader]
    [HideReferenceObjectPicker]
    [Serializable]
    public class SafeIntValueReference
    {
        #region Inspector
        
        public String Tooltip => !UseConstant ? this.Description : "";
        
        protected bool UseConstant => ReferenceTypeSelector == IntReferenceTypes.Constant;
        
        public enum IntReferenceTypes {
            Constant = 0,
            IntVariable = 1,
            FloatVariable = 2,
            BoolVariable = 3,
        }

        public enum IntRoundingBehavior
        {
            Truncate = 0,
            Round = 1,
        }
        
        [VerticalGroup("Top")]
        [HorizontalGroup("Top/Split", LabelWidth = 0.001f)]
        [HorizontalGroup("Top/Split/Left", LabelWidth = .01f)]
        [BoxGroup("Top/Split/Left/Left", ShowLabel = false)]
        [PropertyTooltip("$Tooltip")]
        [HideLabel]
        [SerializeField]
        protected IntReferenceTypes ReferenceTypeSelector;
        
        [HorizontalGroup("Top/Split", LabelWidth = 0.001f, Width = .7f)]
        [BoxGroup("Top/Split/Right", ShowLabel = false)]
        [HideLabel]
        [ShowIf("UseConstant")]
        [SerializeField]
        protected int ConstantValue;
        
        
        #region Reference values

        private bool _showFloatVariable => ReferenceTypeSelector == IntReferenceTypes.FloatVariable;
        [BoxGroup("Top/Split/Right", ShowLabel = false)]
        [HideLabel]
        [ShowIf("@!UseConstant && _showFloatVariable")]
        [InlineEditor()]
        [SerializeField]
        protected FloatVariable ReferenceValue_FloatVariable;
        
        [HideLabel]
        [ShowIf("@!UseConstant && _showFloatVariable")]
        [SerializeField]
        protected IntRoundingBehavior RoundingBehavior_FloatVariable;
        
        
        private bool _showIntVariable => ReferenceTypeSelector == IntReferenceTypes.IntVariable;
        [BoxGroup("Top/Split/Right", ShowLabel = false)]
        [HideLabel]
        [ShowIf("@!UseConstant && _showIntVariable")]
        [InlineEditor()]
        [SerializeField]
        protected IntVariable ReferenceValue_IntVariable;
        
        
        private bool _showBoolVariable => ReferenceTypeSelector == IntReferenceTypes.BoolVariable;
        [BoxGroup("Top/Split/Right", ShowLabel = false)]
        [HideLabel]
        [ShowIf("@!UseConstant && _showBoolVariable")]
        [InlineEditor()]
        [SerializeField]
        protected BoolVariable ReferenceValue_BoolVariable;
        
        
        
        
        #endregion
        
        #endregion
        
        #region Interface

        public int Value
        {
            get
            {
                if (UseConstant) 
                    return ConstantValue;
                switch (ReferenceTypeSelector)
                {
                    case IntReferenceTypes.FloatVariable:
                        if (ReferenceValue_FloatVariable == null)
                        {
                            return 0;
                        }
                        switch (RoundingBehavior_FloatVariable)
                        {
                            default:
                            case IntRoundingBehavior.Truncate:
                                return (int) Math.Truncate(ReferenceValue_FloatVariable.Value);
                            case IntRoundingBehavior.Round:
                                return Mathf.RoundToInt(ReferenceValue_FloatVariable.Value);
                        }
                    case IntReferenceTypes.IntVariable:
                        return ReferenceValue_IntVariable?.Value ?? 0;
                    case IntReferenceTypes.BoolVariable:
                        return ReferenceValue_BoolVariable.Value ? 1 : 0;
                    default:
                        Debug.LogError($"Trying to access Int variable but none set in inspector!");
                        return default(int);
                }
            }
            set
            {
                if (UseConstant)
                {
                    ConstantValue = value;
                    return;
                }
                switch (ReferenceTypeSelector)
                {
                    case IntReferenceTypes.FloatVariable:
                        if (ReferenceValue_FloatVariable != null)
                            ReferenceValue_FloatVariable.Value = value;
                        break;
                    case IntReferenceTypes.IntVariable:
                        if (ReferenceValue_FloatVariable != null)
                            ReferenceValue_FloatVariable.Value = Mathf.FloorToInt(value);
                        break;
                    case IntReferenceTypes.BoolVariable:
                        if (ReferenceValue_BoolVariable != null)
                            ReferenceValue_BoolVariable.Value = value != 0;
                        break;
                    default:
                        Debug.LogError($"Trying to access Int variable but none set in inspector!");
                        break;
                }
            }
        }

        public String Name
        {
            get
            {
                if (UseConstant)
                    return $"{ConstantValue}";
                switch (ReferenceTypeSelector)
                {
                    case IntReferenceTypes.FloatVariable:
                        return ReferenceValue_FloatVariable?.GetName();
                    case IntReferenceTypes.IntVariable:
                        return ReferenceValue_IntVariable?.GetName();
                    case IntReferenceTypes.BoolVariable:
                        return ReferenceValue_BoolVariable?.GetName();
                }
                return "<Missing Float>";
            }
        }
        
        public String Description
        {
            get
            {
                if (UseConstant)
                    return $"{ConstantValue}";
                switch (ReferenceTypeSelector)
                {
                    case IntReferenceTypes.FloatVariable:
                        return ReferenceValue_FloatVariable?.GetDescription();
                    case IntReferenceTypes.IntVariable:
                        return ReferenceValue_IntVariable?.GetDescription();
                    case IntReferenceTypes.BoolVariable:
                        return ReferenceValue_BoolVariable?.GetDescription();
                }
                return "<Missing Float>";
            }
        }

        public void Subscribe(OnUpdate callback)
        {
            switch (ReferenceTypeSelector)
            {
                case IntReferenceTypes.FloatVariable:
                    ReferenceValue_FloatVariable?.Subscribe(callback);
                    break;
                case IntReferenceTypes.IntVariable:
                    ReferenceValue_IntVariable?.Subscribe(callback);
                    break;
                case IntReferenceTypes.BoolVariable:
                    ReferenceValue_BoolVariable?.Subscribe(callback);
                    break;
            }
        }

        public void Unsubscribe(OnUpdate callback)
        {
            switch (ReferenceTypeSelector)
            {
                case IntReferenceTypes.FloatVariable:
                    ReferenceValue_FloatVariable?.Unsubscribe(callback);
                    break;
                case IntReferenceTypes.IntVariable:
                    ReferenceValue_IntVariable?.Unsubscribe(callback);
                    break;
                case IntReferenceTypes.BoolVariable:
                    ReferenceValue_BoolVariable?.Unsubscribe(callback);
                    break;
            }
        }

        #endregion
    }
}