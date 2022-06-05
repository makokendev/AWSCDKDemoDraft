# *WARNING - DRAFT VERSION*

> Solution is based on the [Block Chain Event Stream Processor Coding Challenge Repository](https://github.com/makokendev/BlockChainEventStreamProcessorCodingChallenge)

> **WARNING:_**  This repository is created the showcase AWS CDK however, it is still is in progress. This draft version is only included to provide a sneak peak at the AWS CDK setup + added Cake Build Tasks.



---
---
---
---
---
---
---

# Coding Challenge 

This application is created to address the task described in the [Coding Challenge document](CodingChallenge.pdf).

The task is the following:
> Write a console app in C# that receives some subset of 
transactions, and processes them in such a way that enables the program to 
answer questions about NFT ownership.
Your program must execute only a single command each time it is run, and 
must persist state between runs.

[Coding Challenge document](CodingChallenge.pdf) contains message specifications and commands that the console application must work with.


---

## How to build & run the program?

Make sure that [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) is installed on the computer. 

```
# Linux & MacOS users
./build/build.sh

# Windows users, please run build.ps1
./build/build.ps1

# build script triggers Cake (Frosting) tasks/pipeline. 
# The pipeline will build, (unit&integration) test, publish and run the program with [predefined commands](./build/Tasks/RunConsoleAppCommandsTask.cs). 

```

Build Pipeline publishes CodingChallenge.Console project (entry point for the program) to the "./publish" folder. 

Following command will display help screen that shows available commands.
```
dotnet ./publish/CodingChallenge.Console/CodingChallenge.Console.dll --help

# command response is shown below.
CodingChallenge.Console 1.0.0
Copyright (C) 2022 CodingChallenge.Console

  -i, --read-inline    Reads either a single json element, or an array of json elements representing transactions as an argument.

  -f, --read-file      Reads either a single json element, or an array of json elements representing transactions from the file in the specified location.

  -n, --nft            Returns ownership information for the nft with the given id.

  -w, --wallet         Lists all NFTs currently owned by the wallet of the given address.

  -r, --reset          Deletes all data previously processed by the program.

  --help               Display this help screen.

  --version            Display version information.
```

[Github actions](https://github.com/makokendev/BlockChainEventStreamProcessorCodingChallenge/actions) contains a sample Github Workflow run to demonstrate how the build pipeline output looks like. 

### Build Badge
[![Coding Challenge Build Pipeline](https://github.com/makokendev/BlockChainEventStreamProcessorCodingChallenge/actions/workflows/cakebuild.yml/badge.svg?branch=master)](https://github.com/makokendev/BlockChainEventStreamProcessorCodingChallenge/actions/workflows/cakebuild.yml)

---

## Solution

### Main Folders
This repository consists of 4 main parts: 
* build folder - Build Pipeline
  * Contains Cake Frosting project to create a build pipeline. Next to the build pipeline, an extra task is included to run commands on the console app as a way to demonstrate basic capability of the program.
* src folder - Source Code
  * Contains code libraries that make up the console application. 
  * CodingChallenge.Console project is the Console Application. 
* tests folder - Unit & Integration Tests
  * Contains libraries for conducting unit and integration tests.
* utils
  * Contains bash script file that contains commands to execute console application.

### Github and VSCode folders

.github and .vscode folders contains files related to GitHub and VS Code. These folders are not required for building or running the application. 
* cakebuild.yml under .github is a github workflow contains a call to build.sh script which provides a nice demo on how cake build operates and how the output looks like
* launch.json under .vscode provides settings for debugging the console application using VSCode. It provides a nice basis for VSCode users.

## Architecture
* Jason Taylor's [Clean Architecture](https://github.com/jasontaylordev/CleanArchitecture) solution template is used only as a base for the solution. Please visit the github repository to get more details on Clean Architecture and his solution template setup.
* Such solution template is useful to speed up development process and set a baseline for development teams. James Taylor provides a lot of documentation & videos on Clean Architecture which is great for onboarding new team members.


## Important tools and libraries

* [Cake Build and Code Frosting](https://cakebuild.net/docs/running-builds/runners/cake-frosting)
    * Used for creating the build pipeline. Similar setup can be used for setting up Continuous Integration and Deployments.
* [XUnit](https://xunit.net/) library is used for unit & integration tests. 
* [SQLite](https://www.sqlite.org/index.html) data base used as the database along with [SQLite EF Core Database Provider](https://docs.microsoft.com/en-us/ef/core/providers/sqlite/?tabs=dotnet-core-cli)
    * SQLite is a great self-contained database which is great for self-contained application such as this one.    
* [Fluent Validations](https://docs.fluentvalidation.net/en/latest/) is a great flexible, extendible validation library.
* [CommandLine](https://github.com/commandlineparser/commandline) is a great library to easily parse command line arguments. 

# Quick next steps that can be taken

* Integrate sonarqube -> https://docs.sonarqube.org/latest/
  * For ensuring and improving code quality and code quality
* Introduce webapi and generate docker image (generic and for lambda)
* Create Api Gateway and Lambda using CDK to host the web api

### Sample Input / Output

[samplecommands.sh](./utils/samplecommands.sh) file in utils folder provides set of sample commands. By default, this file targets the published application under "publish/CodingChallenge.Console" folder. 

## Contact 

Please write to makoken@gmail.com for any remarks and questions.
