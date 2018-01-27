using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class MenuManager : MonoBehaviour {

  public static MenuManager Instance  { get; private set; }

  public IReadOnlyCollection<Menu> Menus { get; private set; }
  public IReadOnlyCollection<Menu> Breadcrumbs { get; private set; }
  public Menu CurrentMenu => breadcrumbs.Peek();

  public Menu MainMenu;

  List<Menu> menus;
  Stack<Menu> breadcrumbs;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    Instance = this;

    menus = new List<Menu>(Resources.FindObjectsOfTypeAll<Menu>());
    breadcrumbs = new Stack<Menu>();

    Menus = menus.AsReadOnly();
    Breadcrumbs = breadcrumbs.AsReadOnly();

    SetMenu(MainMenu);
  }

  public void SetMenu(Menu currentMenu) {
    if (currentMenu == null || IsCurrentMenu(currentMenu)) return;
    breadcrumbs.Push(currentMenu);
    UpdateMenu();
  }

  public void Return() {
    if (breadcrumbs.Count <= 0) return;
    breadcrumbs.Pop();
    UpdateMenu();
  }

  bool IsCurrentMenu(Menu menu) => breadcrumbs.Count > 0 && menu == CurrentMenu;

  void UpdateMenu() {
    var currentlyManaged = false;
    foreach (var menu in menus) {
      var isCurrent = menu == CurrentMenu;
      menu.gameObject.SetActive(isCurrent);
      currentlyManaged |= isCurrent;
    }
    if (!currentlyManaged) menus.Add(CurrentMenu);
  }

}

}