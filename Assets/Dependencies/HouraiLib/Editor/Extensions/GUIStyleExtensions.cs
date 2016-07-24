using UnityEngine;
using System;
using System.Collections.Generic;

namespace HouraiTeahouse.Editor {

    public class EventListener {
        readonly Dictionary<EventType, Action<Event>> responses;

        public EventListener() {
            responses = new Dictionary<EventType, Action<Event>>();
        }

        public void EventCheck() {
            Event evt = Event.current;
            var type = evt.type;
            if (responses.ContainsKey(type))
                responses[type].SafeInvoke(evt);
        }

        public void EventCheck(Rect rect) {
            if (!rect.Contains(Event.current.mousePosition))
                return;
            EventCheck();
        }

        public EventListener AddListener(EventType type, Action<Event> action) {
            if (!responses.ContainsKey(type))
                responses[type] = action;
            else
                responses[type] += action;
            return this;
        }

        public EventListener AddListeners(IEnumerable<EventType> types, Action<Event> action) {
            foreach (var type in types.EmptyIfNull())
                AddListener(type, action);
            return this;
        }

        public EventListener RemoveListener(EventType type, Action<Event> action) {
            if (!responses.ContainsKey(type))
                return this;
            responses[type] -= action;
            if (responses[type] == null)
                responses.Remove(type);
            return this;
        }

        public EventListener RemoveListeners(IEnumerable<EventType> types, Action<Event> action) {
            foreach (var type in types.EmptyIfNull())
                RemoveListener(type, action);
            return this;
        }

        public void ClearType(EventType type) { responses.Remove(type); }

        public void Clear() {
            responses.Clear();
        }

        public event Action<Event> MouseDragged {
            add { AddListener(EventType.MouseDrag, value); }
            remove { RemoveListener(EventType.MouseDrag, value); }
        }

        public event Action<Event> MouseDown {
            add { AddListener(EventType.MouseDown, value); }
            remove { RemoveListener(EventType.MouseDown, value); }
        }

        public event Action<Event> DragPerform {
            add { AddListener(EventType.DragPerform, value); }
            remove { RemoveListener(EventType.DragPerform, value); }
        }

        public event Action<Event> DragUpdated {
            add { AddListener(EventType.DragUpdated, value); }
            remove { RemoveListener(EventType.DragUpdated, value); }
        }

        public event Action<Event> DragExited {
            add { AddListener(EventType.DragExited, value); }
            remove { RemoveListener(EventType.DragExited, value); }
        }

    }

    public static class GUIStyleExtensions {

        public static GUIStyle WithPadding(this GUIStyle style, RectOffset padding) {
            return new GUIStyle(Check.NotNull(style)) {
                padding = Check.NotNull(padding) 
            };
        }

        public static GUIStyle WithMargins(this GUIStyle style, RectOffset margin) {
            return new GUIStyle(Check.NotNull(style)) {
                margin = Check.NotNull(margin) 
            };
        }

        public static GUIStyle WithoutPadding(this GUIStyle style) {
            return style.WithPadding(new RectOffset());
        }

        public static GUIStyle WithoutMargins(this GUIStyle style) {
            return style.WithMargins(new RectOffset());
        }

        public static GUIStyle WithRichText(this GUIStyle style) {
            return new GUIStyle(Check.NotNull(style)) {
                richText = true
            };
        }

        public static GUIStyle WithoutRichText(this GUIStyle style) {
            return new GUIStyle(Check.NotNull(style)) {
                richText = false 
            };
        }

        public static GUIStyle WithAlignment(this GUIStyle style, TextAnchor alignment) {
            return new GUIStyle(Check.NotNull(style)) {
                alignment = alignment
            };
        }

    }

}

