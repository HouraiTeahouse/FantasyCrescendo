using System.Collections.Generic;

namespace TeamUtility.IO {

    public interface IInputLoader {

        void Load(out List<InputConfiguration> inputConfigurations, out string defaultConfig);
        InputConfiguration LoadSelective(string inputConfigName);

    }

}