# QCEnabler
This mod provides assets (one prefab and two config files) and used the code that is shipped with the game to enable an ingame debug console.

## What is the Quantum Console
Quantum Console is a asset from the Unity Assetstore that is shipped with the game but apparently not used.

[Documentation](https://www.qfsw.co.uk/docs/QC/articles/quickstart/quickstart.html)

## Usage
- To open or close the console you press F11 or your configured button.
- Change Configurations using the [Configurator](https://github.com/erkle64/Configurator) mod or modify the config file directly.

Currently the command scanning functionalty is disabled. The way QC is scanning for the Commands in the loaded Assemblies throws Exceptions because of something with Mscorlib.
You can add commands with QuantumConsoleProcessor.TryAddCommand and your constructed commanddata object or you use my CommandExtensions.AddCommand Method by reference the dll of the mod.

For a list of commands available, you can type "commands" (without quotes). You dont need to add a slash infront of the commands. The commands are case sensitive.

With tab you can auto-complete commands and show syntax of command.

You can use the console in the Main Menu. Commands that are need a World/Savegame context like for example teleporting (not included in this mod) should check for the right environment before executing. This is not done by the console.

![Picture of Quantum console in Foundry's Main Menu](https://github.com/ldgkrey/foundrygamemods_qcenabler/assets/25432637/9d298c68-3520-4660-a316-5b608e340bcb)

## Installation
- Extract the zip from the [Release](https://github.com/ldgkrey/foundrygamemods_qcenabler/releases) page into your Mods folder.
- You need to also install the [Unfoundry](https://github.com/erkle64/Unfoundry) mod.

## Todo
- Investigate what part of the scanning throws the error and try to fix it.
