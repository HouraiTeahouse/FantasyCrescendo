namespace Hourai.SmashBrew {
    
    public interface IMatchRule {

        void OnMatchStart();
        void OnMatchEnd();
        void OnSpawn(Character character);
        void OnMatchUpdate();

        bool IsFinished { get; }
        Character Winner { get; }

    }

}
