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
/// This is internally implemented as a singly-linked list, most of the 
/// operations on these objects run in O(n) time.
/// </remarks>
public class InputHistory<I> : IEnumerable<TimedInput<I>> {

  struct Element {
    public TimedInput<I> Value;
    public I Input {
      get { return Value.Input; }
      set { Value.Input = value; }
    }
    public uint Timestep {
      get { return Value.Timestep; }
      set { Value.Timestep = value; }
    }
    public int? Next;
  }

  public const int kDefaultStartSize = 128;

  int oldestIndex;
  int currentIndex;
  int newestIndex;
  Element[] inputs;
  readonly Stack<int> pool;
  readonly IMerger<I> merger;
  readonly bool isDisposable;

  
  /// <summary>
  /// Gets the oldest recorded input and timestamp.
  /// </summary>
  public TimedInput<I> Oldest => inputs[oldestIndex].Value;

  /// <summary>
  /// Gets the current input and timestamp.
  /// </summary>
  public TimedInput<I> Current => inputs[currentIndex].Value;

  /// <summary>
  /// Gets the newest recorded input and timestamp.
  /// </summary>
  public TimedInput<I> Newest => inputs[newestIndex].Value;

  public int Count => inputs.Length - pool.Count;

  /// <summary>
  /// Initalizes a new instance of the <see cref="InputHistory{T}"/> class.
  /// </summary>
  /// <param name="input">the starting base input.</param>
  public InputHistory(I input) : this(input, 0, Merger<I>.Default) { }

  /// <summary>
  /// Initalizes a new instance of the <see cref="InputHistory{T}"/> class.
  /// </summary>
  /// <param name="input">the starting base input.</param>
  /// <param name="startTimestamp">the timestamp of the first element.</param>
  public InputHistory(I input, uint startTimestamp) : this(input, startTimestamp, Merger<I>.Default) {
  }

  /// <summary>
  /// Initalizes a new instance of the <see cref="InputHistory{T}"/> class.
  /// </summary>
  /// <param name="input">the starting base input.</param>
  /// <param name="merger">a <see cref="IMerger{T}"/> used to merge inputs.</param>
  public InputHistory(I input, IMerger<I> merger) : this(input, 0, merger) {
  }

  /// <summary>
  /// Initalizes a new instance of the <see cref="InputHistory{T}"/> class.
  /// </summary>
  /// <param name="input">the starting base input.</param>
  /// <param name="startTimestamp">the timestamp of the first element.</param>
  /// <param name="merger">a <see cref="IMerger{T}"/> used to merge inputs.</param>
  public InputHistory(I start, uint startTimestep,
                      IMerger<I> merger, int capacity = kDefaultStartSize) {
    inputs = new Element[capacity];
    pool = new Stack<int>(capacity);
    isDisposable = typeof(IDisposable).IsAssignableFrom(typeof(I));
    this.merger = merger;
    inputs[0] = new Element {
      Input = start,
      Timestep = startTimestep,
      Next = null
    };
    for (var i = inputs.Length - 1; i > 0; i--) {
      pool.Push(i);
    }
  }

  /// <summary>
  /// Steps <see cref="Current"/> forward and retrieves 
  /// </summary>
  /// <remarks>
  /// This will not go past <see cref="Newest"/><
  /// 
  /// This operation runs in O(1) time.
  /// </remarks>
  /// <returns>the new current input.</returns>
  public I Step() => inputs[currentIndex = inputs[currentIndex].Next ?? newestIndex].Input;

  /// <summary>
  /// Resets <see cref="Current"/> back to the oldest recorded input.
  /// </summary>
  /// <remarks>This operation runs in O(1) time.</remarks>
  /// <returns>the new current input.</returns>
  public I Rewind() => inputs[(currentIndex = newestIndex)].Input;

  /// <summary>
  /// Updates <see cref="Current"/> forward to the newest recorded input.
  /// </summary>
  /// <remarks>This operation runs in O(1) time.</remarks>
  /// <returns>the new current input.</returns>
  public I FastForward() => inputs[(currentIndex = newestIndex)].Input;

