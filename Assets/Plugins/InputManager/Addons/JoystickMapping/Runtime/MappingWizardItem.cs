namespace TeamUtility.IO {

    [System.Serializable]
    public class MappingWizardItem {

        private string _axisName;
        private MappingWizard.ScanType _scanType;

        public MappingWizardItem(string axisName, MappingWizard.ScanType scanType) {
            _axisName = axisName;
            _scanType = scanType;
        }

        public string AxisName {
            get { return _axisName; }
        }

        public MappingWizard.ScanType ScanType {
            get { return _scanType; }
        }

    }

}