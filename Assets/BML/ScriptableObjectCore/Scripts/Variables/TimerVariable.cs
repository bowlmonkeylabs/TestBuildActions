using System;
using System.Collections;
using System.Collections.Generic;
using BML.ScriptableObjectCore.Scripts.CustomAttributes;
using BML.ScriptableObjectCore.Scripts.Variables.SafeValueReferences;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace BML.ScriptableObjectCore.Scripts.Variables
{
    [Required]
    [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    [CreateAssetMenu(fileName = "TimerVariable", menuName = "BML/Variables/TimerVariable", order = 0)]
    public class TimerVariable : ScriptableObject
    {
        #region Inspector
        [HideInInlineEditors] public bool EnableDebugOnUpdate;
        
        [TextArea (7, 10)] [HideInInlineEditors] public String _description;
        
        [SerializeField] private SafeFloatValueReference _duration;

        #endregion
        
        #region Public properties
        
        public float Duration
        {
            get => _duration.Value;
            set => _duration.Value = value;
        }
        
        [ShowInInspector, ReadOnly] public float? RemainingTime => _remainingTime;
        [ShowInInspector, ReadOnly] public float ElapsedTime => Duration - (_remainingTime ?? Duration);
        [ShowInInspector, ReadOnly] public bool IsFinished => _isFinished;
        [ShowInInspector, ReadOnly] public bool IsStopped => _isStopped;
        [ShowInInspector, ReadOnly] public bool IsStarted => _isStarted;

        #endregion

        #region Timer state
        
        [NonSerialized] private float? _remainingTime = null;
        [NonSerialized] private bool _isFinished = false;
        [NonSerialized] private float _startTime = Mathf.NegativeInfinity;
        [NonSerialized] private float _lastUpdateTime = Mathf.NegativeInfinity;
        [NonSerialized] private bool _isStopped = true;
        [NonSerialized] private bool _isStarted = false;

        #endregion
        
        #region Public methods

        public void StartTimer()
        {
            if(EnableDebugOnUpdate) Debug.LogError($"{name} - Start");
            _isStarted = true;
            _isStopped = false;
            _isFinished = false;
            _startTime = Time.time;
            _lastUpdateTime = _startTime;
            _remainingTime = Duration;
        }

        public void RestartTimer()
        {
            if(EnableDebugOnUpdate) Debug.LogError($"{name} - Restart");
            StartTimer();
        }

        public void ResetTimer()
        {
            if(EnableDebugOnUpdate) Debug.LogError($"{name} - Reset");
            _isStarted = false;
            _isStopped = true;
            _isFinished = false;
            _startTime = Time.time;
            _lastUpdateTime = _startTime;
            _remainingTime = Duration;
            
        }

        public void StopTimer()
        {
            if(EnableDebugOnUpdate) Debug.LogError($"{name} - Stop");
            _isStopped = true;
        }

        public void UpdateTime(float multiplier = 1f)
        {
            if (!_isStopped && !_isFinished)
            {
                var updateTime = Time.time;
                var deltaTime = (updateTime - _lastUpdateTime);
                _lastUpdateTime = updateTime;
                
                _remainingTime -= deltaTime * multiplier;
                _remainingTime = Mathf.Max(0f, _remainingTime.Value);

                OnUpdate?.Invoke();
                if (_remainingTime == 0)
                {
                    if(EnableDebugOnUpdate) Debug.LogError($"{name} - Finish");
                    _isFinished = true;
                    OnFinished?.Invoke();
                }
            }
            
        }
        
        #endregion

        #region Events
        
        public delegate void OnUpdate_();
        public event OnUpdate_ OnUpdate;
        public delegate void OnFinished_();
        public event OnFinished_ OnFinished;

        public void Subscribe(OnUpdate_ callback)
        {
            this.OnUpdate += callback;
        }

        public void Unsubscribe(OnUpdate_ callback)
        {
            this.OnUpdate -= callback;
        }
        
        public void SubscribeFinished(OnFinished_ callback)
        {
            this.OnFinished += callback;
        }

        public void UnsubscribeFinished(OnFinished_ callback)
        {
            this.OnFinished -= callback;
        }

        #endregion
        
    }

    [Serializable]
    [InlineProperty]
    [SynchronizedHeader]
    public class TimerReference
    {
        #region Inspector
        
        [HorizontalGroup("Split", LabelWidth = .01f, Width = .2f)] [PropertyTooltip("$Tooltip")]
        [BoxGroup("Split/Left", ShowLabel = false)] [LabelText("$LabelText")] [LabelWidth(10f)]
        [SerializeField] private bool _useConstant = false;
        
        [BoxGroup("Split/Right", ShowLabel = false)] [HideLabel] [ShowIf("_useConstant")]
        [SerializeField] private SafeFloatValueReference _constantDuration;
        
        [SerializeField] [ShowIf("_useConstant")] [DisableIf("AlwaysTrue")]
        private float? _constantRemainingTime = null;
        
        [BoxGroup("Split/Right", ShowLabel = false)] [HideLabel] [HideIf("_useConstant")] 
        [SerializeField] private TimerVariable _variable;

        #endregion
        
        public String Tooltip => _variable != null && !_useConstant ? _variable._description : "";
        public String LabelText => _useConstant ? "" : "?";

        public String Name
        {
            get
            {
                if (_useConstant) 
                    return $"<Const>{_constantRemainingTime}";
                    
                return (_variable != null) ? _variable.name : "<Missing Timer>";
            }
        }

        #region Constant timer state

        private bool AlwaysTrue => true;
        
        private float constantStartTime = Mathf.NegativeInfinity;
        private float constantLastUpdateTime = Mathf.NegativeInfinity;
        private bool isConstantStarted = true;
        private bool isConstantStopped = true;
        private bool isConstantFinished = false;

        #endregion

        #region Timer state

        public float Duration
        {
            get
            {
                if (_useConstant)
                    return _constantDuration.Value;
                if (_variable != null)
                    return _variable.Duration;
                
                Debug.LogError("Trying to access duration for timer variable that is not set!");
                return Mathf.Infinity;
            }
            set
            {
                if (_useConstant)
                {
                    _constantDuration.Value = value;
                    return;
                }
                if (_variable != null)
                {
                    _variable.Duration = value;
                    return;
                }
                
                Debug.LogError("Trying to access duration for timer variable that is not set!");
            }
        }

        public float? RemainingTime
        {
            get
            {
                if (_useConstant)
                    return _constantRemainingTime;
                if (_variable != null)
                    return _variable.RemainingTime;
                
                Debug.LogError("Trying to access remaining time for timer variable that is not set!");
                return Mathf.Infinity;
            }
        }
        
        public float ElapsedTime
        {
            get
            {
                if (_useConstant)
                    return _constantDuration.Value - (_constantRemainingTime ?? _constantDuration.Value);
                if (_variable != null)
                    return _variable.ElapsedTime;
                
                Debug.LogError("Trying to access elapsed time for timer variable that is not set!");
                return Mathf.Infinity;
            }
        }
        
        public bool IsStarted => (_useConstant) ? isConstantStarted : _variable.IsStarted;
        
        public bool IsStopped => (_useConstant) ? isConstantStopped : _variable.IsStopped;
        
        public bool IsFinished
        {
            get
            {
                if (_useConstant)
                    return isConstantFinished;
                if (_variable != null)
                    return _variable.IsFinished;
                
                Debug.LogError("Trying to access IsFinished for timer variable that is not set!");
                return false;
            }
        } 

        #endregion

        #region Public methods

        public void RestartTimer()
        {
            if (_useConstant)
            {
                isConstantStarted = true;
                isConstantStopped = false;
                isConstantFinished = false;
                constantStartTime = Time.time;
                constantLastUpdateTime = constantStartTime;
                _constantRemainingTime = Duration;
            }
            else
            {
                _variable?.RestartTimer();
            }
        }

        public void ResetTimer()
        {
            if (_useConstant)
            {
                isConstantStarted = false;
                isConstantStopped = true;
                isConstantFinished = false;
                constantStartTime = Time.time;
                constantLastUpdateTime = constantStartTime;
                _constantRemainingTime = Duration;
            }
            else
            {
                _variable?.ResetTimer();
            }
        }

        public void StopTimer()
        {
            if (_useConstant)
            {
                isConstantStopped = true;
            }
            else
            {
                _variable?.StopTimer();
            }
        }

        public void UpdateTime(float multiplier = 1f)
        {
            if (_useConstant)
            {
                var updateTime = Time.time;
                var deltaTime = (updateTime - constantLastUpdateTime);
                constantLastUpdateTime = updateTime;

                if (!isConstantStopped && !isConstantFinished)
                {
                    _constantRemainingTime -= deltaTime * multiplier;
                    _constantRemainingTime = Mathf.Max(0f, _constantRemainingTime.Value);
                
                    if (_constantRemainingTime == 0)
                    {
                        isConstantFinished = true;
                    }
                }
            }
            else
            {
                _variable?.UpdateTime(multiplier);
            }
        }

        #endregion

        #region Events

        public void Subscribe(TimerVariable.OnUpdate_ callback)
        {
            _variable?.Subscribe(callback);
        }

        public void Unsubscribe(TimerVariable.OnUpdate_ callback)
        {
            _variable?.Unsubscribe(callback);
        }
        
        public void SubscribeFinished(TimerVariable.OnFinished_ callback)
        {
            _variable?.SubscribeFinished(callback);
        }

        public void UnsubscribeFinished(TimerVariable.OnFinished_ callback)
        {
            _variable?.UnsubscribeFinished(callback);
        }

        #endregion
    }
}
