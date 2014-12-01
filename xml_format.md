

RWizard::RAppMenu
=================

*RAppMenu* is an application written to automate the task of creating the so called **applications** in RWizard. These are menus capable of launching functions in R, for example.

For the rest of the document, it is assumed that the file is called *example.xml*. Also note that, though tags appear in a mixed upper and lower case, the XML engine of RAppMenu is case insensitive.

Format of the XML for the applications Menu
-------------------------------------------

Root of the XML document is *menue*. This tag does not have a corresponding **name** attribute, in contrast to all other tags in this XML format. The name of the menu is taken from the name of the file. There are no available attributes.

**Syntax**
```HTML
<Menue>
```

**Example**
```HTML
<Menue>
</Menu>
```

**Shown as**
```
example ->
```

Menus
-----

This is probably the second most important tag in the whole file. It allows to nest menus inside menus. 

**Syntax**
```HTML
<MenuEntry Name="name_of_menu_entry">
```

**Example**
```HTML
<Menue>
  <MenueEntry Name="Geometric">
  </MenueEntry>
</Menue>
```

**Shown as**
```
example -> Geometric ->
```

PDF File
--------

*PDF file* entries are a way to launch a PDF viewer for a given PDF document.

**Syntax**
```HTML
<PDF Name="name_of_PDF_file">
```

**Example**
```HTML
<Menue>
  <MenueEntry Name="Geometric">
  </MenueEntry>
  <PDF Name="geometric_functions.pdf"/>
</Menue>
```

**Shown as**
```
example -> Geometric ->
           geometric_functions.pdf
```

Separators
---------

Separators make it possible to divide the menu in parts, specially when a group of options are not really correlated to another group of options.

**Syntax**
```HTML
<Separator />
```

**Example**
```HTML
<Menue>
  <MenueEntry Name="Geometric">
  </MenueEntry>
  <Separator />
  <PDF Name="geometric_functions.pdf" />
</Menue>
```

**Shown as**
```
example -> Geometric ->
           ---------
           geometric_functions.pdf
```

Functions
---------

Functions are central for RWizard Applications. This allows to call existing functions from within RWizard, with a set of parameters and options.

Functions normally need an enclosing menu entry, unless they are at top level, i.e., they directly hanging from *menue*.

**Syntax**
```HTML
<MenuEntry Name="function_name">
<Function Name="function_name"
	[HasData="true/false"]
	[RemoveQuotationMarks="true/false"]
	[DataHeader="true/false"]
	[DefaultData="value"]
	[StartColumn="1"]
	[EndColumn="1"]
	[PreCommand="value"]>

	[<ExecuteOnce Name="value" />]*
	
	[<RequiredArgument Name="value" />]*
	
	[<Argument Name="value"
		[Tag="value"]
		[AllowMultiselect="true/false"]
		[DependsFrom="value"]
		[Viewer="DataColumnsViewer/DataValuesViewer/Map/TaxTree"]*
	/>]*
	
	[<FunctionArgument Name="argument_name"
			   Function="function_name"
			   Variant="\method{boxplot}{default}">
			 
			 [<SetArgument Name="argument_name" Value="value"/>]*
	</FunctionArgument>]

</Function>
</MenuEntry>
```

- **Arguments** are normal arguments.
- **RequiredArguments** are arguments that cannot be missing.
- **FunctionArguments** are arguments that imply a call to another function.

**Example**
```HTML
<Menue>
  <MenueEntry Name="Numeric">
  	<MenueEntry Name="pow">
  	<Function Name="pow">
  		<RequiredArgument Name="x" />
  		<Argument Name="y" />
  	</Function>
  	</MenuEntry>
  	<MenueEntry Name="sqrt">
  	<Function Name="sqrt">
  		<RequiredArgument Name="x" />
  	</Function>
  	</MenuEntry>
  </MenueEntry>
  <MenueEntry Name="Geometric">
  	<MenuEntry Name="sin">
  	<Function Name="sin">
  		<RequiredArgument Name="x" />
  	</Function>
  	</MenuEntry>
  	<MenuEntry Name="cos">
  	<Function Name="cos">
  		<RequiredArgument Name="x" />
  	</Function>
  	</MenuEntry>
  </MenueEntry>
  <Separator />
  <PDF Name="numeric_functions.pdf" />
  <PDF Name="geometric_functions.pdf" />  
</Menue>
```

**Shown as**
```
example -> 
	   Numeric -> pow
	   	      sqrt
	   Geometric -> sin
	                cos
           ---------
           geometric_functions.pdf
```

Graphic menus
-------------

These are submenus which show an image for each function.

**Syntax**
```HTML
<MenuEntry Name="menu_name" ImageWidth="16..250" ImageHeight="16..250" MinNumberColumns="1">
	<MenuEntry Name="function_name" ImagePath="/path/to/file.png" ImageTooltip="help">
		<Function Name="function_name" ...>
		</Function>
	</MenuEntry>
</MenuEntry>
```

**Example**
```HTML
<MenuEntry Name="Graph menu of numeric functions" ImageWidth="64" ImageHeight="64">
	<MenuEntry Name="pow" ImagePath="pow.png" ImageTooltip="powers a given base to an exponent">
		<Function Name="pow">
			<RequiredArgument Name="base">
			<Argument Name="exponent">
		</Function>
	</MenuEntry>
	<MenuEntry Name="sqrt" ImagePath="sqrt.png" ImageTooltip="square root">
		<Function Name="sqrt">
			<RequiredArgument Name="x">
		</Function>
	</MenuEntry>
</MenuEntry>
```
