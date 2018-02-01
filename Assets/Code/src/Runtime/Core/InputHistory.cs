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

  readonly IMerger<I> merger;
  Element oldest;
  Element current;
  Element newest;
  
  /// <summary>
  /// Gets the oldest recorded input and timestamp.
  /// </summary>
  public TimedInput<I> Oldest => new TimedInput<I> {
    Input = oldest.Input,
    Timestep = oldest.Timestep
  };

  /// <summary>
  /// Gets the current input and timestamp.
  /// </summary>
  public TimedInput<I> Current => new TimedInput<I> {
    Input = current.Input,
    Timestep = current.Timestep
  };

  /// <summary>
  /// Gets the newest recorded input and timestamp.
  /// </summary>
  public TimedInput<I> Newest => new TimedInput<I> {
    Input = newest.Input,
    Timestep = newest.Timestep
  };

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
  public InputHistory(I input, uint startTimestamp, IMerger<I> merger) {
    this.merger = Argument.NotNull(merger);
    oldest = ObjectPool<Element>.Shared.Rent();
    oldest.Timestep = startTimestamp;
    oldest.Input = input;
    oldest.Next = null;
    current = oldest;
    newest = oldest;
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
  public I Step() => (current = current.Next ?? newest).Input;

  /// <summary>
  /// Resets <see cref="Current"/> back to the oldest recorded input.
  /// </summary>
  /// <remarks>This operation runs in O(1) time.</remarks>
  /// <returns>the new current input.</returns>
  public I Rewind() => (current = oldest).Input;

  /// <summary>
  /// Updates <see cref="Current"/> forward to the newest recorded input.
  /// </summary>
  /// <remarks>This operation runs in O(1) time.</remarks>
  /// <returns>the new current input.</returns>
  public I FastForward() => (current = newest).Input;

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
    Element currentElement = FindByTimestamp(timestep);
    foreach (var input in inputs) {
      if (timestep >= currentElement.Timestep) {
        currentElement = AppendOrMerge(currentElement, input, timestep);
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
  public void MergeWith(uint timestep, ArraySlice<I> inputs) {
    Element currentElement = FindByTimestamp(timestep);
    foreach (var input in inputs) {
      if (timestep >= currentElement.Timestep) {
        currentElement = AppendOrMerge(currentElement, input, timestep);
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
    var pool = ObjectPool<Element>.Shared;
    while (oldest.Next != null && oldest.Timestep < timestep)  {
      (oldest.Input as IDisposable)?.Dispose();
      pool.Return(oldest);
      oldest = oldest.Next;
    }
    if (oldest.Timestep >= current.Timestep) {
      current = oldest;
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
    current = AppendOrMerge(current, input);
    return current.Input;
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

  public Enumerator GetEnumerator(bool full = false) => new Enumerator(this, full);
  IEnumerator<TimedInput<I>> IEnumerable<TimedInput<I>>.GetEnumerator() => GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  Element AppendOrMerge(Element previous, I input) {
    return AppendOrMerge(previous, input, previous.Timestep + 1);
  }

  Element AppendOrMerge(Element currentElement, I input, uint timestep) {
    var nextElement = currentElement.Next;
    if (nextElement?.Timestep == timestep) {
      nextElement.Input = merger.Merge(nextElement.Input, input);
      currentElement = nextElement;
    } else if (currentElement?.Timestep == timestep) {
      currentElement.Input = merger.Merge(currentElement.Input, input);
    } else if (nextElement == null || nextElement.Timestep > timestep + 1) {
      Assert.IsTrue(timestep > currentElement.Timestep);
      currentElement = Append(currentElement, input, timestep);
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
