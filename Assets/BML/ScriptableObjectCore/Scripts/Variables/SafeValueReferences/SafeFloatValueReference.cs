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
    public class SafeFloatValueReference
    {
        #region Inspector
        
        public String Tooltip => !UseConstant ? this.Description : "";

        protected bool UseConstant => ReferenceTypeSelector == FloatReferenceTypes.Constant;
        
        public enum FloatReferenceTypes {
            Constant = 0,
            FloatVariable = 1,
            IntVariable = 2,
            EvaluateCurveVariable = 3,
        }
        
        [VerticalGroup("Top")]
        [HorizontalGroup("Top/Split", LabelWidth = 0.001f)]
        [HorizontalGroup("Top/Split/Left", LabelWidth = .01f)]
        [BoxGroup("Top/Split/Left/Left", ShowLabel = false)]
        [PropertyTooltip("$Tooltip")]
        [HideLabel]
        [SerializeField]
        protected FloatReferenceTypes ReferenceTypeSelector;

        [HorizontalGroup("Top/Split", LabelWidth = 0.001f, Width = .7f)]
        [BoxGroup("Top/Split/Right", ShowLabel = false)]
        [HideLabel]
        [ShowIf("UseConstant")]
        [SerializeField]
        protected float ConstantValue;
        
        
        #region Reference values
        
        private bool _showFloatVariable => ReferenceTypeSelector == FloatReferenceTypes.FloatVariable;
        [BoxGroup("Top/Split/Right", ShowLabel = false)]
        [HideLabel]
        [ShowIf("@!UseConstant && _showFloatVariable")]
        [InlineEditor()]
        [SerializeField]
        protected FloatVariable ReferenceValue_FloatVariable;
        
        private bool _showIntVariable => ReferenceTypeSelector == FloatReferenceTypes.IntVariable;
        [BoxGroup("Top/Split/Right", ShowLabel = false)]
        [HideLabel]
        [ShowIf("@!UseConstant && _showIntVariable")]
        [InlineEditor()]
        [SerializeField]
        protected IntVariable ReferenceValue_IntVariable;
        
        private bool _showEvaluateCurveVariable => ReferenceTypeSelector == FloatReferenceTypes.EvaluateCurveVariable;
        [BoxGroup("Top/Split/Right", ShowLabel = false)]
        [HideLabel]
        [ShowIf("@!UseConstant && _showEvaluateCurveVariable")]
        [InlineEditor()]
        [SerializeField]
        protected EvaluateCurveVariable ReferenceValue_EvaluateCurveVariable;
        
        
        
        #endregion
        
        #endregion
        
        #region Interface

        public float Value
        {
            get
            {
                if (UseConstant) 
                    return ConstantValue;
                switch (ReferenceTypeSelector)
                {
                    case FloatReferenceTypes.FloatVariable:
                        return ReferenceValue_FloatVariable?.Value ?? 0;
                    case FloatReferenceTypes.IntVariable:
                        return ReferenceValue_IntVariable?.Value ?? 0;
                    case FloatReferenceTypes.EvaluateCurveVariable:
                        return ReferenceValue_EvaluateCurveVariable?.Value ?? 0;
                    default:
                        Debug.LogError($"Trying to access Float variable but none set in inspector!");
                        return default(float);
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
                    case FloatReferenceTypes.FloatVariable:
                        if (ReferenceValue_FloatVariable != null)
                            ReferenceValue_FloatVariable.Value = value;
                        break;
                    case FloatReferenceTypes.IntVariable:
                        if (ReferenceValue_FloatVariable != null)
                            ReferenceValue_FloatVariable.Value = Mathf.FloorToInt(value);
                        break;
                    case FloatReferenceTypes.EvaluateCurveVariable:
                        // if (ReferenceValue_EvaluateCurveVariable != null)
                        //     ReferenceValue_EvaluateCurveVariable.Value = value;
                        break;
                    default:
                        Debug.LogError($"Trying to access Float variable but none set in inspector!");
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
                    case FloatReferenceTypes.FloatVariable:
                        return ReferenceValue_FloatVariable?.GetName();
                    case FloatReferenceTypes.IntVariable:
                        return ReferenceValue_IntVariable?.GetName();
                    case FloatReferenceTypes.EvaluateCurveVariable:
                        return ReferenceValue_EvaluateCurveVariable?.GetName();
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
                    case FloatReferenceTypes.FloatVariable:
                        return ReferenceValue_FloatVariable?.GetDescription();
                    case FloatReferenceTypes.IntVariable:
                        return ReferenceValue_IntVariable?.GetDescription();
                    case FloatReferenceTypes.EvaluateCurveVariable:
                        return ReferenceValue_EvaluateCurveVariable?.GetDescription();
                }
                return "<Missing Float>";
            }
        }

        public void Subscribe(OnUpdate callback)
        {
            switch (ReferenceTypeSelector)
            {
                case FloatReferenceTypes.FloatVariable:
                    ReferenceValue_FloatVariable?.Subscribe(callback);
                    break;
                case FloatReferenceTypes.IntVariable:
                    ReferenceValue_IntVariable?.Subscribe(callback);
                    break;
                case FloatReferenceTypes.EvaluateCurveVariable:
                    ReferenceValue_EvaluateCurveVariable?.Subscribe(callback);
                    break;
            }
        }

        public void Unsubscribe(OnUpdate callback)
        {
            switch (ReferenceTypeSelector)
            {
                case FloatReferenceTypes.FloatVariable:
                    ReferenceValue_FloatVariable?.Unsubscribe(callback);
                    break;
                case FloatReferenceTypes.IntVariable:
                    ReferenceValue_IntVariable?.Unsubscribe(callback);
                    break;
                case FloatReferenceTypes.EvaluateCurveVariable:
                    ReferenceValue_EvaluateCurveVariable?.Unsubscribe(callback);
                    break;
            }
        }

        #endregion
    }
}