using System;

public class Serno : IComparable {
    public bool IsBaka {
        get { return true; }
    }

    public bool IsStrongest {
        get { return true; }
    }

    public int Bus {
        get { throw new NoBusesInGensokyoException("There are no buses in Gensokyo!"); }
    }

    public override string ToString() {
        return "⑨";
    }

    public int CompareTo(object anyOtherBeing){
        return -1;
    }
}

public class NoBusesInGensokyoException : System.Exception {
    public NoBusesInGensokyoException(string e) {
    }
}
