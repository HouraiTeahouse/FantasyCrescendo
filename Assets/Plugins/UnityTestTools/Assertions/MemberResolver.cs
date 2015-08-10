using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UnityTest {

    public class MemberResolver {

        private readonly GameObject m_GameObject;
        private readonly string m_Path;
        private object m_CallingObjectRef;
        private MemberInfo[] m_Callstack;

        public MemberResolver(GameObject gameObject, string path) {
            path = path.Trim();
            ValidatePath(path);

            m_GameObject = gameObject;
            m_Path = path.Trim();
        }

        public object GetValue(bool useCache) {
            if (useCache && m_CallingObjectRef != null) {
                object val = m_CallingObjectRef;
                for (var i = 0; i < m_Callstack.Length; i++)
                    val = GetValueFromMember(val, m_Callstack[i]);
                return val;
            }

            object result = GetBaseObject();
            MemberInfo[] fullCallStack = GetCallstack();

            m_CallingObjectRef = result;
            List<MemberInfo> tempCallstack = new List<MemberInfo>();
            for (var i = 0; i < fullCallStack.Length; i++) {
                MemberInfo member = fullCallStack[i];
                result = GetValueFromMember(result, member);
                tempCallstack.Add(member);
                if (result == null)
                    return null;
                Type type = result.GetType();

                //String is not a value type but we don't want to cache it
                if (!IsValueType(type) && type != typeof (string)) {
                    tempCallstack.Clear();
                    m_CallingObjectRef = result;
                }
            }
            m_Callstack = tempCallstack.ToArray();
            return result;
        }

        public Type GetMemberType() {
            MemberInfo[] callstack = GetCallstack();
            if (callstack.Length == 0)
                return GetBaseObject().GetType();

            MemberInfo member = callstack[callstack.Length - 1];
            if (member is FieldInfo)
                return (member as FieldInfo).FieldType;
            if (member is MethodInfo)
                return (member as MethodInfo).ReturnType;
            return null;
        }

        private object GetValueFromMember(object obj, MemberInfo memberInfo) {
            if (memberInfo is FieldInfo)
                return (memberInfo as FieldInfo).GetValue(obj);
            if (memberInfo is MethodInfo)
                return (memberInfo as MethodInfo).Invoke(obj, null);
            throw new InvalidPathException(memberInfo.Name);
        }

        private object GetBaseObject() {
            if (string.IsNullOrEmpty(m_Path))
                return m_GameObject;
            string firstElement = m_Path.Split('.')[0];
            Component comp = m_GameObject.GetComponent(firstElement);
            if (comp != null)
                return comp;
            return m_GameObject;
        }

        private MemberInfo[] GetCallstack() {
            if (m_Path == "")
                return new MemberInfo[0];
            Queue<string> propsQueue = new Queue<string>(m_Path.Split('.'));

            Type type = GetBaseObject().GetType();
            if (type != typeof (GameObject))
                propsQueue.Dequeue();

            PropertyInfo propertyTemp;
            FieldInfo fieldTemp;
            List<MemberInfo> list = new List<MemberInfo>();
            while (propsQueue.Count != 0) {
                string nameToFind = propsQueue.Dequeue();
                fieldTemp = GetField(type, nameToFind);
                if (fieldTemp != null) {
                    type = fieldTemp.FieldType;
                    list.Add(fieldTemp);
                    continue;
                }
                propertyTemp = GetProperty(type, nameToFind);
                if (propertyTemp != null) {
                    type = propertyTemp.PropertyType;
                    MethodInfo getMethod = GetGetMethod(propertyTemp);
                    list.Add(getMethod);
                    continue;
                }
                throw new InvalidPathException(nameToFind);
            }
            return list.ToArray();
        }

        private void ValidatePath(string path) {
            var invalid = false;
            if (path.StartsWith(".") || path.EndsWith("."))
                invalid = true;
            if (path.IndexOf("..") >= 0)
                invalid = true;
            if (Regex.IsMatch(path, @"\s"))
                invalid = true;

            if (invalid)
                throw new InvalidPathException(path);
        }

        private static bool IsValueType(Type type) {
#if !UNITY_METRO
            return type.IsValueType;
#else
            return false;
            #endif
        }

        private static FieldInfo GetField(Type type, string fieldName) {
#if !UNITY_METRO
            return type.GetField(fieldName);
#else
            return null;
            #endif
        }

        private static PropertyInfo GetProperty(Type type, string propertyName) {
#if !UNITY_METRO
            return type.GetProperty(propertyName);
#else
            return null;
            #endif
        }

        private static MethodInfo GetGetMethod(PropertyInfo propertyInfo) {
#if !UNITY_METRO
            return propertyInfo.GetGetMethod();
#else
            return null;
            #endif
        }

        #region Static wrappers

        public static bool TryGetMemberType(GameObject gameObject, string path, out Type value) {
            try {
                var mr = new MemberResolver(gameObject, path);
                value = mr.GetMemberType();
                return true;
            } catch (InvalidPathException) {
                value = null;
                return false;
            }
        }

        public static bool TryGetValue(GameObject gameObject, string path, out object value) {
            try {
                var mr = new MemberResolver(gameObject, path);
                value = mr.GetValue(false);
                return true;
            } catch (InvalidPathException) {
                value = null;
                return false;
            }
        }

        #endregion
    }

}