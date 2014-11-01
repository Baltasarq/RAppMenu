using System;
using System.Drawing;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RAppMenu.Ui {
	/// <summary>
	/// User actions involve all kind of widgets (menus, buttons...)
	/// which can trigger an execution.
	/// </summary>
	public class UserAction {
        /// <summary>
        /// Initializes a new instance of the <see cref="RAppMenu.Ui.UserAction"/> class.
        /// This class represents a user action, such as Load, Save.
        /// It can be composed by varios controls.
        /// </summary>
        /// <param name="txt">Text.</param>
        /// <param name="imgIndex">Image index.</param>
        /// <param name="f">F.</param>
        public UserAction(string txt, int imgIndex, Action f)
            :this( txt, f )
        {
            this.ImageIndex = imgIndex;
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="RAppMenu.Ui.UserAction"/> class.
		/// This class represents a user action, such as Load, Save.
		/// It includes controls such as a menu, a button, and the objective
		/// is to control them all at the same time.
		/// </summary>
		/// <param name="f">F.</param>
		public UserAction(string txt, Action f)
		{
            this.Text = txt;
			this.CallBack = f;
			this.controls = new List<Component>();
		}

		/// <summary>
		/// Adds a given control for the list of controls
		/// that triggers this action.
		/// The class must be Widget. Be sure it supports Enabled and Visible.
		/// </summary>
		/// <param name="c">The given control, as a <see cref="Component"/> object</param>
		public void AddComponent(Component c)
		{
			this.controls.Add( c );
		}

		/// <summary>
		/// Returns the count of controls.
		/// </summary>
		/// <returns>The number of controls.</returns>
		public int CountComponents()
		{
			return this.controls.Count;
		}

		/// <summary>
		/// Hides all controls.
		/// </summary>
		public void HideControls()
		{
			this.SetVisible( false );
		}

		/// <summary>
		/// Shows all controls.
		/// </summary>
		public void Show()
		{
			this.SetVisible( true );
		}

		/// <summary>
		/// Returns whether the controls are visible.
		/// Note that only the first one is taken into account.
		/// </summary>
		/// <returns><c>true</c>, if controls are visible, <c>false</c> otherwise.</returns>
		public bool IsVisible()
		{
			bool toret = false;

			if ( this.controls.Count > 0 ) {
                dynamic c = this.controls[ 0 ];
				toret = c.Visible;
			}

			return toret;
		}

		/// <summary>
		/// Sets the controls visible or not, depending on parameter.
		/// </summary>
		/// <param name="visible">If set to <c>true</c> controls will be visible.</param>
		public void SetVisible(bool visible)
		{
			foreach(dynamic c in this.controls) {
				c.Visible = visible;
			}

			return;
		}

		/// <summary>
		/// Disables all controls.
		/// </summary>
		public void Disable()
		{
			this.SetEnabled( false );
		}

		/// <summary>
		/// Enables all controls.
		/// </summary>
		public void Enable()
		{
			this.SetEnabled( true );
		}

		/// <summary>
		/// Determines whether controls are enabled.
		/// Note that only the first one is taken into account.
		/// </summary>
		/// <returns><c>true</c>, if controls are enabled, <c>false</c> otherwise.</returns>
		public bool IsEnabled()
		{
			bool toret = false;

			if ( this.controls.Count > 0 ) {
                dynamic c = this.controls[ 0 ];
				toret = c.Enabled;
			}

			return toret;
		}

		/// <summary>
		/// Enables all controls or disables them, depending on param.
		/// </summary>
		/// <param name="enabled">If set to <c>true</c>, all enabled.</param>
		public void SetEnabled(bool enabled)
		{
			foreach(dynamic c in this.controls) {
				c.Enabled = enabled;
			}

			return;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="RAppMenu.Ui.UserAction"/> is enabled.
		/// (i.e., whether its controls are enabled)
		/// </summary>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		public bool Enabled {
			get {
				return this.IsEnabled();
			}
			set {
				this.SetEnabled( value );
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="RAppMenu.Ui.UserAction"/> is visible.
		/// (i.e., whether the controls are visible)
		/// </summary>
		/// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
		public bool Visible {
			get {
				return this.IsVisible();
			}
			set {
				this.SetVisible( value );
			}
		}

		/// <summary>
		/// Gets the controls this action is controlled by.
		/// </summary>
		/// <value>The controls, as a collecion of controls.</value>
		public ReadOnlyCollection<Component> Components {
			get {
                return this.controls.AsReadOnly();
			}
		}

		/// <summary>
		/// Gets or sets the callback.
		/// </summary>
		/// <value>The call back, as an <see cref="Action"/> object.</value>
		public Action CallBack {
			get; set;
		}

        /// <summary>
        /// Gets or sets the text message associated to this action.
        /// </summary>
        /// <value>The text, as a string.</value>
        public string Text {
            get; set;
        }

        /// <summary>
        /// Gets or sets the index of the image in the image list.
        /// </summary>
        /// <value>The index of the image, as an int.</value>
        public int ImageIndex {
            get; set;
        }

        /// <summary>
        /// Gets or sets the bitmap.
        /// Some controls only accept a bitmap, so...
        /// </summary>
        /// <value>The bitmap.</value>
        public Bitmap Bitmap {
            get; set;
        }

        public static ImageList ImageList = new ImageList();

		private List<Component> controls;
	}
}
