

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

This is probably the most important tag in the whole file. It allows to nest menus inside menus. 

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

Separators
---------

Separators make it possible to divide the menu in parts, specially when a group of options are not really correlated to another group of options.

```HTML
<Separator />
```

Function arguments

<FunctionArgument Name="BOXPLOT" Function="boxplot" Variant="\method{boxplot}{default}">
		         <ReadOnlyArgument Name="x"/>
		         <ReadOnlyArgument Name="..."/>
		         <ReadOnlyArgument Name="data"/>
   	         <SetArgument Name="xlim" Value="c(0.5,2.5)"/>
		         <SetArgument Name="ylab" Value="Ylab"/>
		         <SetArgument Name="xlab" Value="Xlab"/>
		         <SetArgument Name="col" Value="color"/>
	         </FunctionArgument>
	         <FunctionArgument Name="PAR" Function="PAR">
		         <SetArgument Name="font.lab" Value="2"/>
		         <SetArgument Name="mar" Value="c(5,5,3,2)"/>
		         <SetArgument Name="cex.lab" Value="1.5"/>
            </FunctionArgument>
