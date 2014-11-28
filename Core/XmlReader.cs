using System;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections.Generic;

using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Core {
	static class XmlAttributeCollectionExtension {
		public static XmlNode GetNamedItemIgnoreCase(this XmlAttributeCollection xmlc, string id)
		{
			XmlAttribute toret = null;
			id = id.Trim();

			foreach ( XmlAttribute attr in xmlc ) {
				if ( attr.Name.Equals( id, StringComparison.OrdinalIgnoreCase ) ) {
					toret = attr;
					break;
				}
			}

			return toret;
		}
	}

    public class XmlReader {
        /// <summary>
        /// Initializes a new instance of the <see cref="RAppMenu.Core.XmlReader"/> class.
        /// The constructor does not launch the lecture.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public XmlReader(string fileName)
        {
            this.FileName = fileName;
            this.document = null;
        }

        /// <summary>
        /// Launch the lecture, building the <see cref="DesignOfUserMenu"/>.
        /// </summary>
        public void Read()
        {
            this.document = new DesignOfUserMenu();

            // Open the file
            var docXml = new XmlDocument();
            docXml.Load( this.FileName );

            // Extract the name (it is not in the root node, but in the file name)
            this.document.Root.Name = Path.GetFileNameWithoutExtension( this.FileName );

            // Read the immediate upper level nodes
            this.ReadNodeInto( docXml.DocumentElement, this.document.Root );
        }

        private void ReadNodeInto(XmlNode node, Menu menu)
        {
            // Subnodes of node
            foreach(XmlNode subNode in node.ChildNodes) {
                if ( subNode.Name.Equals( Menu.TagName, StringComparison.OrdinalIgnoreCase ) ) {
					if ( this.IsGraphicMenu( subNode ) ) {
						GraphicMenu.FromXml( subNode, menu );
					} else {
						this.ReadNodeInto( subNode, Menu.FromXml( subNode, menu ) );
					}
                }
				else
				if ( subNode.Name.Equals( PdfFile.TagName, StringComparison.OrdinalIgnoreCase ) ) {
					PdfFile.FromXml( subNode, menu );
				}
				else
				if ( subNode.Name.Equals( Separator.TagName, StringComparison.OrdinalIgnoreCase ) ) {
					Separator.FromXml( subNode, menu );
				}
				else
				if ( subNode.Name.Equals( Function.TagName, StringComparison.OrdinalIgnoreCase ) ) {
					Function.FromXml( subNode, menu );
				}
            }

            return;
        }

		/// <summary>
		/// Determines if the given node has graphic attributes.
		/// </summary>
		/// <returns><c>true</c> if has graphic attributes the specified node; otherwise, <c>false</c>.</returns>
		/// <param name="node">The <see cref="System.Xml.XmlNode"/>.</param>
		/// <param name="setAttributes">A <see cref="SortedSet"/> of strings.</param>
		private static bool HasAttributes(XmlNode node, SortedSet<string> setAttributes)
		{
			bool toret = false;

			foreach ( XmlAttribute attr in node.Attributes ) {
				if ( setAttributes.Contains( attr.Name.ToLowerInvariant() ) ) {
					toret = true;
					break;
				}
			}
			return toret;
		}

		/// <summary>
		/// Determines whether the specified node represents a graphic menu.
		/// Note that there is no special tag for graphic menus.
		/// Only the attributes ImageWidth, ImageHeight, MinNumOfColumns, ImagePath, or ImageTooltip
		/// will reveal the presence of a graphic menu in a regular menu... and they can be anywhere!
		/// </summary>
		/// <returns><c>true</c> if this instance is graphic menu the specified node; otherwise, <c>false</c>.</returns>
		/// <param name="node">A <see cref="XmlNode"/> which can hold a menu or not.</param>
		private bool IsGraphicMenu(XmlNode node)
		{
			bool toret = false;

			// Is it a menu?
			if ( node.Name.Equals( Menu.TagName, StringComparison.OrdinalIgnoreCase ) )
			{
				if ( !HasAttributes( node, FirstLevelGraphicAttributes ) )
				{
					// Explore its subnodes
					foreach ( XmlNode subNode in node.ChildNodes ) {
						if ( HasAttributes( subNode, SecondLevelGraphicAttributes ) )
						{
							toret = true;
							break;
						}
					}
				}
			}

			return toret;
		}

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file, as a string.</value>
        public string FileName {
            get; set;
        }

        /// <summary>
        /// Gets the design of user menu.
        /// </summary>
        /// <value>The design of user menu, as a <see cref="DesignOfUserMenu"/>.</value>
        public DesignOfUserMenu DesignOfUserMenu {
            get {
                return this.document;
            }
        }

        private DesignOfUserMenu document;

		private static readonly SortedSet<string> FirstLevelGraphicAttributes =
			new SortedSet<string>( new string[] {
				GraphicMenu.EtqImageWidth.ToLowerInvariant(),
				GraphicMenu.EtqImageHeight.ToLowerInvariant(),
				GraphicMenu.EtqMinimumNumberOfColumns.ToLowerInvariant()
		});

		private static readonly SortedSet<string> SecondLevelGraphicAttributes =
			new SortedSet<string>( new string[] {
				GraphicMenuEntry.EtqImagePath.ToLowerInvariant(),
				GraphicMenuEntry.EtqImageToolTip.ToLowerInvariant()
		});
    }
}

