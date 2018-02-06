using System;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A merge strategy for combining two objects into one.
/// </summary>
public interface IMerger<I> {

  I Merge(I a, I b);

}

/// <summary>
/// A static class for creating <see cref="IMerger{T}"/> objects.
/// </summary>
public static class Merger<T> {

  /// <summary>
  /// Gets the default merger for a type.
  /// </summary>
  /// <remarks>
  /// Currently only supports <typeparamref name="T"/> types that derive from
  /// <see cref="IMergable{T}"/>. Attempting to get a default merger that does
  /// not derive from the interface will throw a <see cref="System.NotImplementedException"/>.
  /// </remarks>
  /// <returns>the default merger for the type.</returns>
  public static IMerger<T> Default {
    get {
      if (typeof(IMergable<T>).IsAssignableFrom(typeof(T))) {
        return new DefaultMerger();
      }
      throw new NotImplementedException();
    }
  }

  /// <summary>
  /// Creates a merger based on a provided delegate.
  /// </summary>
  /// <param name="mergeFunc">the merge strategy for the merger.</param>
  /// <exception cref="System.ArgumentNullException">
  ///   <paramref cref="mergeFunc"/> is null.
  /// </exception>
  /// <returns>the merger object.</returns>
  public static IMerger<T> FromDelegate(Func<T, T, T> mergeFunc) {
    return new DelegateMerger(mergeFunc);
  }

  class DefaultMerger : IMerger<T> {
    public T Merge(T a, T b) => ((IMergable<T>)a).MergeWith(b);
  }

  class DelegateMerger : IMerger<T> {

    readonly Func<T, T, T> mergeFunc;

    public DelegateMerger(Func<T, T, T> mergeFunc) {
      this.mergeFunc = Argument.NotNull(mergeFunc);
    }

    public T Merge(T a, T b) => mergeFunc(a, b);

  }

}

}