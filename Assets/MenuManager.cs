using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace HouraiTeahouse {

    public class MenuManager : MonoBehaviour {

        [SerializeField]
        private Menu _currentMenu;

        private Dictionary<string, Menu> _availableMenus;
        private static Stack<string> _menuBreadcrumnbs;

        void Start() {
            _availableMenus = FindObjectsOfType<Menu>().ToDictionary(menu => menu.Name);
            if (_menuBreadcrumnbs == null) {
                _menuBreadcrumnbs = new Stack<string>();
            }
            else {
                _currentMenu = null;
                // Recurse back up the 
                while(!_currentMenu && _menuBreadcrumnbs.Count > 0) {
                    string currentName = _menuBreadcrumnbs.Pop();
                    if(_availableMenus.ContainsKey(currentName))
                        ChangeMenu(_availableMenus[currentName]);
                }
            }
            if(_currentMenu != null)
                _menuBreadcrumnbs.Push(_currentMenu.Name);
        }

        public void ChangeMenu(Menu menu) {
            if(_currentMenu)
                 _currentMenu.gameObject.SetActive(false);
            if (menu) {
                menu.gameObject.SetActive(true);
                if (!_availableMenus.ContainsValue(menu))
                    _availableMenus.Add(menu.Name, menu);
            }
            _currentMenu = menu;
        }

    }

}
