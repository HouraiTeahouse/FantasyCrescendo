using UnityEngine;
using System.Collections;

public class Serno {
    public bool IsBaka {
        get { return true; }
    }

    public bool IsStrongest {
        get { return true; }
    }

    public int Bus {
        get { throw new NoBusesInGensokyoException("There are no buses in Gensokyo!"); }
    }

    override public string ToString() {
        return "⑨";
    }

    public int IComparable.CompareTo(object anyOtherBeing){
        return -1;
    }
}

public class NoBusesInGensokyoException : System.Exception {
    public NoBusesInGensokyoException(string e) {
    }
}
