// The MIT License (MIT)
// 
// Copyright (c) 2016 Hourai Teahouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse {
    public class MenuManager : MonoBehaviour {
        static Stack<string> _menuBreadcrumnbs;

        Dictionary<string, Menu> _availableMenus;

        [SerializeField]
        Menu _currentMenu;

        void Start() {
            _availableMenus =
                FindObjectsOfType<Menu>().ToDictionary(menu => menu.Name);
            if (_menuBreadcrumnbs == null) {
                _menuBreadcrumnbs = new Stack<string>();
            }
            else {
                _currentMenu = null;
                // Recurse back up the 
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
            if (menu) {
                menu.gameObject.SetActive(true);
                if (!_availableMenus.ContainsValue(menu))
                    _availableMenus.Add(menu.Name, menu);
            }
            _currentMenu = menu;
        }
    }
}