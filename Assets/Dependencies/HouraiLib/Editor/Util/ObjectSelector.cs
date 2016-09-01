using System;
using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEngine.Assertions;

namespace HouraiTeahouse.Editor {

    /// <summary>
    /// Wrapper around EditorGUI.Popup that allows for arbitrary selection of any object.
    /// </summary>
    /// <typeparam name="T">the type of object to be selecting.</typeparam>
    public class ObjectSelector<T> where T : class {

        // The mapping from object value to GUIContent
        readonly Func<T, GUIContent> _contentFunc;
        readonly Func<T, bool> _filter;

        /// <summary>
        /// Fired every time the selected object changes to a new value.
        /// First argument is old value, new argument is new value.
        /// </summary>
        public event Action<T, T> SelectionChanged;

        /// <summary>
        /// Initializes an instance of ObjectSelector.
        /// </summary>
        public ObjectSelector() : this(o => o.ToString()) {}

        /// <summary>
        /// Initializes an instance of ObjectSelector.
        /// </summary>
        /// <param name="contentFunc">the mapping between object and string for use.</param>
        /// <param name="filterFunc">a filter for the type of objects selectable</param>
        /// <exception cref="ArgumentNullException"><paramref name="contentFunc"/> is null</exception>
        public ObjectSelector(Func<T, string> contentFunc, Func<T, bool> filterFunc = null) 
            : this(t => new GUIContent(contentFunc(t)), filterFunc) {
        }

        /// <summary>
        /// Initializes an instance of ObjectSelector.
        /// </summary>
        /// <param name="contentFunc">the mapping between object and GUIContent for use.</param>
        /// <param name="filterFunc">a filter for the type of objects selectable</param>
        /// <exception cref="ArgumentNullException"><paramref name="contentFunc"/> is null</exception>
        public ObjectSelector(Func<T, GUIContent> contentFunc, Func<T, bool> filterFunc = null) {
            _contentFunc = Check.NotNull(contentFunc);
            _filter = filterFunc;
        }

        /// <summary>
        /// Initializes an instance of ObjectSelector.
        /// </summary>
        /// <param name="selections">the set of selections to begin with</param>
        public ObjectSelector(T[] selections) : this() {
            Selections = selections;
        }

        T[] _selections;
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
                }
                else {
                    Content = null;
                    Selected = default(T);
                }
            }
        }

        /// <summary>
        /// Gets the selected item.
        /// </summary>
        public T Selected {
            get { return _selected; }
            set {
                bool changed = _selected != value;
                var oldValue = Selected;
                _selected = value;
                if(changed)
                    SelectionChanged.SafeInvoke(oldValue, value);
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
                Assert.AreEqual(Selected, obj);
            }
            return success;
        }

        /// <summary>
        /// Gets all of the generated GUIContent.
        /// </summary>
        public GUIContent[] Content { get; private set; }

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
            get { return _contentFunc(Selected); }
        }

        /// <summary>
        /// Draws a EditorGUI Popup to select one of the items in Selections
        /// </summary>
        public T Draw(GUIContent label, GUIStyle style = null, params GUILayoutOption[] options) {
            Check.NotNull(_selections);
            Check.NotNull(Content);
            if (style == null)
                style = EditorStyles.popup;
            T[] selections = Selections;
            if (_filter != null)
                selections = selections.Where(_filter).ToArray();
            GUIContent[] content = selections.Select(_contentFunc).ToArray();
            int index = Array.IndexOf(selections, Selected);
            index = EditorGUILayout.Popup(label, index, content, style, options);
            Selected = Check.Range(index, selections) ? selections[index] : default(T);
            return Selected;
        }
    }
}
