using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A managed history of a set of inputs.
/// </summary>
/// <remarks>
/// This is internally implemented as a LinkedList, most of the operations on these objects
/// run in O(n) time.
/// </remarks>
public class InputHistory<I> : IReadOnlyCollection<I> where I : IMergable<I> {

  private class Element {
    public uint Timestamp;
    public I Input;
    public Element Next;
  }

  /// <summary>
  /// Gets the number of stored inputs
  /// </summary>
  public int Count { get; private set; }

  /// <summary>
  /// Gets the most recently stored input in the history.
  /// </summary>
  public I Latest => Tail.Input;

  /// <summary>
  /// Gets the oldest stored input's timestamp.
  /// </summary>
  public uint LastConfirmedTimestep { get; private set; }

  Element Head;
  Element Tail;
  uint LastTimestep;

  /// <summary>
  /// Creates a InputHistory starting at a given timestamp
  /// </summary>
  /// <param name="startTimestamp">the timestamp to start at</param>
  public InputHistory(uint startTimestamp = 0) {
    LastTimestep = startTimestamp;
    LastConfirmedTimestep = startTimestamp;
  }

  /// <summary>
  /// Appends a new input as the most recent input in the history.
  /// </summary>
  /// <param name="input">the new input to add to the history</param>
  public void Append(I input) {
    var newElement = ObjectPool<Element>.Shared.Rent();
    newElement.Timestamp = ++LastTimestep;
    newElement.Input = input;
    newElement.Next = null;
    if (Tail != null) {
      Tail.Next = newElement;
    }
    if (Head == null) {
      Head = newElement;
      LastConfirmedTimestep = newElement.Timestamp;
    }
    Tail = newElement;
    Count++;
  }

  /// <summary>
  /// Performs an element-wise merge between the history and the
  /// </summary>
  /// <remarks>
  /// Only mutates the history. Any excess inputs will be appended to the end 
  /// of the history.
  /// 
  /// Merges are done with the `IMergable.MergeWith` method of the provided type.
  /// </remarks>
  /// <param name="startTimestamp">the timestamp to start the merge.</param>
  /// <param name="source">an enumeration of inputs to merge into the history.</param>
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

  public void MergeWith(uint startTimestamp, ArraySlice<I> source) {
    Element currentNode = FindByTimestamp(startTimestamp);
    ArraySlice<I>.Enumerator enumerator = source.GetEnumerator();
    while (currentNode != null && enumerator.MoveNext()) {
      currentNode.Input.MergeWith(enumerator.Current);
    }
    while (enumerator.MoveNext()) {
      Append(enumerator.Current);
    }
  }

  /// <summary>
  /// Drops all inputs before a given timestamp from the history.
  /// </summary>
  /// <param name="timestamp">the starting timestamp.</param>
  public void DropBefore(uint timestamp) {
    var pool = ObjectPool<Element>.Shared;
    Element currentNode = Head;
    while (currentNode != null && currentNode.Timestamp < timestamp) {
      pool.Return(currentNode);
      (currentNode.Input as IDisposable)?.Dispose();
      LastConfirmedTimestep = currentNode.Timestamp;
      currentNode = currentNode.Next;
      Count--;
    }
    Head = currentNode;
    if (Head == null && Tail != null) {
      LastTimestep = Tail.Timestamp;
      Tail = null;
    }
  }

  /// <summary>
  /// Enumerates all the inputs 
  /// </summary>
  /// <param name="timestamp"></param>
  /// <returns></returns>
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

  public IEnumerator<I> GetEnumerator() {
    Element currentNode = Head;
    while (currentNode != null) {
      yield return currentNode.Input;
      currentNode = currentNode.Next;
    }
  }

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

}

}
