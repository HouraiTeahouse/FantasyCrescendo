namespace HouraiTeahouse {

    //TODO: Document
    public interface IRegistrar<in T> {

        void Register(T obj);
        bool Unregister(T obj);

    }

}