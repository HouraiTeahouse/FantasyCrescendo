namespace Hourai.SmashBrew {
    
    public interface IMatchRule {

        bool IsFinished { get; }
        Character Winner { get; }

    }

}
