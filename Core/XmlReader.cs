using System;
using System.Text;
using System.Xml;
using System.IO;

using RAppMenu.Core.MenuComponents;

namespace RAppMenu.Core {
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

            return;
        }

        private void ReadNodeInto(XmlNode node, Menu menu)
        {
            // Subnodes of node
            foreach(XmlNode subNode in node.ChildNodes) {
                if ( subNode.Name.Equals( Menu.TagName, StringComparison.OrdinalIgnoreCase ) ) {
                    Menu subMenu = Menu.FromXml( subNode, menu );

                    menu.Add( subMenu );
                    this.ReadNodeInto( subNode, subMenu );
                }
            }

            return;
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
    }
}

