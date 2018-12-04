# Puppetry
It is a  framework for automated testing of Games written in Unity3d game engine

Link to wiki with the documentation.
https://github.com/GameUnitLab/Puppetry/wiki


## Puppeteer
Client lib of the framework
Install nuget package of Puppetry.Puppeteer (it is shared nuget pages that located on nuget.org) to your test solution or add reference to Puppeteer.dll (that can be builded by yourself).

## PuppetDriver
Proxy server that joins Puppeteer and UnityPlugin/Puppet
Run up published src of PuppetDriver by dotnet. Example: dotnet //pathToPublishedSolution/PuppetDriver.dll or build it by yourself and run it

## UnityPlugin/Puppet
Plugin for Unity to communicate with PuppetDriver and emulate interaction with a Game
Copy Puppet folder from UnityPlugin and add it to Game's code base inside Asset folder. Note: Game should be launched after PuppetDriver is started as Puppet is connecting to PuppetDriver

Additional: you can use UnityPlugin/Puppet in Editor and Native Game. To use Puppet in Game, you should add GameObject with component "InGameApiClientLoader" to your scene. InGameApiClientLoader will set up communication between the Game and PuppetDriver
