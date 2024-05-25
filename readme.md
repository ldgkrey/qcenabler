# QCEnabler
This mod provides assets (one prefab and two config files) and used the code that is shipped with the game to enable an ingame debug console.

## What is the Quantum Console
Quantum Console is a asset from the Unity Assetstore that is shipped with the game but apparently not used.

[Documentation](https://www.qfsw.co.uk/docs/QC/articles/quickstart/quickstart.html)

## Usage
- To open or close the console you press F11.<br />
- The console is ready to open after the main menu is loaded.<br />
- While ingame you might need to unlock the mouse. Opening the crafting or inventory menu should suffice.<br />

Currently the command scanning functionalty is disabled. The way QC is scanning for the Commands in the loaded Assemblies throws Exceptions because of something with Mscorlib.
You can add commands with QuantumConsoleProcessor.TryAddCommand and your constructed commandata object or you use my CommandExtensions.AddCommand Method by reference the dll of the mod.

## Installation
Extract the zip. Into your Mods folder.

## Todo
- KeyConfiguration
- Investigate what part of the scanning throws the error and try to fix it.
