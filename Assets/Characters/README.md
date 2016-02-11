#Characters

This is the store for all Character related information that *doesn't* need to dynamically loaded. File structure under here is seperated by character name. (i.e. Marisa's assets are kept under the Marisa folder.).
For the files that need to be loaded dynamically, see [Assets/Resources/Characters](../Assets/Resources/Characters/).

The main asset files kept in here are usually the ones shared between all versions of the character, including but not limited to:

* Model (for the character)
* Textures (Materials are loaded dynamically, which statically load the textures needed)
* Animations
* Animation Controllers 