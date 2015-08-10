using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace TeamUtility.IO {

    [System.Serializable]
    public class JoystickMapping : IEnumerable<AxisMapping> {

        private List<AxisMapping> _axes;
        private string _name;

        public JoystickMapping() {
            _name = null;
            _axes = new List<AxisMapping>();
        }

        public string Name {
            get { return _name; }
        }

        public int AxisCount {
            get { return _axes.Count; }
        }

        public IEnumerator<AxisMapping> GetEnumerator() {
            return _axes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _axes.GetEnumerator();
        }

        public void Load(string filename) {
#if UNITY_WINRT && !UNITY_EDITOR
			if(UnityEngine.Windows.File.Exists(filename))
			{
				byte[] buffer = UnityEngine.Windows.File.ReadAllBytes(filename);
				string xmlData = System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length);
				if(!string.IsNullOrEmpty(xmlData))
				{
                    InternalLoad(xmlData);
                }
            }
#else
            if (File.Exists(filename)) {
                using (StreamReader reader = File.OpenText(filename))
                    InternalLoad(reader.ReadToEnd());
            }
#endif
        }

        public void LoadFromResources(string path) {
            var textAsset = Resources.Load<TextAsset>(path);
            if (textAsset != null) {
                InternalLoad(textAsset.text);
                Resources.UnloadAsset(textAsset);
            }
        }

        private void InternalLoad(string xmlData) {
            try {
                var doc = new XmlDocument();
                doc.LoadXml(xmlData);

                _name = doc.DocumentElement.Attributes["name"].InnerText;
                foreach (XmlNode axisNode in doc.DocumentElement) {
                    string name = axisNode.Attributes["name"].InnerText;
                    var key =
                        (KeyCode) System.Enum.Parse(typeof (KeyCode), axisNode.Attributes["key"].InnerText, true);
                    int joystickAxis = int.Parse(axisNode.Attributes["joystickAxis"].InnerText);
                    var scanType =
                        (MappingWizard.ScanType)
                        System.Enum.Parse(typeof (MappingWizard.ScanType),
                                          axisNode.Attributes["scanType"].InnerText,
                                          true);

                    if (scanType == MappingWizard.ScanType.Button)
                        _axes.Add(new AxisMapping(name, key));
                    else
                        _axes.Add(new AxisMapping(name, joystickAxis));
                }
            } catch {
                _name = null;
                _axes.Clear();
            }
        }

    }

}