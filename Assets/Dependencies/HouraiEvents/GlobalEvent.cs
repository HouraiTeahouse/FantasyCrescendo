namespace Hourai.Events {

    /// <summary>
    /// Singleton version of the Mediator class.
    /// Use Instance to get the singleton object.
    /// </summary>
    public class GlobalMediator : Mediator {

        private static GlobalMediator _instance;

        /// <summary>
        /// Gets the singleton GlobalMediator instance.
        /// </summary>
        public static GlobalMediator Instance {
            get { return _instance ?? (_instance = new GlobalMediator()); }
        }

        /// <summary>
        /// Initializes a new GlobalMediator.
        /// Private to prevent external creation.
        /// </summary>
        GlobalMediator() {
        }

    }


}