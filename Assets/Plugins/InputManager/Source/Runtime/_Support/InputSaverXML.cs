using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace TeamUtility.IO {

    public sealed class InputSaverXML : IInputSaver {

        private string _filename;
        private StringBuilder _output;
        private Stream _outputStream;

        public InputSaverXML(string filename) {
            if (filename == null)
                throw new ArgumentNullException("filename");

            _filename = filename;
            _outputStream = null;
            _output = null;
        }

        public InputSaverXML(Stream stream) {
            if (stream == null)
                throw new ArgumentNullException("stream");

            _filename = null;
            _output = null;
            _outputStream = stream;
        }

        public InputSaverXML(StringBuilder output) {
            if (output == null)
                throw new ArgumentNullException("output");

            _filename = null;
            _outputStream = null;
            _output = output;
        }

        public void Save(List<InputConfiguration> inputConfigurations, string defaultConfiguration) {
            var settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;

            using (XmlWriter writer = CreateXmlWriter(settings)) {
                writer.WriteStartDocument(true);
                writer.WriteStartElement("Input");
                writer.WriteAttributeString("defaultConfiguration", defaultConfiguration);
                foreach (InputConfiguration inputConfig in inputConfigurations)
                    WriteInputConfiguration(inputConfig, writer);

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

#if UNITY_WINRT && !UNITY_EDITOR
			if(_filename != null && _outputStream != null && (_outputStream is MemoryStream))
			{
				UnityEngine.Windows.File.WriteAllBytes(_filename, ((MemoryStream)_outputStream).ToArray());
				_outputStream.Dispose();
			}
#endif
        }

        private XmlWriter CreateXmlWriter(XmlWriterSettings settings) {
            if (_filename != null) {
#if UNITY_WINRT && !UNITY_EDITOR
				_outputStream = new MemoryStream();
				return XmlWriter.Create(_outputStream, settings);
#else
                return XmlWriter.Create(_filename, settings);
#endif
            }
            if (_outputStream != null)
                return XmlWriter.Create(_outputStream, settings);
            if (_output != null)
                return XmlWriter.Create(_output, settings);

            return null;
        }

        private void WriteInputConfiguration(InputConfiguration inputConfig, XmlWriter writer) {
            writer.WriteStartElement("InputConfiguration");
            writer.WriteAttributeString("name", inputConfig.name);
            foreach (AxisConfiguration axisConfig in inputConfig.axes)
                WriteAxisConfiguration(axisConfig, writer);

            writer.WriteEndElement();
        }

        private void WriteAxisConfiguration(AxisConfiguration axisConfig, XmlWriter writer) {
            writer.WriteStartElement("AxisConfiguration");
            writer.WriteAttributeString("name", axisConfig.name);
            writer.WriteElementString("description", axisConfig.description);
            writer.WriteElementString("positive", axisConfig.positive.ToString());
            writer.WriteElementString("altPositive", axisConfig.altPositive.ToString());
            writer.WriteElementString("negative", axisConfig.negative.ToString());
            writer.WriteElementString("altNegative", axisConfig.altNegative.ToString());
            writer.WriteElementString("deadZone", axisConfig.deadZone.ToString());
            writer.WriteElementString("gravity", axisConfig.gravity.ToString());
            writer.WriteElementString("sensitivity", axisConfig.sensitivity.ToString());
            writer.WriteElementString("snap", axisConfig.snap.ToString().ToLower());
            writer.WriteElementString("invert", axisConfig.invert.ToString().ToLower());
            writer.WriteElementString("type", axisConfig.type.ToString());
            writer.WriteElementString("axis", axisConfig.axis.ToString());
            writer.WriteElementString("joystick", axisConfig.joystick.ToString());

            writer.WriteEndElement();
        }

    }

}