  /// <summary>
  /// Merges the sequence of inputs with the existing history of inputs.
  /// </summary>
  /// <remarks>
  /// Inputs from before the oldest recorded input will be ignored.
  /// 
  /// Inputs newer than the newest recorded input will be appended to
  /// the history and become the newest inputs added.
  /// 
  /// Intermediate inputs that have a matching input for the same timestep
  /// will be merged according to the <see cref="IMerger"/> provided during
  /// the history's construction.
  /// 
  /// Intermedate inputs that do not have a corresponding input will be 
  /// inserted into approriate locations.
  /// 
  /// This operation runs in O(n) time where n is the number of elements
  /// managed by the history.
  /// </remarks>
  /// <param name="timestep">
  /// The start timestamp corresponding to the first element in the list.
  /// </param>
  /// <param name="inputs">the list of inputs to merge into the history.</param>
  public void MergeWith(uint timestep, IEnumerable<I> inputs) {
    int index = FindByTimestamp(timestep);
    foreach( var input in inputs) {
      if (timestep >= this.inputs[index].Timestep) {
        index = AppendOrMerge(index, input, timestep);
      }
      timestep++;
    }
  }

  /// <summary>
  /// Merges the sequence of inputs with the existing history of inputs.
  /// </summary>
  /// <remarks>
  /// Inputs from before the oldest recorded input will be ignored.
  /// 
  /// Inputs newer than the newest recorded input will be appended to
  /// the history and become the newest inputs added.
  /// 
  /// Intermediate inputs that have a matching input for the same timestep
  /// will be merged according to the <see cref="IMerger"/> provided during
  /// the history's construction.
  /// 
  /// Intermedate inputs that do not have a corresponding input will be 
  /// inserted into approriate locations.
  /// 
  /// This operation runs in O(n) time where n is the number of elements
  /// managed by the history.
  /// </remarks>
  /// <param name="timestep">
  /// The start timestamp corresponding to the first element in the list.
  /// </param>
  /// <param name="inputs">the list of inputs to merge into the history.</param>
  public void MergeWith(uint timestep, ArraySegment<I> inputs) {
    int index = FindByTimestamp(timestep);
    foreach( var input in inputs) {
      if (timestep >= this.inputs[index].Timestep) {
        index = AppendOrMerge(index, input, timestep);
      }
      timestep++;
    }
  }


  /// <summary>
  /// Drops all elements older than a certain timestamp.
  /// </summary>
  /// <remarks>
  /// If the <type cref="Current"/> input is dropped, it will be updated
  /// to the new oldest input in the history.
  /// 
  /// If <typeparamref name="T"/> inherits from <see cref="IDisposable"/>
  /// all dropped inputs will be disposed.
  /// 
  /// This operation runs in O(n) time where n is the number of elements
  /// managed by the history.
  /// </remarks>
  /// <param name="timestep">the new oldest timestamp supported.</param>
  public void DropBefore(uint timestep) {
    var oldest = inputs[oldestIndex];
    while (oldest.Next != null && oldest.Timestep < timestep) {
      if (isDisposable) ((IDisposable)oldest.Input).Dispose();
      pool.Push(oldestIndex);
      oldestIndex = inputs[oldestIndex].Next.Value;
      oldest = inputs[oldestIndex];
    }
    if (oldest.Timestep >= inputs[currentIndex].Timestep) {
      currentIndex = oldestIndex;
    }
  }

  /// <summary>
  /// Advances the <see cref="Current"/> input by one timestep and records it
  /// in the history.
  /// </summary>
  /// <remarks>
  /// If there are inputs in the history newer than the current input, the 
  /// provided input may be merged with the newer input before being stored.
  /// The merge strategy is determined by the <see cref="IMerger"/> provided
  /// during the history's construction.
  /// 
  /// This operation runs in O(1) time.
  /// </remarks>
  /// <param name="input"></param>
  /// <returns></returns>
  public I Append(I input) {
    currentIndex = AppendOrMerge(currentIndex, input);
    return inputs[currentIndex].Input;
  }

