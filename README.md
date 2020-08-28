# UoN.ZipBuilder

[![License](https://img.shields.io/badge/licence-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![Build Status](https://travis-ci.org/uon-nuget/UoN.ZipBuilder.svg?branch=master)](https://travis-ci.org/uon-nuget/UoN.ZipBuilder)
[![NuGet](https://img.shields.io/nuget/v/UoN.ZipBuilder.svg)](https://www.nuget.org/packages/UoN.ZipBuilder/)

## What is it?

A library for creating zip files using the builder pattern.

It wraps [SharpZipLib] for its compression functionality, since that's more fully featured than `System.IO.Compression`.

## What are its features?

- It provides a Builder Pattern "Fluent API" (i.e. with method chaining) for creating compressed archives.
- It supports PKZIP 2.0 and Winzip AES encryption.
- Use of Zip64 is configurable.
- Shallow directories of files (non-recursive) can be added quickly and easily
- Text content can be written directly to archive entries.
- Bytes can be written directly to archive entries.
- It currently only supports **Zip** archives, though support for other archives through [SharpZipLib] should be trivial.
- It currently only supports creating archives **In Memory**, but can output the results to a byte array for use as you see fit.

In short, within its restricted confines, it provides a **nice interface** for creating **basic zip files**.

## Dependencies

The library targets `netstandard1.3` and depends upon [SharpZipLib] 1.x.

## Reference

### `CreateZipStream()`

Initialises an archive with an underlying `MemoryStream`.

### `DisableZip64()`

Prevents the archive ever using the `Zip64` format.

Offers better compatibility, but prevents files being larger than 4GB.

### `UseEncryption()`

Turns on encryption for all entries in the archive.

A password **must** be provided, and will be required to decrypt and extract any files from the archive.

Supported Encryption formats are:

- PKZip 2.0 ("Classic")
    - supported by older clients including **Windows Compressed Folders**
    - VERY WEAK. Should not be used unless wide compatibility is required.
- WinZip AES with **128-bit** or **256-bit** keys.
    - 256-bit is better, and the default, but doesn't work in some environments (pending a [SharpZipLib] fix).

### `AddFile()`

Adds a physical file (at a given path) to the archive, with the specified entry path.

### `AddTextContent()`

Adds string content directly to a new archive entry at the specified entry path.

### `AddBytes()`

Adds a byte array directly to a new archive entry at the specified entry path.

### `AddDirectoryShallow()`

Adds the top level content of a physical directory (at a given path) to the arhive at the specified entry path, preserving filenames.

### `AsByteArray()`

Closes out the archive, rendering the Builder complete, and returns the data of the built archive as a byte array.

## Usage

The important thing to consider about usage of the library is that there is an order of usage.

There are four stages to building a zip, and the methods provided by the ZipBuilder each fit into a given stage:

1. Initialisation
    - `CreateZipStream()`
2. Configuration
    - `DisableZip64()`
    - `UseEncryption()`
3. Content
    - `AddFile()`
    - `AddTextContent()`
    - `AddDirectoryShallow()`
4. Output
    - `AsByteArray()`

### Initialisation

Only one initialisation method should be used, and once used the builder has an active underlying archive to "build".

### Configuration

Any configuration options can be provided, but they must be performed before any Content is added, at this time.

Safe mixing and matching of file encryption, for example, may be possible in future, if it becomes a desirable feature.

### Content

Add entries to the archive.

### Output

Close out the archive and get its data in a given output format.

This is the `Build()` step typical of most other Builder Pattern APIs, and it functions the same - the Builder has served its purpose and is no longer usable once an Output method has been called.

## Examples

### Password protected physical file

```csharp
using UoN.ZipBuilder;
using System.IO; //for File

var zipBytes = new ZipBuilder()
    .CreateZipStream() // initialise the archive
    .UseEncryption("password", Encryption.Classic) // use PKZIP 2.0 password protection - weak, widely supported
    .AddTextContent("hello there", "hello.txt"); // add some text to a `.txt` file in the zip
    .AsByteArray(); // close out the archive, getting its contents as a byte array

File.WriteAllBytes("test.zip", zipBytes); // write the bytes to a physical file
```

### File Download from ASP.NET Core

```csharp
// in a controller somewhere
[HttpGet]
public IActionResult GetZipFile()
{
    ...
    
    // start building the zip
    var zipBuilder = new ZipBuilder()
        .CreateZipStream()
        .DisableZip64() // we want older Zip clients to work
        .AddTextContent(jsonData, $"{fileName}.json");

    // conditionally add further content
    if(config.AddImagesDirectory)
        zipBuilder = zipBuilder.AddDirectoryShallow(baseImagePath, "images");

    // get the data for the complete zip file
    var zip = zipBuilder.AsByteArray();

    return File(zip, "application/zip", fileName);
}
```

## Building from source

We recommend building with the `dotnet` cli, but since the package targets `netstandard1.3` and depends only on [SharpZipLib] 1.x, you should be able to build it in any tooling that supports those requirements.

- Have the .NET Core SDK 2.0 or newer
- `dotnet build`
- Optionally `dotnet pack`
- Reference the resulting assembly, or NuGet package.

## Contributing

Contributions are welcome, if you want to expose any further [SharpZipLib] functionality via this interface or otherwise improve things around here.

If there are issues open, please feel free to make pull requests for them, and they will be reviewed.

[SharpZipLib]: https://github.com/icsharpcode/SharpZipLib
