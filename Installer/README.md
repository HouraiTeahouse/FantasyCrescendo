#Installers

This folder contains configuration files and scripts for creating installer executables for the game.

To build the installer files, the program Inno Setup is needed. Downloadable at http://www.jrsoftware.org/isinfo.php. Be sure to install the included Inno Setup Preprocessor, it is required to compile the installers.

To buildt the installers, build the game first in Unity into the respective Build folders. Then open the \*.iss files in Inno Setup and Build/Run.

Note: Inno Setup requires a large amount of RAM to compress the game's built files. The build process may use up to several gigabytes of RAM at once.
