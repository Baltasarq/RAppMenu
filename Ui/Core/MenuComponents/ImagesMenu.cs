using System.Xml;

namespace RAppMenu.Core.MenuComponents {
	/// <summary>
	/// Represents a menu composed by images, which launch functions.
	/// </summary>
	public class ImagesMenu: MenuComponent {
		public ImagesMenu(string name)
			:base( name )
		{
		}

		public override void ToXml(XmlTextWriter doc)
		{
		}
	}
}

