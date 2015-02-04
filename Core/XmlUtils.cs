using System;
using System.Text;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using RWABuilder.Core.MenuComponents;

namespace RWABuilder.Core {
	static class XmlUtils {
        public static string AsString(this XmlNode node)
        {
            var attributesInfo = new StringBuilder();

            foreach(XmlAttribute attr in node.Attributes) {
                attributesInfo.AppendFormat( "({0}: {1})", attr.Name, attr.Value );
            }

            return string.Format( "[{0} [{1}]: '{2}']",
                node.Name,
                attributesInfo.ToString(),
                node.Value );
        }

		public static XmlNode GetNamedItemIgnoreCase(this XmlAttributeCollection attrList, string id)
		{
			XmlAttribute toret = null;

            if ( string.IsNullOrWhiteSpace( id ) ) {
                throw new XmlException(
                    "getting attribute of list: asking for null id" );
            }

            if ( attrList == null ) {
                throw new XmlException(
                    "getting attribute of list: missing attribute list" );
            }

            id = id.Trim();
			foreach ( XmlAttribute attr in attrList ) {
				if ( attr.Name.Equals( id, StringComparison.OrdinalIgnoreCase ) ) {
					toret = attr;
					break;
				}
			}

			return toret;
		}

        public static XmlAttribute GetAttribute(this XmlNode node, string id)
        {
            XmlAttribute toret = null;

            if ( string.IsNullOrWhiteSpace( id ) ) {
                throw new XmlException(
                    "getting attribute from node: asking for null id" );
            }

            if ( node == null ) {
                throw new XmlException(
                    "getting attribute from node: missing attribute list" );
            }

            id = id.Trim();
            toret = (XmlAttribute) node.Attributes.GetNamedItemIgnoreCase( id );

            if ( toret == null ) {
                throw new XmlException( "missing attribute: '" + id +"' at "
                    + node.GetPath()
                );
            }

            return toret;
        }

        public static string GetPath(this XmlNode node)
        {
            var attr = node as XmlAttribute;
            var toret = new StringBuilder();

            toret.Append( node.Name );

            if ( attr != null ) {
                // Get to the owner of the attribute
                node = attr.OwnerElement;
                toret.Insert( 0, node.Name + @": " );
            }

            while ( node.ParentNode != null
                 && node.ParentNode.NodeType != XmlNodeType.Document )
            {
                node = node.ParentNode;
                toret.Insert( 0, node.Name + @": " );
            }

            return toret.ToString();
        }

        public static int GetValueAsInt(this XmlNode node)
        {
            int toret;

            if ( !int.TryParse( node.InnerText.Trim(), out toret) ) {
                throw new XmlException(
                    "node '" + node.Name + "' does not contain a number at "
                    + node.GetPath()
                );
            }

            return toret;
        }

        public static bool GetValueAsBool(this XmlNode node)
        {
            bool toret;

            if ( !bool.TryParse( node.InnerText.Trim(), out toret) ) {
                throw new XmlException(
                    "node '" + node.Name + "' does not contain a boolean at "
                    + node.GetPath()
                );
            }

            return toret;
        }
	}
}
