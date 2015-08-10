using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UnityTest {

    [Serializable]
    public class PropertyResolver {

        public PropertyResolver() {
            ExcludedFieldNames = new string[] {};
            ExcludedTypes = new Type[] {};
            AllowedTypes = new Type[] {};
        }

        public string[] ExcludedFieldNames { get; set; }
        public Type[] ExcludedTypes { get; set; }
        public Type[] AllowedTypes { get; set; }

        public IList<string> GetFieldsAndPropertiesUnderPath(GameObject go, string propertPath) {
            propertPath = propertPath.Trim();
            if (!PropertyPathIsValid(propertPath))
                throw new ArgumentException("Incorrect property path: " + propertPath);

            int idx = propertPath.LastIndexOf('.');

            if (idx < 0) {
                IList<string> components = GetFieldsAndPropertiesFromGameObject(go, 2, null);
                return components;
            }

            string propertyToSearch = propertPath;
            Type type;
            if (MemberResolver.TryGetMemberType(go, propertyToSearch, out type))
                idx = propertPath.Length - 1;
            else {
                propertyToSearch = propertPath.Substring(0, idx);
                if (!MemberResolver.TryGetMemberType(go, propertyToSearch, out type)) {
                    IList<string> components = GetFieldsAndPropertiesFromGameObject(go, 2, null);
                    return components.Where(s => s.StartsWith(propertPath.Substring(idx + 1))).ToArray();
                }
            }

            List<string> resultList = new List<string>();
            var path = "";
            if (propertyToSearch.EndsWith("."))
                propertyToSearch = propertyToSearch.Substring(0, propertyToSearch.Length - 1);
            foreach (char c in propertyToSearch) {
                if (c == '.')
                    resultList.Add(path);
                path += c;
            }
            resultList.Add(path);
            foreach (PropertyInfo prop in type.GetProperties().Where(info => info.GetIndexParameters().Length == 0)) {
                if (prop.Name.StartsWith(propertPath.Substring(idx + 1)))
                    resultList.Add(propertyToSearch + "." + prop.Name);
            }
            foreach (FieldInfo prop in type.GetFields()) {
                if (prop.Name.StartsWith(propertPath.Substring(idx + 1)))
                    resultList.Add(propertyToSearch + "." + prop.Name);
            }
            return resultList.ToArray();
        }

        internal bool PropertyPathIsValid(string propertPath) {
            if (propertPath.StartsWith("."))
                return false;
            if (propertPath.IndexOf("..") >= 0)
                return false;
            if (Regex.IsMatch(propertPath, @"\s"))
                return false;
            return true;
        }

        public IList<string> GetFieldsAndPropertiesFromGameObject(GameObject gameObject,
                                                                  int depthOfSearch,
                                                                  string extendPath) {
            if (depthOfSearch < 1)
                throw new ArgumentOutOfRangeException("depthOfSearch has to be greater than 0");

            IEnumerable<string> goVals = GetPropertiesAndFieldsFromType(typeof (GameObject),
                                                                        depthOfSearch - 1)
                .Select(s => "gameObject." + s);

            List<string> result = new List<string>();
            if (AllowedTypes == null || !AllowedTypes.Any() || AllowedTypes.Contains(typeof (GameObject)))
                result.Add("gameObject");
            result.AddRange(goVals);

            foreach (Type componentType in GetAllComponents(gameObject)) {
                if (AllowedTypes == null || !AllowedTypes.Any() ||
                    AllowedTypes.Any(t => t.IsAssignableFrom(componentType)))
                    result.Add(componentType.Name);

                if (depthOfSearch > 1) {
                    string[] vals = GetPropertiesAndFieldsFromType(componentType, depthOfSearch - 1);
                    IEnumerable<string> valsFullName = vals.Select(s => componentType.Name + "." + s);
                    result.AddRange(valsFullName);
                }
            }

            if (!string.IsNullOrEmpty(extendPath)) {
                var memberResolver = new MemberResolver(gameObject, extendPath);
                Type pathType = memberResolver.GetMemberType();
                string[] vals = GetPropertiesAndFieldsFromType(pathType, depthOfSearch - 1);
                IEnumerable<string> valsFullName = vals.Select(s => extendPath + "." + s);
                result.AddRange(valsFullName);
            }

            return result;
        }

        private string[] GetPropertiesAndFieldsFromType(Type type, int level) {
            level--;

            List<string> result = new List<string>();
            List<MemberInfo> fields = new List<MemberInfo>();
            fields.AddRange(type.GetFields().Where(f => !Attribute.IsDefined(f, typeof (ObsoleteAttribute))).ToArray());
            fields.AddRange(
                            type.GetProperties()
                                .Where(
                                       info =>
                                       info.GetIndexParameters().Length == 0 &&
                                       !Attribute.IsDefined(info, typeof (ObsoleteAttribute)))
                                .ToArray());

            foreach (MemberInfo member in fields) {
                Type memberType = GetMemberFieldType(member);
                string memberTypeName = memberType.Name;

                if (AllowedTypes == null
                    || !AllowedTypes.Any()
                    ||
                    (AllowedTypes.Any(t => t.IsAssignableFrom(memberType)) &&
                     !ExcludedFieldNames.Contains(memberTypeName)))
                    result.Add(member.Name);

                if (level > 0 && IsTypeOrNameNotExcluded(memberType, memberTypeName)) {
                    string[] vals = GetPropertiesAndFieldsFromType(memberType, level);
                    IEnumerable<string> valsFullName = vals.Select(s => member.Name + "." + s);
                    result.AddRange(valsFullName);
                }
            }
            return result.ToArray();
        }

        private Type GetMemberFieldType(MemberInfo info) {
            if (info.MemberType == MemberTypes.Property)
                return (info as PropertyInfo).PropertyType;
            if (info.MemberType == MemberTypes.Field)
                return (info as FieldInfo).FieldType;
            throw new Exception("Only properties and fields are allowed");
        }

        internal Type[] GetAllComponents(GameObject gameObject) {
            List<Type> result = new List<Type>();
            Component[] components = gameObject.GetComponents(typeof (Component));
            foreach (Component component in components) {
                Type componentType = component.GetType();
                if (IsTypeOrNameNotExcluded(componentType, null))
                    result.Add(componentType);
            }
            return result.ToArray();
        }

        private bool IsTypeOrNameNotExcluded(Type memberType, string memberTypeName) {
            return !ExcludedTypes.Contains(memberType) && !ExcludedFieldNames.Contains(memberTypeName);
        }

    }

}