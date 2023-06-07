# Limbo Iddqd

**Limbo Iddqd** is a package that provides various developer oriented improvements for the Umbraco backoffice. The purpose of the package is somewhhat similar to that of the excellent [**Diplo God Mode** package](https://marketplace.umbraco.com/package/diplo.godmode) by Dan Diplo. **Limbo Iddqd** isn't supposed to replace the **Diplo God Mode** package, but more focus on some of the areas that **Diplo God Mode** doesn't cover. Coincidentally **iddqd** is the Doom cheat code for invulnerability - also known as *god mode*.

As of now, **Limbo Iddqd** provides the following improvements:

- [**Examine content app for content and media**](#examine-content-app)  
Shows how a content or media item is indexed in the **ExternalIndex** and **InternalIndex** indexes, as well as an option to re-index a node for a specific index.

- [**Info content app for content types**](#info-content-app)  
Add a new **Info** content app to all content types with a bit of information about the content type - eg. numeric ID and GUID key.

- [**List of registered property editors**](#property-editors)  
Shows a list of all property editors registered in Umbraco - either from `package.manifest` files or from C#.






<br /><br /><br />

## Features

### Examine dashboard

The package adds a new content app for both content and media showing how the node is indexed in Examine, and an option for re-indexing the node in each index. Supported indexes are the **ExternalIndex** and **InternalIndex** indexes, and optionally also the **PDFIndex** if the [**UmbracoExamine.PDF** package](https://github.com/umbraco/UmbracoExamine.PDF) is installed.

![image](https://github.com/abjerner/Limbo.Umbraco.Iddqd/assets/3634580/f659dd33-4b0c-4d98-acfb-64f5a68c2a10)

### Info content app

As a developer it's often relevant to know the GUID key of a specific content type, but this isn't directly available through the default UI of the backoffice. **Limbo Iddqd** introduces a new **Info** content app that shows various information about the content type being shown - eg. the GUID key.

![image](https://github.com/abjerner/Limbo.Umbraco.Iddqd/assets/3634580/72608d49-3dae-46f7-9750-8d890013d3e3)

### Property editors

The list is available through the **Iddqd** tree in the **Settings** section. It exposes various information about each property editor, and also shows a warning if two property editors are registered with the same alias.

![image](https://github.com/abjerner/Limbo.Umbraco.Iddqd/assets/3634580/586a5be8-f360-407f-b080-0b509d15f92c)
