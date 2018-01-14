using HouraiTeahouse;
using HouraiTeahouse.FantasyCrescendo;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class AbstractDataTest<T> where T : GameDataBase {

    protected delegate object AssetFunc(T data);

    protected delegate IEnumerable AssetManyFunc(T data);

    protected static IEnumerable<T> data;

    public static IEnumerable<object[]> TestData() {
        if (data == null) {
          data = EditorAssetUtil.LoadAll<T>().Where(d => d != null && d.IsSelectable && d.IsVisible);
        }
        foreach (var datum in data) {
          yield return new object[] {datum};
        }
    }

}