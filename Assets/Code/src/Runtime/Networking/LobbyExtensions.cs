using HouraiTeahouse.Networking;
using System;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public static class LobbyExtensions {

  // Fantasy Crescendo Specific Lobby Helpers

  const string kNameKey = "name";
  const string kStageKey = "stage";
  const string kStocksKey = "stocks";
  const string kTimeKey = "time";

  public static string GetName(this IMetadataContainer container) =>
    container.GetMetadata(kNameKey);

  public static void SetName(this IMetadataContainer container, string name) =>
    container.SetMetadata(kNameKey, name);

  public static uint GetStageID(this IMetadataContainer container) =>
    container.GetUInt32(kStageKey);

  public static void SetStageID(this IMetadataContainer container, uint stage) =>
    container.SetUInt32(kStageKey, stage);

  public static uint GetStocks(this IMetadataContainer container) =>
    container.GetUInt32(kStocksKey);

  public static void SetStocks(this IMetadataContainer container, uint stage) =>
    container.SetUInt32(kStocksKey, stage);

  public static uint GetTime(this IMetadataContainer container) =>
    container.GetUInt32(kTimeKey);

  public static void SetTime(this IMetadataContainer container, uint stage) =>
    container.SetUInt32(kTimeKey, stage);

  // General Lobby Extensions

  public static uint GetUInt32(this IMetadataContainer container, string key, uint defaultValue = 0) =>
    ParseUint32(container.GetMetadata(key), defaultValue);

  public static void SetUInt32(this IMetadataContainer container, string key, uint value) =>
    container.SetMetadata(key, value.ToString());

  public static uint ParseUint32(string val, uint defaultValue = 0) {
      if (String.IsNullOrEmpty(val)) return defaultValue;
      return (UInt32.TryParse(val, out uint result)) ? result : defaultValue;
  }

}

}