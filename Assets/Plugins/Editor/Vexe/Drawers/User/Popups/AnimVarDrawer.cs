using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Xml.Xsl;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
    public class AnimVarDrawer : AttributeDrawer<AnimVarAttribute> {

        protected AnimatorControllerParameter[] Parameters;
        private AnimatorControllerParameter _currentParameter;

        private ParameterType _mask;

        private Animator _animator;
        protected Animator animator {
            get {
                if (_animator == null) {
                    string getterMethod = attribute.GetAnimatorMethod;
                    if (getterMethod.IsNullOrEmpty())
                        _animator = gameObject.GetComponent<Animator>();
                    else
                        _animator = targetType.GetMethod(getterMethod, Flags.InstanceAnyVisibility)
                                              .Invoke(rawTarget, null) as Animator;
                }
                return _animator;
            }
        }

        public bool IsValid(AnimatorControllerParameter parameter) {
            if (_mask == ParameterType.All)
                return true;
            
            switch (parameter.type) {
                case AnimatorControllerParameterType.Float:
                    return (_mask & ParameterType.Float) != 0;
                case AnimatorControllerParameterType.Int:
                    return (_mask & ParameterType.Int) != 0;
                case AnimatorControllerParameterType.Bool:
                    return (_mask & ParameterType.Bool) != 0;
                case AnimatorControllerParameterType.Trigger:
                    return (_mask & ParameterType.Trigger) != 0;
                default:
                    return false;
            }
        }

        public virtual AnimatorControllerParameter Current {
            get {
                if (!Ready)
                    return _currentParameter = null;
                return _currentParameter;
            }
            set {
                _current = Parameters.IndexOfZeroIfNotFound(value);
                _currentParameter = (!Ready || Parameters.IsEmpty()) ? null : Parameters[_current];
                if(memberType.IsA<string>())
                    member.Value = _currentParameter == null ? "" : _currentParameter.name;
                else if (memberType.IsA<int>())
                    member.Value = _currentParameter == null ? 0 : _currentParameter.nameHash;
                else //Assuming it is a AnimatorControllerParameter
                    member.Value = _currentParameter;
            }
        }

        private string[] _names;
        private int _current;

        protected bool Ready {
            get { return animator != null && animator.runtimeAnimatorController != null; }
        }

        protected override void Initialize() {
            _mask = attribute.Filter;
            FetchVariables();
        }

        private void FetchVariables() {
            if (!Ready)
                return;

            Parameters = animator.parameters.Where(param => IsValid(param)).ToArray();
            _names = Parameters.Select(param => param.name).ToArray();

            if (Parameters.IsEmpty())
                _names = new[] {"N/A"};
            else {
                // Try to find a match
                int index = -1;
                if (memberType.IsA<int>()) {
                    index = Parameters.Select(param => param.nameHash).ToArray().IndexOf((int)member.Value);
                } else if (memberType.IsA<string>()) {
                    index = _names.IndexOf(member.Value as string);
                } else { // Assuming it is a AnimationControllerParameter
                    index = Parameters.IndexOf(member.Value as AnimatorControllerParameter);
                }
                if (!attribute.AutoMatch.IsNullOrEmpty() && index < 0) {
                    string match = displayText.Remove(displayText.IndexOf(attribute.AutoMatch));
                    match = Regex.Replace(match, @"\s+", "");
                    index = _names.IndexOf(match);
                }

                Current = index >= 0 ? Parameters[index] : Parameters[0];
            }
        }

        public override void OnGUI() {
            if (!Ready) {
                if (memberType.IsA<string>())
                    member.Value = gui.Text(displayText, (string) member.Value);
                else {
                    using (gui.Horizontal()) {
                        gui.Label(displayText);
                        gui.Label("N/A: Please Attach Animator with Controller");
                    }
                }
            } else {
                if(Parameters.IsNullOrEmpty())
                    FetchVariables();

                var selection = gui.Popup(displayText, _current, _names);
                {
                    if (Parameters.IsEmpty())
                        Current = null;
                    else if (_current != selection || Current != Parameters[selection])
                        Current = Parameters[_current = selection];
                }
            }
        }

        public override bool CanHandle(Type memberType) {
            return memberType.IsA<string>() || memberType.IsA<int>() || memberType.IsA<AnimatorControllerParameter>();
        }

    }
}