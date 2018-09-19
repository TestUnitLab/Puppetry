# Puppetry
It is a MVP of framework to test automatically Games


# How to set up the framework:

# 0. PuppetContracts
It's shared library with contracts that is used in Puppeteer and PuppetDriver solutions

Preparation:
a) Open and build PuppetContract solution

# 1. Puppeteer:
It's client-side library that should be used in Testing Framework

Preparation:
a) Open Puppetter.sln and build the project
b) Add refence of the Puppeteer.dll to your test solution
c) Use Puppetry.Puppeteer namespace in your tests

# 2. UnityPlugin/Puppet
It's plugin for UnityEditor

Preparation;
a) Copy Puppet folder to your Unity application code base, under Asset folder
b) Launch Unity Editor with the application

# 2. PuppetDriver
It's a proxy system that conects our tests (Puppeteer) and UnityEditor itself

Preparation:
a) Open and build PuppetDriver solution
b) Run it up

alternative

a) use dotnet commands and run the solution
Example:
dotnet run /path_to_the_project/PuppetDriver/PuppetDriver/bin/$Configuration$/netcoreapp2.0/PuppetDriver.dll

# Example of test
Look at Puppeteer/PuppetTesting/PuppetTesting.cs 
It represent an example of tests
