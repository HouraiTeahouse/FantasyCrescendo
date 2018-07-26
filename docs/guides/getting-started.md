# Getting Started
Before attempting to make any contributions or work with the game at all, we suggest all contributors join the [Hourai Teahouse Discord](https://discord.gg/VuZhs9V).

## Installing Tools
Fantasy Crescendo is a video game, pulling together multiple creative and technical disciplines. We use a variety of tools to finish the necessary tasks.

### For Game Designers/Programmers
Fantasy Creacendo is developed with the Unity3D engine. If you plan on directly changing game assets or code, you will need Unity 2018.2. It's suggested to use the [Unity Hub](https://forum.unity.com/threads/unity-hub-preview-0-18-1-is-now-available.539353/) to manage and install Unity.

To also get the project files and push changes, you will also need a **Git client**. There's multiple choices for this: [git bash](https://git-scm.com/) (Command-line), [SourceTree](https://www.sourcetreeapp.com/) (GUI), [GitHub Desktop](https://desktop.github.com/) (GUI). Any git client works, choose one that you feel you work with best.

You will also need a GitHub account. If you do not have one already, [register](https://github.com/join).

For programmers, while it is possible to work with any program that edits text files (vim, Sublime, Notepad++), it is suggested that you use an editor that supports language specific features for C#. Some suggestions for this are [Visual Studio Code](https://code.visualstudio.com/), [Visual Studio](https://visualstudio.microsoft.com/), or [Rider](https://www.jetbrains.com/rider/).

### For Animators
The source files used to animate characters in Fantasy Crescendo is [Autodesk Maya](https://www.autodesk.com/products/maya/overview). Specifically, we use **Autodesk Maya 2016 Extension 2**. 

### For Localizers
Localizers do not need to install anything on their computers, but they will need access to the shared [localization Google Sheet](https://docs.google.com/spreadsheets/d/10FN34rosfGUNap72iH8PPLmM2yZ8_Wv_cFdtXx2IsTk/edit?usp=sharing). If you wish to localize a language, request access, then contact the project maintainers on the Hourai Teahouse Discord to get the request approved.

### For 2D Artists/Musicians
Luckily for most of these assets used in the game, we do not have any strong requirements so long as the end asset importable by Unity. We suggest you use whatever tool you find most adequate.

## Development Enviroment Setup
This part only pretains to those directly working with files on the repo, namely game designers and programmers. **NOTE:** Fantasy Crescendo, as of writing, currently depends on using many **git
submodules** to manage dependencies. This is a suboptimal solution for
dependency management, but is currently the only available choice we have, even
if it complicates the setup process for the project. Sometime in the future we
will be migrating to the [Unity Package
Manager](https://docs.unity3d.com/Packages/com.unity.package-manager-ui@1.8/manual/index.html)
when it supports 3rd party remote packages, which will remove the need to manage
these submodules.

### Setting up remote repo

If you are a member of the Hourai Teahouse GitHub organization, you are allowed
to directly commit to the official repository, skip this step.

If you are an external contributor, it is suggested that you
[fork](https://help.github.com/articles/fork-a-repo/) the repo onto your own
account.

### Setting up your local files

To get the local files, you'll need to **git clone** the remote repository (the
official one or the fork you have created).

**Via GitHub for Desktop**

TODO(Gabo7): Document.

**Via Command Line**

This assumes you are using a \*nix (Linux/macOS) style terminal. Windows users
are suggested to use either [Windows Subsystem for
Linux](https://docs.microsoft.com/en-us/windows/wsl/install-win10) (requires
Windows 10) or [GitBash](https://git-scm.com/downloads) 

```bash
git clone https://github.com/HouraiTeahouse/FantasyCrescendo # Clone the
repository
cd FantasyCrescendo # Change directory to the cloned repo
git submodule init --recursive
```

### Loading into the Unity Editor
Simply open the cloned directory with the Unity Editor. It will ask to import a large number of files (this may take longer on less powerful development machines).

## Common Development Operations

### Running Unit Tests
In the top menu bar, Window > General > Test Runner will bring up the **Test Runner Window**. Press "Run All". This will run all unit tests defined in the project. Generally all of these tests should pass for every code change pushed to the repository. The continuous integration system that creates builds will reject any change that causes unit tests to fail.

### Building Asset Bundles
In the top menu bar, "Hourai Teahouse > Build Asset Bundles (<Your OS>)" will build all Asset Bundles in the project. This may take some time for the first you do it.

### Playing In-Editor
Make sure you have built Asset Bundles before doing this step (Errors will occur without them). Load up a common test scene to make sure that the game starts correctly. In the Project Window, search for and open the "DebugStage" scene. If you enter Play Mode (press the Play button at the top of the editor), it should load up the game, and a number of test players. It should play out just like any other stock match, but within the Editor.

### Creating a Build
Make sure you have built Asset Bundles before doing this step (Errors will occur without them). In the top menu bar, File > Build Settings, this will open the build window. Select the approriate build target for the operating system you are on. Click either of the Build options (Build and Run will run the build on completion).

### Deploying a Build
Hourai Teahouse employs a continuous integration and continuous deployment pipeline upon source control changes. If you make changes and get them integrated into the official repository's `master` and `develop` branches, it will automatically trigger builds to be created. If builds for all platforms (Windows, macOS, and Linux) succeed, and they all pass defined unit tests, it will be deployed to Steam. Please note, the processes for creating game builds can take quite a while, it may take up to 1-3 hours before a build is completed and deployed.

The `develop` branch is configured to deploy only to the `latest-beta` beta release line on Steam, changes in the default release line must be manually made by a Hourai Teahouse administrator.
