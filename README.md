# FluentResult

[![build](https://github.com/rosenkolev/result-fluent/actions/workflows/github-actions.yml/badge.svg)](https://github.com/rosenkolev/result-fluent/actions/workflows/github-actions.yml)
[![spell check](https://github.com/rosenkolev/result-fluent/actions/workflows/spell-check.yml/badge.svg)](https://github.com/rosenkolev/result-fluent/actions/workflows/spell-check.yml)
[![codeql analyze](https://github.com/rosenkolev/result-fluent/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/rosenkolev/result-fluent/actions/workflows/codeql-analysis.yml)
[![codecov](https://codecov.io/gh/rosenkolev/result-fluent/branch/main/graph/badge.svg?token=ANXME8CYJP)](https://codecov.io/gh/rosenkolev/result-fluent)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/rosenkolev/result-fluent/blob/main/LICENSE)

<!-- PROJECT LOGO -->
<center>
  <img src="resources/icons/result-icon-128.png" alt="Logo" width="128">
  <h3>FluentResult</h3>
  <p align="center">
    <a href="https://github.com/rosenkolev/result-fluent/issues">Report Bug & Request Features</a>
  </p>
</center>

<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#about-the-project">About The Project</a></li>
    <li><a href="#getting-started">Getting Started</a></li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
  </ol>
</details>

<!-- ABOUT THE PROJECT -->
## About The Project

This is a lightweight .NET library, that can be used for returning and validating Result without relaying on exceptions.

### Features

* Simple `Return` model.
* Allows attributes and model validation.
* Allows easy mapping of models.
* Provides a fluent syntax.
* No external dependencies.

## Getting Started

You can install [ResultFluent with NuGet](https://www.nuget.org/packages/ResultFluent/):

```shell
dotnet add package ResultFluent
```

## Usage

| Package | Link |
| ---------- | ---------- |
| [![nuget](https://img.shields.io/nuget/v/resultfluent.svg)](https://www.nuget.org/packages/ResultFluent/) [![nuget downloads](https://img.shields.io/nuget/dt/resultfluent)](https://www.nuget.org/packages/ResultFluent/) | [**ResultFluent** Usage Guide](/README.CORE.md) |
| [![nuget](https://img.shields.io/nuget/v/resultfluent.mvc.svg)](https://www.nuget.org/packages/ResultFluent.MVC/) [![nuget downloads](https://img.shields.io/nuget/dt/resultfluent.mvc)](https://www.nuget.org/packages/ResultFluent.MVC/) | [**ResultFluent.Mvc** Usage Guide](/README.MVC.md) |

<!-- CONTRIBUTING -->
## Contributing

If you find a bug or have a feature request, please report them at this repository's issues section.

Anyone can submit a pull request and it will be considered.

### Build
- Clone or download the repo
- `restore` to install dependencies
- `build` to build

### Folder Structure
* `FluentResult` - The core Result package source files
* `FluentResult.Mvc` - The Result.Mvc package source files
* `FluentResult.Tests` - The shared unit tests files

## License

Distributed under MIT License. See [LICENSE](/LICENSE) for more information.

## Contact

[Rosen Kolev](https://github.com/rosenkolev) - rosen.kolev@hotmail.com
