using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo {

public interface IMerger<I> {

  I Merge(I a, I b);

}

public static class Merger<T> {

  public static IMerger<T> Default {
    get {
      if (typeof(IMergable<T>).IsAssignableFrom(typeof(T))) {
        return new DefaultMerger();
      }
      throw new NotImplementedException();
    }
  }

  public static IMerger<T> FromDelegate(Func<T, T, T> mergeFunc) {
    return new DelegateMerger(mergeFunc);
  }

  class DefaultMerger : IMerger<T> {

    public T Merge(T a, T b) {
      ((IMergable<T>)a).MergeWith(b);
      return a;
    }

  }

  class DelegateMerger : IMerger<T> {

    readonly Func<T, T, T> mergeFunc;

    public DelegateMerger(Func<T, T, T> mergeFunc) {
      this.mergeFunc = Argument.NotNull(mergeFunc);
    }

    public T Merge(T a, T b) => mergeFunc(a, b);

  }

}

public struct TimedInput<I> {
  public I Input;
  public uint Timestep;
}

/// <summary>
/// A managed history of a set of inputs.
/// </summary>
/// <remarks>
/// This is internally implemented as a LinkedList, most of the operations on these objects
/// run in O(n) time.
/// </remarks>
public class InputHistory<I> : IEnumerable<TimedInput<I>> {

  readonly IMerger<I> merger;
  Element oldest;
  Element current;
  Element newest;

  public TimedInput<I> Oldest => new TimedInput<I> {
    Input = oldest.Input,
    Timestep = oldest.Timestep
  };

  public TimedInput<I> Current => new TimedInput<I> {
    Input = current.Input,
    Timestep = current.Timestep
  };

  public TimedInput<I> Newest => new TimedInput<I> {
    Input = newest.Input,
    Timestep = newest.Timestep
  };

  public InputHistory(I input) : this(input, 0, Merger<I>.Default) { }

  public InputHistory(I input, uint startTimestamp) : this(input, startTimestamp, Merger<I>.Default) {
  }

  public InputHistory(I input, IMerger<I> merger) : this(input, 0, merger) {
  }

  public InputHistory(I input, uint startTimestamp, IMerger<I> merger) {
    this.merger = Argument.NotNull(merger);
    oldest = ObjectPool<Element>.Shared.Rent();
    oldest.Timestep = startTimestamp;
    oldest.Input = input;
    oldest.Next = null;
    current = oldest;
    newest = oldest;
  }

  public IEnumerable<TimedInput<I>> GetFullSequence() {
    var enumerator = GetEnumerator(true);
    while (enumerator.MoveNext()) {
      yield return enumerator.Current;
    }
  }

  public Enumerator GetEnumerator(bool full = false) => new Enumerator(this, full);
  IEnumerator<TimedInput<I>> IEnumerable<TimedInput<I>>.GetEnumerator() => GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  public void MergeWith(uint timestep, ArraySlice<I> inputs) {
    Element currentElement = FindByTimestamp(timestep);
    foreach (var input in inputs) {
      currentElement = AppendOrMerge(currentElement, input, timestep);
      timestep++;
    }
  }

  public void DropBefore(uint timestep) {
    var pool = ObjectPool<Element>.Shared;
    while (oldest != newest && oldest.Timestep < timestep)  {
      (oldest.Input as IDisposable)?.Dispose();
      pool.Return(oldest);
      oldest = oldest.Next;
    }
    if (oldest.Timestep > current.Timestep) {
      current = oldest;
    }
    Assert.AreEqual(oldest?.Timestep, timestep);
  }

  public void Append(I input) {
    current = AppendOrMerge(current, input);
  }

  Element AppendOrMerge(Element previous, I input) {
    return AppendOrMerge(previous, input, previous.Timestep + 1);
  }

  Element AppendOrMerge(Element currentElement, I input, uint timestep) {
    var nextElement = currentElement.Next;
    if (nextElement == null || nextElement.Timestep > timestep + 1) {
      Assert.IsTrue(timestep > currentElement.Timestep);
      currentElement = Append(currentElement, input, timestep);
    } else if (nextElement.Timestep == timestep) {
      nextElement.Input = merger.Merge(nextElement.Input, input);
      currentElement = nextElement;
    } else if (currentElement.Timestep == timestep) {
      currentElement.Input = merger.Merge(currentElement.Input, input);
    } else {
      Debug.LogAssertion("Invalid AppendOrMerge call.");
    }
    return currentElement;
  }

  Element Append(Element previous, I input, uint timestep) {
    var nextElement = previous.Next;
    var newElement = ObjectPool<Element>.Shared.Rent();
    newElement.Timestep = timestep;
    newElement.Input = input;
    newElement.Next = nextElement;
    previous.Next = newElement;
    if (newest == previous) {
      newest = newElement;
    }
    return newElement;
  }

  Element FindByTimestamp(uint timestep) {
    if (timestep > newest.Timestep) return newest;
    Element currentElement, end;
    if (timestep > current.Timestep) {
      currentElement = current;
      end = newest;
    } else {
      currentElement = oldest;
      end = current;
    }
    while (currentElement != end && currentElement.Timestep < timestep) {
      currentElement = currentElement.Next;
    }
    Assert.IsNotNull(currentElement);
    return currentElement;
  }

  class Element {
    public uint Timestep;
    public I Input;
    public Element Next;
  }

  public struct Enumerator : IEnumerator<TimedInput<I>> {

    readonly Element StartElement;
    readonly Element EndElement;
    Element current;

    public TimedInput<I> Current => new TimedInput<I> {
      Input = current.Input,
      Timestep = current.Timestep
    };
    object IEnumerator.Current => Current;

    public Enumerator(InputHistory<I> inputHistory, bool full) {
      StartElement = inputHistory.oldest;
      EndElement = full ? inputHistory.newest : inputHistory.current;
      current = null;
    }

    public bool MoveNext() {
      var hasNext = current != EndElement;
      if (current == null) {
        current = StartElement;
        return true;
      } else if (hasNext) {
        current = current.Next;
      }
      return hasNext;
    }

    public void Reset() => current = StartElement;

    void IDisposable.Dispose() { }

  }

}

}
