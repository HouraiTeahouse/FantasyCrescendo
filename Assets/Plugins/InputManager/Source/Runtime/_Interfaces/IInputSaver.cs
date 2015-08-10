using System.Collections.Generic;

namespace TeamUtility.IO {

    public interface IInputSaver {

        void Save(List<InputConfiguration> inputConfigurations, string defaultConfiguration);

    }

}