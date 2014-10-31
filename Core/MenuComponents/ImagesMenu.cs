using System.Xml;

namespace RAppMenu.Core.MenuComponents {
	/// <summary>
	/// Represents a menu composed by images, which launch functions.
	/// </summary>
	public class ImagesMenu: MenuComponent {
		public ImagesMenu(string name, MenuEntry parent)
			:base( name, parent )
		{
		}

		public override void ToXml(XmlTextWriter doc)
		{
		}
	}
}

