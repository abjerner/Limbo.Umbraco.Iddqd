# Limbo Iddqd

 [![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md) [![NuGet](https://img.shields.io/nuget/vpre/Limbo.Umbraco.Iddqd.svg)](https://www.nuget.org/packages/Limbo.Umbraco.Iddqd) [![NuGet](https://img.shields.io/nuget/dt/Limbo.Umbraco.Iddqd.svg)](https://www.nuget.org/packages/Limbo.Umbraco.Iddqd) [![Umbraco Marketplace](https://img.shields.io/badge/umbraco-marketplace-%233544B1)](https://marketplace.umbraco.com/package/limbo.umbraco.iddqd)



**Limbo Iddqd** is a package that provides various developer oriented improvements for the Umbraco backoffice. The purpose of the package is somewhhat similar to that of the excellent [**Diplo God Mode** package](https://marketplace.umbraco.com/package/diplo.godmode) by Dan Diplo. **Limbo Iddqd** isn't supposed to replace the **Diplo God Mode** package, but more focus on some of the areas that **Diplo God Mode** doesn't cover. Coincidentally **iddqd** is the Doom cheat code for invulnerability - also known as *god mode*.

As of now, **Limbo Iddqd** provides the following improvements:

- [**Examine content app for content and media**](#examine-content-app)  
Shows how a content or media item is indexed in the **ExternalIndex** and **InternalIndex** indexes, as well as an option to re-index a node for a specific index.

- [**Info content app for content types**](#info-content-app)  
Add a new **Info** content app to all content types with a bit of information about the content type - eg. numeric ID and GUID key.

- [**List of registered property editors**](#property-editors)  
Shows a list of all property editors registered in Umbraco - either from `package.manifest` files or from C#.





<br /><br />

## Installation

The Umbraco 10+ version of this package is only available via [**NuGet**](https://www.nuget.org/packages/Limbo.Umbraco.Iddqd/1.0.0). To install the package, you can use either .NET CLI:

```
dotnet add package Limbo.Umbraco.Iddqd --version 1.0.0
```

or the older NuGet Package Manager:

```
Install-Package Limbo.Umbraco.Iddqd -Version 1.0.0
```




<br /><br />

## Features

### Examine dashboard

The package adds a new content app for both content and media showing how the node is indexed in Examine, and an option for re-indexing the node in each index. Supported indexes are the **ExternalIndex** and **InternalIndex** indexes, and optionally also the **PDFIndex** if the [**UmbracoExamine.PDF** package](https://github.com/umbraco/UmbracoExamine.PDF) is installed.

![image](https://github.com/abjerner/Limbo.Umbraco.Iddqd/assets/3634580/f7e5a5d6-3f97-481f-91e4-e4b77981d7a5)

### Info content app

As a developer it's often relevant to know the GUID key of a specific content type, but this isn't directly available through the default UI of the backoffice. **Limbo Iddqd** introduces a new **Info** content app that shows various information about the content type being shown - eg. the GUID key.

![image](https://github.com/abjerner/Limbo.Umbraco.Iddqd/assets/3634580/228f140b-6c6d-4cc3-a6bd-1839626ec905)

### Property editors

The list is available through the **Iddqd** tree in the **Settings** section. It exposes various information about each property editor, and also shows a warning if two property editors are registered with the same alias.

![image](https://github.com/abjerner/Limbo.Umbraco.Iddqd/assets/3634580/0cc622b3-3daa-4e05-a5fd-f8626858f1f2)

