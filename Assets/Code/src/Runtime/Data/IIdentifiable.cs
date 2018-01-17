namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// An (almost) uniquely identifiable object.
/// </summary>
public interface IEntity {

  /// <summary>
  /// Gets the (almost) unique identifier for the object.
  /// </summary>
  /// <remarks>
  /// This value must be static over the lifetime of an object. 
  /// 
  /// The identifier should be unique on any local application 
  /// using this interface.
  /// 
  /// Any networked use of this identifier should point to the 
  /// same resource or object no matter what machine it executes
  /// on.
  /// </remarks>
  /// <returns></returns>
  uint Id { get; }

}

}

