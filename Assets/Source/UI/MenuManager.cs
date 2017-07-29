using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse {

    public class MenuOpenedEvent {
        public string MenuName;
    }

    public class MenuManager : MonoBehaviour, IRegistrar<Menu> {

        static Stack<string> _menuBreadcrumnbs;

        public static MenuManager Instance { get; private set; }

        Dictionary<string, Menu> _availableMenus;

        [SerializeField]
        Menu _currentMenu;

        void Awake() {
            Instance = this;
            if (_availableMenus == null)
                _availableMenus = new Dictionary<string, Menu>();
        }

        void Start() {
            if (_menuBreadcrumnbs == null) {
                _menuBreadcrumnbs = new Stack<string>();
            } else {
                _currentMenu = null;
                while (!_currentMenu && _menuBreadcrumnbs.Count > 0) {
                    string currentName = _menuBreadcrumnbs.Pop();
                    if (_availableMenus.ContainsKey(currentName))
                        ChangeMenu(_availableMenus[currentName]);
                }
            }
            if (_currentMenu != null)
                _menuBreadcrumnbs.Push(_currentMenu.Name);
            foreach (Menu inactiveMenu in _availableMenus.Values)
                if (inactiveMenu && inactiveMenu != _currentMenu)
                    inactiveMenu.gameObject.SetActive(false);
        }

        public void ChangeMenu(Menu menu) {
            if (_currentMenu)
                _currentMenu.gameObject.SetActive(false);
            if (menu != null) {
                menu.gameObject.SetActive(true);
                if (!_availableMenus.ContainsValue(menu))
                    _availableMenus.Add(menu.Name, menu);
                Mediator.Global.Publish(new MenuOpenedEvent {
                    MenuName = menu.Name
                });
            }
            _currentMenu = menu;
        }

        public void Register(Menu obj) {
            if (obj == null) {
                Log.Error("Attempted to register null menu.");
                return;
            }
            if (_availableMenus == null)
                _availableMenus = new Dictionary<string, Menu>();
            Menu altRegistry;
            string menuName = obj.Name;
            if (_availableMenus.TryGetValue(menuName, out altRegistry) && obj != altRegistry) {
                Log.Error("Cannot register multple menus under the name of {0}. ", obj.Name);
                return;
            }
            _availableMenus.Add(menuName, obj);
            Log.Info("Registered {0} as a valid menu under the name of {1}", obj, obj.Name);
        }

        public bool Unregister(Menu obj) {
            if (obj == null || _availableMenus == null)
                return false;
            Menu registeredMenu; 
            if (!_availableMenus.TryGetValue(obj.Name, out registeredMenu))
                return false ;
            if (registeredMenu == obj)
                _availableMenus.Remove(obj.Name);
            return true;
        }

    }

}