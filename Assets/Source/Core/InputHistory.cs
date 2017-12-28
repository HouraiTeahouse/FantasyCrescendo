using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

//TODO(james7132): Unit test this

namespace HouraiTeahouse.FantasyCrescendo {

public struct TimedInput<I> {
  public uint Timestamp;
  public I Input;
}

public class InputHistory<I> : IEnumerable<TimedInput<I>> {

  private class Element {
    public TimedInput<I> Value;
    public Element Next;
  }

  Element Head;
  Element Tail;
  uint LastTimestamp;

  // Creates a inputhistory starting at a given timestamp
  public InputHistory(uint startTimestamp = 0) {
    LastTimestamp = startTimestamp;
  }

  // Appends a new input to the end of the list
  public void Append(I input) {
    var newElement = new Element {
        Value = new TimedInput<I> {
          Timestamp = ++LastTimestamp,
          Input = input
        }
    };
    if (Tail != null) {
      Tail.Next = newElement;
    }
    if (Head == null) {
      Head = newElement;
    }
    Tail = newElement;
  }

  // Merges the given input history with an per tick enumeration of the input.
  public void MergeWith(uint startTimestamp, IEnumerable<I> source,
                        Func<I, I, I> mergeFunc) {
    Assert.IsNotNull(mergeFunc);
    Assert.IsNotNull(source);
    Element currentNode = FindByTimestamp(startTimestamp);
    IEnumerator<I> enumerator = source.GetEnumerator();
    while (currentNode != null && enumerator.MoveNext()) {
      currentNode.Value.Input = mergeFunc(currentNode.Value.Input,
                                          enumerator.Current);
    }
    while (enumerator.MoveNext()) {
      Append(enumerator.Current);
    }
  }

  // Drops all inputs before a given timestamp from the history.
  public void DropBefore(uint timestamp) {
    Head = FindByTimestamp(timestamp);
    if (Head == null) {
      LastTimestamp = Tail.Value.Timestamp;
      Tail = null;
    }
  }

  public IEnumerator<TimedInput<I>> GetEnumerator() {
    return StartingWith(0).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() {
    return GetEnumerator();
  }

  public IEnumerable<TimedInput<I>> StartingWith(uint timestamp) {
    Element currentNode = FindByTimestamp(timestamp);
    while (currentNode != null) {
      yield return currentNode.Value;
      currentNode = currentNode.Next;
    }
  }

  Element FindByTimestamp(uint timestamp) {
    Element currentNode = Head;
    while (currentNode != null && currentNode.Value.Timestamp < timestamp) {
      currentNode = currentNode.Next;
    }
    return currentNode;
  }

}

}
