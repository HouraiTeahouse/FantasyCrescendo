using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

//TODO(james7132): Unit test this

namespace HouraiTeahouse.FantasyCrescendo {

public class InputHistory<I> : IReadOnlyCollection<I> where I : IMergable<I> {

  private class Element {
    public uint Timestamp;
    public I Input;
    public Element Next;
  }

  public int Count { get; private set; }
  public I Latest => Tail.Input;
  public uint LastConfirmedTimestep => Head.Timestamp;

  Element Head;
  Element Tail;
  uint LastTimestamp;

  // Creates a inputhistory starting at a given timestamp
  public InputHistory(uint startTimestamp = 0) {
    LastTimestamp = startTimestamp;
  }

  // Appends a new input to the end of the list
  public void Append(I input) {
    var newElement = ObjectPool<Element>.Shared.Rent();
    newElement.Timestamp = ++LastTimestamp;
    newElement.Input = input;
    newElement.Next = null;
    if (Tail != null) {
      Tail.Next = newElement;
    }
    if (Head == null) {
      Head = newElement;
    }
    Tail = newElement;
    Count++;
  }

  // Merges the given input history with an per tick enumeration of the input.
  public void MergeWith(uint startTimestamp, IEnumerable<I> source) {
    Assert.IsNotNull(source);
    Element currentNode = FindByTimestamp(startTimestamp);
    IEnumerator<I> enumerator = source.GetEnumerator();
    while (currentNode != null && enumerator.MoveNext()) {
      currentNode.Input.MergeWith(enumerator.Current);
    }
    while (enumerator.MoveNext()) {
      Append(enumerator.Current);
    }
  }

  public void DropWhile(Func<I, bool> predicate) {
    Argument.NotNull(predicate);
    Drop(e => predicate(e.Input));
  }

  // Drops all inputs before a given timestamp from the history.
  public void DropBefore(uint timestamp) => Drop(e => e.Timestamp <  timestamp);

  void Drop(Func<Element, bool> predicate) {
    Assert.IsNotNull(predicate);
    var pool = ObjectPool<Element>.Shared;
    Element currentNode = Head;
    while (currentNode != null && predicate(currentNode)) {
      pool.Return(currentNode);
      currentNode = currentNode.Next;
      Count--;
    }
    Head = currentNode;
    if (Head == null && Tail != null) {
      LastTimestamp = Tail.Timestamp;
      Tail = null;
    }
  }

  public IEnumerable<I> StartingWith(uint timestamp) {
    Element currentNode = FindByTimestamp(timestamp);
    while (currentNode != null) {
      yield return currentNode.Input;
      currentNode = currentNode.Next;
    }
  }

  Element FindByTimestamp(uint timestamp) {
    Element currentNode = Head;
    while (currentNode != null && currentNode.Timestamp < timestamp) {
      currentNode = currentNode.Next;
    }
    return currentNode;
  }

  public IEnumerator<I> GetEnumerator() => StartingWith(0).GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

}

}
