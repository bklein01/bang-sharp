
// This file has been generated by the GUI designer. Do not modify.
namespace BangSharp.Client
{
	public partial class ImageSelectorWidget
	{
		private global::Gtk.Table table1;
		private global::Gtk.FileChooserButton chooser;
		private global::Gtk.Button clearButton;
		private global::Gtk.Image image;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget BangSharp.Client.ImageSelectorWidget
			global::Stetic.BinContainer.Attach (this);
			this.Name = "BangSharp.Client.ImageSelectorWidget";
			// Container child BangSharp.Client.ImageSelectorWidget.Gtk.Container+ContainerChild
			this.table1 = new global::Gtk.Table (((uint)(2)), ((uint)(2)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(6));
			this.table1.ColumnSpacing = ((uint)(6));
			// Container child table1.Gtk.Table+TableChild
			this.chooser = new global::Gtk.FileChooserButton (global::Mono.Unix.Catalog.GetString ("Select an avatar image"), ((global::Gtk.FileChooserAction)(0)));
			this.chooser.Name = "chooser";
			this.table1.Add (this.chooser);
			global::Gtk.Table.TableChild w1 = ((global::Gtk.Table.TableChild)(this.table1 [this.chooser]));
			w1.TopAttach = ((uint)(1));
			w1.BottomAttach = ((uint)(2));
			w1.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.clearButton = new global::Gtk.Button ();
			this.clearButton.CanFocus = true;
			this.clearButton.Name = "clearButton";
			this.clearButton.UseStock = true;
			this.clearButton.UseUnderline = true;
			this.clearButton.Label = "gtk-clear";
			this.table1.Add (this.clearButton);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table1 [this.clearButton]));
			w2.TopAttach = ((uint)(1));
			w2.BottomAttach = ((uint)(2));
			w2.LeftAttach = ((uint)(1));
			w2.RightAttach = ((uint)(2));
			w2.XOptions = ((global::Gtk.AttachOptions)(4));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.image = new global::Gtk.Image ();
			this.image.Name = "image";
			this.table1.Add (this.image);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table1 [this.image]));
			w3.RightAttach = ((uint)(2));
			w3.XOptions = ((global::Gtk.AttachOptions)(0));
			w3.YOptions = ((global::Gtk.AttachOptions)(0));
			this.Add (this.table1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
			this.clearButton.Clicked += new global::System.EventHandler (this.OnClearButtonClicked);
			this.chooser.SelectionChanged += new global::System.EventHandler (this.OnChooserSelectionChanged);
		}
	}
}
