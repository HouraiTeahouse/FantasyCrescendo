# v0.3.0-alpha
## General Patch Notes
###New Additions:
* Characters: Youmu Konpaku is now playable.
* Stages: Forest of Magic is now selectable.
* Stages: Netherworld is now selectable.
* Stages: Soft platforms have been introduced. Players can fall through these by smashing the down direction.
* Menus: A basic placeholder stage select menu has been added.
* Menus: An options menu has been added.
* Menus: A credits menu has been added.
* Options: Game quality is now an in-game option.
* Options: Volume controls for master, music, and sound effecs are now in game options.
* Controls: Gamepad controls have been re-enabled.
* Networking: Network play has been added.
* Engine: Fast falling has been implemented.
* Engine: Game pausing has been added. Pausing will not work in networked games.
* Engine: A player controlled free-pan/rotate camera has been added while the game is paused.
* Engine: Shielding has been implemented.
* Engine: Tilt attacks have beem implented.
* Engine: Smash attacks have been implemented (though may be very buggy right now).
* Engine: Aerial attacks have been implemented.
* Engine: Rolling and sidestepping has been implemented.
* Engine: Damage via hitboxes has been implemented.
* Engine: Knockback via hitboxes has been implemented.
* Engine: Hitstun via hitboxes has been implemented (Partial/WIP).
* Misc: A game reset effect has been added. Press Ctrl + R from any point to return to the stage select  menu.
* Misc: A screenshot button has been added. Press F12 to take a screenshot. Saves to the game's directory.
* Launcher: A automatically updating launcher has been added to the game, will try to keep the game as up  to date as possible.
* Localization: Game will automatically localize itself to the user's current system language.
* Localization: Added German as an available game language.
* Localization: Added English as an available game language.
* Localization: Added Spanish as an available game language.
* Localization: Added French as an available game language.
* Localization: Added Croatian as an available game language.
* Localization: Added Hungarian as an available game language.
* Localization: Added Italian as an available game language.
* Localization: Added Japanese as an available game language.
* Localization: Added Korean as an available game language.
* Localization: Added Dutch as an available game language.
* Localization: Added Polish as an available game language.
* Localization: Added Portuguese as an available game language.
* Localization: Added Russian as an available game language.
* Localization: Added Serbian as an available game language.
* Localization: Added Thai as an available game language.
* Localization: Added Chinese (Simplified) as an available game language.
* Localization: Added Chinese (Traditional) as an available game language.

### Known Issues:
* ([#152](https://github.com/HouraiTeahouse/FantasyCrescendo/issues/152), [#153](https://github.com/HouraiTeahouse/FantasyCrescendo/issues/153)) Exiting ledge grabbing is extremely buggy. Climbing, falling, and attacking out of a ledge can lead 
  to unexpected behavior.
* UI: No character select screen is available: only one character is available currently.
* ([#144](https://github.com/HouraiTeahouse/FantasyCrescendo/issues/144)) UI: No custom network lobby UI is currently available. The debug UI is used instead. 
* ([#104](https://github.com/HouraiTeahouse/FantasyCrescendo/issues/104)) Youmu: Neutral combo is not implemented yet.
* Youmu: Special attacks are not implemented yet.
* ([#105](https://github.com/HouraiTeahouse/FantasyCrescendo/issues/105)) Youmu: Some tilt attacks do not have animations yet.
* Youmu: Still missing a few other animations, several of her states copy other animations as placeholders.
* ([#84](https://github.com/HouraiTeahouse/FantasyCrescendo/issues/84)) Youmu: Some animations have a broken facial animations making Youmu look like a goblin in these cases.
* ([#74](https://github.com/HouraiTeahouse/FantasyCrescendo/issues/74)) Engine: Ledge guarding is not implemented yet. Multiple players can grab the same ledge
* ([#213](https://github.com/HouraiTeahouse/FantasyCrescendo/issues/213)) Engine: Smash attacks cannot currently be charged.
* Attacks are currently purely visual. They do not spawn hitboxes yet.
* ([#178](https://github.com/HouraiTeahouse/FantasyCrescendo/issues/178)) Engine: Shielding via keyboard controls seems to be broken for select individuals.
* ([#190](https://github.com/HouraiTeahouse/FantasyCrescendo/issues/190)) Engine: Tapping down may occasionally cause a delayed jump.
* ([#151](https://github.com/HouraiTeahouse/FantasyCrescendo/issues/151)) Engine: Force falling through soft platforms always triggers immediate fast falling.
* ([#101](https://github.com/HouraiTeahouse/FantasyCrescendo/issues/101)) Networking: Non-host players in a networked match may occasionally lose multiple lives upon dying.
* Network: Network animation synchronization is very simple right now. Remote player's animations may be very jerky or laggy.

##Technical Patch Notes:
###New Additions:
* Added dependency on HouraiOptions
* Debug builds will have an additional Debug Stage added to the stage select screen.
* A debug console has been added. Use F5 to toggle it. (Available anywhere in game).
* A hitbox display effect (shows character hitboxes) has been added. Use F11 to toggle it.
* Unity Performance Reporting has been enabled. All exceptions will be reported via this service to help 
  further develop the game.
* Unity Cloud Build has been enabled and integrated into the game's deployment system. Builds from Unity 
  Cloud Build will be immediately available under the "Development" branch in the Launcher.
* Unity Multiplayer Service has been enabled. The game now utilizes the matchmaking and relay services to 
  assist in building internet based network matches. LAN matches do not go through this networking layer.
* CloudFlare has been added as an intermediate caching layer for the deployment system.
* Added dependency on Memes