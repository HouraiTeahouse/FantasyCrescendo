using System;
using System.Collections.Generic;

namespace Hourai.SmashBrew {

    public abstract class RestrictableCharacterComponent : CharacterComponent {

        private List<Func<bool>> _restrictions;

        private bool _restricted;

        public virtual bool Restricted {
            get {
                if (_restricted)
                    return true;
                for (var i = 0; i < _restrictions.Count; i++) {
                    if (!_restrictions[i]())
                        return true;
                }
                return false;
            }
            set { _restricted = value; }
        }

        public event Func<bool> Restrictions {
            add {
                if (_restrictions.Contains(value))
                    return;
                _restrictions.Add(value);
            }
            remove { _restrictions.Remove(value); }
        }

        void Awake() {
            _restrictions = new List<Func<bool>>();
        }

    }

}

