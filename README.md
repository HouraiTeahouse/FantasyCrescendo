![logo](./Assets/Assets/Sprites/logo.png)
<p align="center">
    <a href="https://github.com/HouraiTeahouse/FantasyCrescendo/blob/develop/LICENSE">
        <img src="https://img.shields.io/github/license/HouraiTeahouse/FantasyCrescendo.svg" alt="License">
    </a>
    <a href="https://houraiteahouse.github.io/FantasyCrescendo-Docs/">
        <img src="https://img.shields.io/badge/docs-passing-brightgreen.svg" alt="Docs">
    </a>
    <a href="https://discordapp.com/invite/VuZhs9V">
        <img src="https://discordapp.com/api/guilds/151219753434742784/widget.png" alt="Join the Hourai Teahouse Discord Chat">
    </a>
</p>

An Experimental Rewrite of Fantasy Crescendo from the ground up to support
various types of net-enabled gameplay.

## About
*Fantasy Crescendo* is a collaborative community project to create a *[Touhou Project](https://en.touhouwiki.net/wiki/Touhou_Project)* fangame akin to that of Super Smash Bros.

This project does not only consist of code contributions, and many of the assets included are custom art, music, sound effects, and 3D designs created by community submissions. For a full list of credits, see the [credits](./CREDITS.md) file in the repository.

## Setup
This project uses git submodules extensively to manage remote dependencies 
(due to a general lack of a user-facing package manager for Unity3D).To 
properly clone the entire project use `git clone --recursive ...` to check 
out all submodules and dependencies. If using an older version of git or to
check out and initialize in a normally cloned repo, use the following command:
`git submodule update --init --recursive`. This command is also used to update
submodules that may have been updated by remote changes.

## Contributing
This is an open community driven  project. Contributions are very welcome. 
Code based contributions can be received and reviewed publicly on this repository. 
Non-code assets like BGM or 3D models are best sent through and collaborated 
on in our development Discord server, linked above. For more information, 
please read [CONTRIBUTING.md](./.github/CONTRIBUTING.md).

## License
First and foremost, *Fantasy Crescendo ~ Rumble Dream Ensemble* is a derivative
of Touhou project. Thus, we ask that any redistirbution or derivative of this
project adhere to the guidelines created by ZUN, 
[viewable in English here](http://en.touhouwiki.net/wiki/Touhou_Wiki:Copyrights).

Furthermore, original *Fantasy Crescendo* content is licensed under two seperate 
liscenses depending what content is in question:  
- The software (all text files) are under the Version 2 of the GPL or any later 
  version. See [LICENSE](./LICENSE) for more information.
- The content, everything else, is evaluated on a per item basis. Please contact 
  the original creator before reproducing or editing any of the game assets.