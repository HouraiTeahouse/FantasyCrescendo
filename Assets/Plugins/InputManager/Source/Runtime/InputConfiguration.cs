using System;
using System.Collections.Generic;

namespace TeamUtility.IO {

    [Serializable]
    public sealed class InputConfiguration {

        public List<AxisConfiguration> axes;
        public bool isExpanded;
        public string name;

        public InputConfiguration() :
            this("New Configuration") {}

        public InputConfiguration(string name) {
            axes = new List<AxisConfiguration>();
            this.name = name;
            isExpanded = false;
        }

        public static InputConfiguration Duplicate(InputConfiguration source) {
            var inputConfig = new InputConfiguration();
            inputConfig.name = source.name;

            inputConfig.axes = new List<AxisConfiguration>(source.axes.Count);
            for (var i = 0; i < source.axes.Count; i++)
                inputConfig.axes.Add(AxisConfiguration.Duplicate(source.axes[i]));

            return inputConfig;
        }

    }

}