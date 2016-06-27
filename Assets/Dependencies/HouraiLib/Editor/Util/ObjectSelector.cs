using System;
using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace HouraiTeahouse.Editor {

    /// <summary>
    /// Wrapper around EditorGUI.Popup that allows for arbitrary selection of any object.
    /// </summary>
    /// <typeparam name="T">the type of object to be selecting.</typeparam>
    public class ObjectSelector<T> where T : Object {

        // The mapping from object value to GUIContent
        readonly Func<T, GUIContent> _contentFunc;

        /// <summary>
        /// Fired every time the selected object changes to a new value.
        /// </summary>
        public event Action<T> OnSelectedChange;

        /// <summary>
        /// Initializes an instance of ObjectSelector.
        /// </summary>
        public ObjectSelector() { _contentFunc = obj => new GUIContent(obj.name); }

        /// <summary>
        /// Initializes an instance of ObjectSelector.
        /// </summary>
        /// <param name="contentFunc">the mapping between object and GUIContent for use.</param>
        /// <exception cref="ArgumentNullException"><paramref name="contentFunc"/> is null</exception>
        public ObjectSelector(Func<T, GUIContent> contentFunc) {
            _contentFunc = Check.NotNull(contentFunc);
        }

        /// <summary>
        /// Initializes an instance of ObjectSelector.
        /// </summary>
        /// <param name="selections">the set of selections to begin with</param>
        public ObjectSelector(T[] selections) : this() {
            Selections = selections;
        }

        T[] _selections;
        int _index;
        T _selected;

        /// <summary>
        /// Gets or sets the available selectable items.
        /// </summary>
        public T[] Selections {
            get { return _selections; }
            set {
                _selections = value;
                if (IsValid) {
                    Content = Selections.Select(_contentFunc).ToArray();
                    _index = Array.IndexOf(Selections, Selected);
                }
                else {
                    Content = null;
                    Selected = null;
                    _index = -1;
                }
            }
        }

        /// <summary>
        /// Gets the selected item.
        /// </summary>
        public T Selected {
            get { return _selected; }
            private set {
                bool changed = value != Selected;
                _selected = value;
                if(OnSelectedChange != null && changed)
                    OnSelectedChange(value);
            }
        }

        /// <summary>
        /// Attempts to set the selected value. If <paramref name="obj"/> is not found, the selected value
        /// will not be changed
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>whether the object was successfully set</returns>
        public bool SetSelected(T obj) {
            if(!IsValid)
                throw new InvalidOperationException();
            int index = Array.IndexOf(_selections, obj);
            bool success = index >= 0;
            if (success) {
                SelectedIndex = index;
                Assert.AreEqual(Selected, obj);
            }
            return success;
        }

        /// <summary>
        /// Gets all of the generated GUIContent.
        /// </summary>
        public GUIContent[] Content { get; private set; }

        /// <summary>
        /// Gets or sets selected index.
        /// </summary>
        public int SelectedIndex {
            get { return _index; }
            set {
                if (IsValid && Check.Range(value, _selections)) {
                    _index = value;
                    Selected = _selections[_index];
                }
                else {
                    _index = -1;
                    Selected = null;
                }
            }
        }

        /// <summary>
        /// Checks if the current state of the selector is valid. 
        /// Will return true if there are valid selections to use and false otherwise.
        /// </summary>
        public bool IsValid {
            get { return Selections != null; }
        }

        /// <summary>
        /// Gets the GUIContent used for the currently selected. If nothing is currently selected
        /// or the selector is not currently valid, will return null.
        /// </summary>
        public GUIContent SelectedContent {
            get { return IsValid || Check.Range(SelectedIndex, Content) ? Content[SelectedIndex] : null; }
        }

        /// <summary>
        /// Draws a EditorGUI Popup to select one of the items in Selections
        /// </summary>
        public T Draw(GUIContent label, GUIStyle style = null, params GUILayoutOption[] options) {
            Check.NotNull(_selections);
            Check.NotNull(Content);
            SelectedIndex = EditorGUILayout.Popup(label,
                SelectedIndex,
                Content,
                style,
                options);
            return Selected;
        }
    }
}
