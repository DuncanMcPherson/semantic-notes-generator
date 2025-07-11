# SemanticRelease.NotesGenerator

[![NuGet](https://img.shields.io/nuget/v/SemanticRelease.NotesGenerator.svg)](https://www.nuget.org/packages/SemanticRelease.NotesGenerator/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A .NET Standard library for generating release notes based on semantic commit messages. This project is inspired by the [semantic-release](https://github.com/semantic-release/semantic-release) ecosystem for Node.js.

## Overview

SemanticRelease.NotesGenerator is a plugin for the SemanticRelease system that automatically generates formatted release notes from your git commit history. It categorizes commits based on conventional commit types and creates a well-structured markdown document.

## Features

- Generates formatted markdown release notes
- Categorizes commits by type (features, bug fixes, etc.)
- Supports breaking changes detection
- Links commits to their GitHub URLs
- Integrates with the SemanticRelease lifecycle

## Installation

Install the package from NuGet:

```bash
dotnet add package SemanticRelease.NotesGenerator
```

## Usage

This library implements the `ISemanticPlugin` interface from the SemanticRelease.Abstractions package, allowing it to be used within the SemanticRelease ecosystem.

```csharp
using SemanticRelease.NotesGenerator;

// Register the plugin with the SemanticRelease lifecycle
var notesGenerator = new NotesGenerator();
lifecycle.RegisterPlugin(notesGenerator);
```

## Commit Types

The NotesGenerator recognizes the following commit types:

- `feat`: Features (new functionality)
- `fix`: Bug Fixes
- `docs`: Documentation changes
- `refactor`: Code Refactoring
- `perf`: Performance Improvements
- `test`: Tests
- `chore`: Chores (maintenance tasks)
- `!`: Breaking Changes (as a suffix to a type, or via the string "BREAKING CHANGES" in the commit)

## Example Output

```markdown
## ([1.2.0](https://github.com/user/repo/compare/1.1.0...1.2.0)) (07/11/2025)

### Features

* Add support for custom commit types - 2025 July 11 - ([a1b2c3d](https://github.com/user/repo/commit/a1b2c3d))

### Bug Fixes

* Fix issue with breaking changes detection - 2025 July 10 - ([e4f5g6h](https://github.com/user/repo/commit/e4f5g6h))
```

## Requirements

- .NET Standard 2.1
- LibGit2Sharp (0.31.0)
- SemanticRelease.Abstractions (1.1.0)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Inspiration for this project comes from the [node-semver](https://github.com/npm/node-semver) package in the Node.js ecosystem
- Thanks to the contributors of LibGit2Sharp for providing .NET bindings to libgit2