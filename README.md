RAppMenu
========

Application's menu editor for RWizard

Usage
-----

The source code is written in C#, for .NET 4.0, with the following references:


- microsoft.csharp.dll
- system.dll
- system.xml.dll
- system.drawing.dll
- system.windows.forms.dll


The *Res* folder holds the binary files for icons. These files must be chosen to be embedded in the project.

Running
-------

It's enough to create an instance of the main window class and show it.
- The *ApplicationsFolder* string property can be written in order to store the path to the folder in which the xml files with the menu descriptions are stored. It defaults to "applications".
- The *PdfFolder* string property can be written in order to store the path to the folder in which the PDF files are stored. It defaults to "PDF".
- The *GraphFolder* string property can be written in order to store the path to the folder in which the image files are stored. It defaults to "Graph".