  /// <summary>
  /// Enumerates the full sequence of timed inputs, including the inputs newer
  /// than the <see cref="Current"/> timed input.
  /// </summary>
  public IEnumerable<TimedInput<I>> GetFullSequence() {
    var enumerator = GetEnumerator(true);
    while (enumerator.MoveNext()) {
      yield return enumerator.Current;
    }
  }

  public IEnumerable<TimedInput<I>> StartingWith(uint timestep, bool full = false) {
    int index = FindByTimestamp(timestep);
    int endIndex = full ? newestIndex : currentIndex;
    while (index != currentIndex) {
      yield return inputs[index].Value;
      index = inputs[index].Next.Value;
    }
  }

  public Enumerator GetEnumerator(bool full = false) => new Enumerator(this, full);
  IEnumerator<TimedInput<I>> IEnumerable<TimedInput<I>>.GetEnumerator() => GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  int AppendOrMerge(int index, I input) {
    var previousTimestep = inputs[index].Timestep;
    return AppendOrMerge(index, input, previousTimestep + 1);
  }

  int AppendOrMerge(int index, I input, uint timestep) {
    var current = inputs[index];
    var next = GetNext(current);
    var nextIndex = current.Next;
    if (next?.Timestep == timestep) {
      index = nextIndex.Value;
      inputs[index].Input = merger.Merge(inputs[index].Input, input);
    } else if (current.Timestep == timestep) {
      inputs[index].Input = merger.Merge(inputs[index].Input, input);
    } else if (next == null || next?.Timestep > timestep + 1) {
      return Append(index, input, timestep);
    } else {
      Debug.LogAssertion($"Invalid AppedOrMerge call: i:{index} ts:{timestep}");
    }
    return index;
  }

  int Append(int previousIndex, I input, uint timestep) {
    if (pool.Count <= 0) Resize();
    var previous = inputs[previousIndex];
    var newIndex = pool.Pop();
    inputs[newIndex] = new Element {
      Input = input,
      Timestep = timestep,
      Next = previous.Next
    };
    inputs[previousIndex].Next = newIndex;
    if (newestIndex == previousIndex) {
      newestIndex = newIndex;
    }
    return newIndex;
  }

  Element? GetNext(Element element) {
    if (element.Next == null) return null;
    return inputs[element.Next.Value];
  }

  int FindByTimestamp(uint timestep) {
    if (timestep >= inputs[newestIndex].Timestep) return newestIndex;
    int index, endIndex;
    if (timestep > inputs[currentIndex].Timestep) {
      index = currentIndex;
      endIndex = newestIndex;
    } else {
      index = oldestIndex;
      endIndex = currentIndex;
    }
    while (index != endIndex && inputs[index].Timestep < timestep) {
      index = inputs[index].Next.Value;
    }
    return index;
  }

  void Resize() {
    Assert.IsTrue(pool.Count <= 0);
    var newInputs = new Element[inputs.Length * 2];
    Array.Copy(inputs, 0, newInputs, 0, inputs.Length);
    for (var i = newInputs.Length - 1; i >= inputs.Length; i--) {
      pool.Push(i);
    }
    inputs = newInputs;
  }

  public struct Enumerator : IEnumerator<TimedInput<I>> {

    readonly Element[] inputs;
    readonly int startIndex;
    readonly int endIndex;
    int currentIndex;

    public TimedInput<I> Current => inputs[currentIndex].Value;
    object IEnumerator.Current => Current;

    internal Enumerator(InputHistory<I> history, bool full) {
      startIndex = history.oldestIndex;
      inputs = history.inputs;
      endIndex = full ? history.newestIndex : history.currentIndex;
      currentIndex = -1;
    }

    public bool MoveNext() {
      var hasNext = currentIndex != endIndex;
      if (currentIndex < 0) {
        currentIndex = startIndex;
        return true;
      }  else if (hasNext) {
        currentIndex = inputs[currentIndex].Next.Value;
      }
      return hasNext;
    }

    public void Reset() => currentIndex = -1;

    void IDisposable.Dispose() {}

  }


}

}
