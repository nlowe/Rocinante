# Rocinante [![Build Status](https://travis-ci.org/roci-gen/Rocinante.svg?branch=master)](https://travis-ci.org/roci-gen/Rocinante)
A static-site generator built on dotnet core - WIP

## Building
You need a recent dotnet core version, specifically one that supports
`netstandard1.6`, `netcoreapp1.1`, and can build `csproj` style projects

If you are on linux, you will need `mono` for the cake script (I believe they're switching to dotnet core in the future)

Invoke the cake script to build the project and run tests

```bash
./build.sh # or .\build.ps1 on windows
```

## License
This project is licensed under the Apache 2 license. Some plugins might be
licensed with another compatible license.