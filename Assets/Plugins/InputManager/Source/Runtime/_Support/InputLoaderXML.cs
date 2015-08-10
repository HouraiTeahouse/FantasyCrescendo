using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace TeamUtility.IO {

    public sealed class InputLoaderXML : IInputLoader {

        private string _filename;
        private Stream _inputStream;
        private TextReader _textReader;

        public InputLoaderXML(string filename) {
            if (filename == null)
                throw new ArgumentNullException("filename");

            _filename = filename;
            _inputStream = null;
            _textReader = null;
        }

        public InputLoaderXML(Stream stream) {
            if (stream == null)
                throw new ArgumentNullException("stream");

            _filename = null;
            _textReader = null;
            _inputStream = stream;
        }

        public InputLoaderXML(TextReader reader) {
            if (reader == null)
                throw new ArgumentNullException("reader");

            _filename = null;
            _inputStream = null;
            _textReader = reader;
        }

        public void Load(out List<InputConfiguration> inputConfigurations, out string defaultConfig) {
            using (XmlReader reader = CreateXmlReader()) {
                inputConfigurations = new List<InputConfiguration>();
                defaultConfig = string.Empty;
                while (reader.Read()) {
                    if (reader.IsStartElement("Input"))
                        defaultConfig = reader["defaultConfiguration"];
                    else if (reader.IsStartElement("InputConfiguration"))
                        inputConfigurations.Add(ReadInputConfiguration(reader));
                }
            }
        }

        public InputConfiguration LoadSelective(string inputConfigName) {
            InputConfiguration inputConfig = null;
            using (XmlReader reader = CreateXmlReader()) {
                while (reader.Read()) {
                    if (reader.IsStartElement("InputConfiguration") && reader["name"] == inputConfigName) {
                        inputConfig = ReadInputConfiguration(reader);
                        break;
                    }
                }
            }

            return inputConfig;
        }

        private XmlReader CreateXmlReader() {
            if (_filename != null)
                return XmlReader.Create(_filename);
            if (_inputStream != null)
                return XmlReader.Create(_inputStream);
            if (_textReader != null)
                return XmlReader.Create(_textReader);

            return null;
        }

        private InputConfiguration ReadInputConfiguration(XmlReader reader) {
            var inputConfig = new InputConfiguration();
            inputConfig.name = reader["name"];

            while (reader.Read()) {
                if (!reader.IsStartElement("AxisConfiguration"))
                    break;

                inputConfig.axes.Add(ReadAxisConfiguration(reader));
            }

            return inputConfig;
        }

        private AxisConfiguration ReadAxisConfiguration(XmlReader reader) {
            var axisConfig = new AxisConfiguration();
            axisConfig.name = reader["name"];

            var endOfAxis = false;
            while (reader.Read() && reader.IsStartElement() && !endOfAxis) {
                switch (reader.LocalName) {
                    case "description":
                        axisConfig.description = reader.IsEmptyElement
                                                     ? string.Empty
                                                     : reader.ReadElementContentAsString();
                        break;
                    case "positive":
                        axisConfig.positive = AxisConfiguration.StringToKey(reader.ReadElementContentAsString());
                        break;
                    case "altPositive":
                        axisConfig.altPositive = AxisConfiguration.StringToKey(reader.ReadElementContentAsString());
                        break;
                    case "negative":
                        axisConfig.negative = AxisConfiguration.StringToKey(reader.ReadElementContentAsString());
                        break;
                    case "altNegative":
                        axisConfig.altNegative = AxisConfiguration.StringToKey(reader.ReadElementContentAsString());
                        break;
                    case "deadZone":
                        axisConfig.deadZone = reader.ReadElementContentAsFloat();
                        break;
                    case "gravity":
                        axisConfig.gravity = reader.ReadElementContentAsFloat();
                        break;
                    case "sensitivity":
                        axisConfig.sensitivity = reader.ReadElementContentAsFloat();
                        break;
                    case "snap":
                        axisConfig.snap = reader.ReadElementContentAsBoolean();
                        break;
                    case "invert":
                        axisConfig.invert = reader.ReadElementContentAsBoolean();
                        break;
                    case "type":
                        axisConfig.type = AxisConfiguration.StringToInputType(reader.ReadElementContentAsString());
                        break;
                    case "axis":
                        axisConfig.axis = reader.ReadElementContentAsInt();
                        break;
                    case "joystick":
                        axisConfig.joystick = reader.ReadElementContentAsInt();
                        break;
                    default:
                        endOfAxis = true;
                        break;
                }
            }

            return axisConfig;
        }

    }

}