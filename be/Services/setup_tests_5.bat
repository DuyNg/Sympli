@echo off

REM Load configuration
call config.bat

cd %LIBRARY_NAME%

REM Create a directory for the tests project
mkdir %SLN%.%LIBRARY_NAME%.Tests

REM Navigate into the new directory
cd %SLN%.%LIBRARY_NAME%.Tests

REM Initialize a new MSTest project
dotnet new mstest

REM Add references to the Core, Application, Infrastructure, and API projects
dotnet add reference ../%SLN%.%LIBRARY_NAME%.Core
dotnet add reference ../%SLN%.%LIBRARY_NAME%.Application
dotnet add reference ../%SLN%.%LIBRARY_NAME%.Infrastructure
dotnet add reference ../%SLN%.%LIBRARY_NAME%.API

REM Add Moq package for mocking in unit tests
dotnet add package Moq

REM Add the project to the parent SLN
dotnet sln ../../../%SLN%.sln add .
