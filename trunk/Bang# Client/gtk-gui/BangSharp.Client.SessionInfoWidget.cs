
// This file has been generated by the GUI designer. Do not modify.
namespace BangSharp.Client
{
	public partial class SessionInfoWidget
	{
		private global::Gtk.Frame frame3;
		private global::Gtk.Alignment GtkAlignment;
		private global::Gtk.Notebook notebook;
		private global::Gtk.HBox hbox1;
		private global::Gtk.Table table2;
		private global::Gtk.Label descriptionLabel;
		private global::Gtk.Expander expander1;
		private global::Gtk.Table table3;
		private global::Gtk.Label aFistfulOfCardsLabel;
		private global::Gtk.Label dodgeCityLabel;
		private global::Gtk.Label highNoonLabel;
		private global::Gtk.Label label15;
		private global::Gtk.Label label16;
		private global::Gtk.Label label17;
		private global::Gtk.Label label18;
		private global::Gtk.Label wildWestShowLabel;
		private global::Gtk.Label GtkLabel;
		private global::Gtk.Label gamesPlayedLabel;
		private global::Gtk.Label hasPlayerPasswordLabel;
		private global::Gtk.Label hasSpectatorPasswordLabel;
		private global::Gtk.Label label10;
		private global::Gtk.Label label11;
		private global::Gtk.Label label14;
		private global::Gtk.Label label4;
		private global::Gtk.Label label5;
		private global::Gtk.Label label6;
		private global::Gtk.Label label7;
		private global::Gtk.Label label8;
		private global::Gtk.Label label9;
		private global::Gtk.Label maxPlayerCountLabel;
		private global::Gtk.Label maxSpectatorCountLabel;
		private global::Gtk.Label minPlayerCountLabel;
		private global::Gtk.Label nameLabel;
		private global::Gtk.Label stateLabel;
		private global::Gtk.VBox vbox3;
		private global::Gtk.Frame frame4;
		private global::Gtk.Alignment GtkAlignment1;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.NodeView playersView;
		private global::Gtk.Label GtkLabel2;
		private global::Gtk.Frame frame5;
		private global::Gtk.Alignment GtkAlignment2;
		private global::Gtk.ScrolledWindow GtkScrolledWindow1;
		private global::Gtk.NodeView spectatorsView;
		private global::Gtk.Label GtkLabel3;
		private global::Gtk.Label label3;
		private global::Gtk.ScrolledWindow GtkScrolledWindow2;
		private global::Gtk.NodeView charStats;
		private global::Gtk.Label label1;
		private global::Gtk.ScrolledWindow GtkScrolledWindow3;
		private global::Gtk.NodeView roleStats;
		private global::Gtk.Label label2;
		private global::Gtk.Label GtkLabel4;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget BangSharp.Client.SessionInfoWidget
			global::Stetic.BinContainer.Attach (this);
			this.Name = "BangSharp.Client.SessionInfoWidget";
			// Container child BangSharp.Client.SessionInfoWidget.Gtk.Container+ContainerChild
			this.frame3 = new global::Gtk.Frame ();
			this.frame3.Name = "frame3";
			this.frame3.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frame3.Gtk.Container+ContainerChild
			this.GtkAlignment = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment.Name = "GtkAlignment";
			this.GtkAlignment.LeftPadding = ((uint)(12));
			// Container child GtkAlignment.Gtk.Container+ContainerChild
			this.notebook = new global::Gtk.Notebook ();
			this.notebook.Sensitive = false;
			this.notebook.CanFocus = true;
			this.notebook.Name = "notebook";
			this.notebook.CurrentPage = 0;
			// Container child notebook.Gtk.Notebook+NotebookChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			this.hbox1.BorderWidth = ((uint)(6));
			// Container child hbox1.Gtk.Box+BoxChild
			this.table2 = new global::Gtk.Table (((uint)(10)), ((uint)(2)), false);
			this.table2.Name = "table2";
			this.table2.RowSpacing = ((uint)(6));
			this.table2.ColumnSpacing = ((uint)(6));
			// Container child table2.Gtk.Table+TableChild
			this.descriptionLabel = new global::Gtk.Label ();
			this.descriptionLabel.Name = "descriptionLabel";
			this.descriptionLabel.Wrap = true;
			this.table2.Add (this.descriptionLabel);
			global::Gtk.Table.TableChild w1 = ((global::Gtk.Table.TableChild)(this.table2 [this.descriptionLabel]));
			w1.TopAttach = ((uint)(1));
			w1.BottomAttach = ((uint)(2));
			w1.LeftAttach = ((uint)(1));
			w1.RightAttach = ((uint)(2));
			w1.XOptions = ((global::Gtk.AttachOptions)(4));
			w1.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.expander1 = new global::Gtk.Expander (null);
			this.expander1.CanFocus = true;
			this.expander1.Name = "expander1";
			this.expander1.Expanded = true;
			// Container child expander1.Gtk.Container+ContainerChild
			this.table3 = new global::Gtk.Table (((uint)(4)), ((uint)(2)), false);
			this.table3.Name = "table3";
			this.table3.RowSpacing = ((uint)(6));
			this.table3.ColumnSpacing = ((uint)(6));
			this.table3.BorderWidth = ((uint)(6));
			// Container child table3.Gtk.Table+TableChild
			this.aFistfulOfCardsLabel = new global::Gtk.Label ();
			this.aFistfulOfCardsLabel.Name = "aFistfulOfCardsLabel";
			this.aFistfulOfCardsLabel.LabelProp = "No";
			this.table3.Add (this.aFistfulOfCardsLabel);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table3 [this.aFistfulOfCardsLabel]));
			w2.TopAttach = ((uint)(2));
			w2.BottomAttach = ((uint)(3));
			w2.LeftAttach = ((uint)(1));
			w2.RightAttach = ((uint)(2));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.dodgeCityLabel = new global::Gtk.Label ();
			this.dodgeCityLabel.Name = "dodgeCityLabel";
			this.dodgeCityLabel.LabelProp = "No";
			this.table3.Add (this.dodgeCityLabel);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table3 [this.dodgeCityLabel]));
			w3.LeftAttach = ((uint)(1));
			w3.RightAttach = ((uint)(2));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.highNoonLabel = new global::Gtk.Label ();
			this.highNoonLabel.Name = "highNoonLabel";
			this.highNoonLabel.LabelProp = "No";
			this.table3.Add (this.highNoonLabel);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table3 [this.highNoonLabel]));
			w4.TopAttach = ((uint)(1));
			w4.BottomAttach = ((uint)(2));
			w4.LeftAttach = ((uint)(1));
			w4.RightAttach = ((uint)(2));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label15 = new global::Gtk.Label ();
			this.label15.Name = "label15";
			this.label15.Xalign = 0F;
			this.label15.LabelProp = global::Mono.Unix.Catalog.GetString ("Dodge City:");
			this.table3.Add (this.label15);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table3 [this.label15]));
			w5.XOptions = ((global::Gtk.AttachOptions)(4));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label16 = new global::Gtk.Label ();
			this.label16.Name = "label16";
			this.label16.Xalign = 0F;
			this.label16.LabelProp = global::Mono.Unix.Catalog.GetString ("High Noon:");
			this.table3.Add (this.label16);
			global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.table3 [this.label16]));
			w6.TopAttach = ((uint)(1));
			w6.BottomAttach = ((uint)(2));
			w6.XOptions = ((global::Gtk.AttachOptions)(4));
			w6.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label17 = new global::Gtk.Label ();
			this.label17.Name = "label17";
			this.label17.Xalign = 0F;
			this.label17.LabelProp = global::Mono.Unix.Catalog.GetString ("A Fistful of Cards:");
			this.table3.Add (this.label17);
			global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this.table3 [this.label17]));
			w7.TopAttach = ((uint)(2));
			w7.BottomAttach = ((uint)(3));
			w7.XOptions = ((global::Gtk.AttachOptions)(4));
			w7.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label18 = new global::Gtk.Label ();
			this.label18.Name = "label18";
			this.label18.Xalign = 0F;
			this.label18.LabelProp = global::Mono.Unix.Catalog.GetString ("Wild West Show:");
			this.table3.Add (this.label18);
			global::Gtk.Table.TableChild w8 = ((global::Gtk.Table.TableChild)(this.table3 [this.label18]));
			w8.TopAttach = ((uint)(3));
			w8.BottomAttach = ((uint)(4));
			w8.XOptions = ((global::Gtk.AttachOptions)(4));
			w8.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.wildWestShowLabel = new global::Gtk.Label ();
			this.wildWestShowLabel.Name = "wildWestShowLabel";
			this.wildWestShowLabel.LabelProp = "No";
			this.table3.Add (this.wildWestShowLabel);
			global::Gtk.Table.TableChild w9 = ((global::Gtk.Table.TableChild)(this.table3 [this.wildWestShowLabel]));
			w9.TopAttach = ((uint)(3));
			w9.BottomAttach = ((uint)(4));
			w9.LeftAttach = ((uint)(1));
			w9.RightAttach = ((uint)(2));
			w9.YOptions = ((global::Gtk.AttachOptions)(4));
			this.expander1.Add (this.table3);
			this.GtkLabel = new global::Gtk.Label ();
			this.GtkLabel.Name = "GtkLabel";
			this.GtkLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Expansions</b>");
			this.GtkLabel.UseMarkup = true;
			this.GtkLabel.UseUnderline = true;
			this.expander1.LabelWidget = this.GtkLabel;
			this.table2.Add (this.expander1);
			global::Gtk.Table.TableChild w11 = ((global::Gtk.Table.TableChild)(this.table2 [this.expander1]));
			w11.TopAttach = ((uint)(8));
			w11.BottomAttach = ((uint)(9));
			w11.RightAttach = ((uint)(2));
			w11.XOptions = ((global::Gtk.AttachOptions)(4));
			w11.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.gamesPlayedLabel = new global::Gtk.Label ();
			this.gamesPlayedLabel.Name = "gamesPlayedLabel";
			this.table2.Add (this.gamesPlayedLabel);
			global::Gtk.Table.TableChild w12 = ((global::Gtk.Table.TableChild)(this.table2 [this.gamesPlayedLabel]));
			w12.TopAttach = ((uint)(9));
			w12.BottomAttach = ((uint)(10));
			w12.LeftAttach = ((uint)(1));
			w12.RightAttach = ((uint)(2));
			w12.XOptions = ((global::Gtk.AttachOptions)(4));
			w12.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.hasPlayerPasswordLabel = new global::Gtk.Label ();
			this.hasPlayerPasswordLabel.Name = "hasPlayerPasswordLabel";
			this.hasPlayerPasswordLabel.LabelProp = "No";
			this.table2.Add (this.hasPlayerPasswordLabel);
			global::Gtk.Table.TableChild w13 = ((global::Gtk.Table.TableChild)(this.table2 [this.hasPlayerPasswordLabel]));
			w13.TopAttach = ((uint)(6));
			w13.BottomAttach = ((uint)(7));
			w13.LeftAttach = ((uint)(1));
			w13.RightAttach = ((uint)(2));
			w13.XOptions = ((global::Gtk.AttachOptions)(4));
			w13.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.hasSpectatorPasswordLabel = new global::Gtk.Label ();
			this.hasSpectatorPasswordLabel.Name = "hasSpectatorPasswordLabel";
			this.hasSpectatorPasswordLabel.LabelProp = "No";
			this.table2.Add (this.hasSpectatorPasswordLabel);
			global::Gtk.Table.TableChild w14 = ((global::Gtk.Table.TableChild)(this.table2 [this.hasSpectatorPasswordLabel]));
			w14.TopAttach = ((uint)(7));
			w14.BottomAttach = ((uint)(8));
			w14.LeftAttach = ((uint)(1));
			w14.RightAttach = ((uint)(2));
			w14.XOptions = ((global::Gtk.AttachOptions)(4));
			w14.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label10 = new global::Gtk.Label ();
			this.label10.Name = "label10";
			this.label10.Xalign = 0F;
			this.label10.LabelProp = global::Mono.Unix.Catalog.GetString ("Has Player Password:");
			this.table2.Add (this.label10);
			global::Gtk.Table.TableChild w15 = ((global::Gtk.Table.TableChild)(this.table2 [this.label10]));
			w15.TopAttach = ((uint)(6));
			w15.BottomAttach = ((uint)(7));
			w15.XOptions = ((global::Gtk.AttachOptions)(4));
			w15.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label11 = new global::Gtk.Label ();
			this.label11.Name = "label11";
			this.label11.Xalign = 0F;
			this.label11.LabelProp = global::Mono.Unix.Catalog.GetString ("Has Spectator Password:");
			this.table2.Add (this.label11);
			global::Gtk.Table.TableChild w16 = ((global::Gtk.Table.TableChild)(this.table2 [this.label11]));
			w16.TopAttach = ((uint)(7));
			w16.BottomAttach = ((uint)(8));
			w16.XOptions = ((global::Gtk.AttachOptions)(4));
			w16.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label14 = new global::Gtk.Label ();
			this.label14.Name = "label14";
			this.label14.Xalign = 0F;
			this.label14.LabelProp = global::Mono.Unix.Catalog.GetString ("Games Played:");
			this.table2.Add (this.label14);
			global::Gtk.Table.TableChild w17 = ((global::Gtk.Table.TableChild)(this.table2 [this.label14]));
			w17.TopAttach = ((uint)(9));
			w17.BottomAttach = ((uint)(10));
			w17.XOptions = ((global::Gtk.AttachOptions)(4));
			w17.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label4 = new global::Gtk.Label ();
			this.label4.Name = "label4";
			this.label4.Xalign = 0F;
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("Name:");
			this.table2.Add (this.label4);
			global::Gtk.Table.TableChild w18 = ((global::Gtk.Table.TableChild)(this.table2 [this.label4]));
			w18.XOptions = ((global::Gtk.AttachOptions)(4));
			w18.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label5 = new global::Gtk.Label ();
			this.label5.Name = "label5";
			this.label5.Xalign = 0F;
			this.label5.LabelProp = global::Mono.Unix.Catalog.GetString ("Description:");
			this.table2.Add (this.label5);
			global::Gtk.Table.TableChild w19 = ((global::Gtk.Table.TableChild)(this.table2 [this.label5]));
			w19.TopAttach = ((uint)(1));
			w19.BottomAttach = ((uint)(2));
			w19.XOptions = ((global::Gtk.AttachOptions)(4));
			w19.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label6 = new global::Gtk.Label ();
			this.label6.Name = "label6";
			this.label6.Xalign = 0F;
			this.label6.LabelProp = global::Mono.Unix.Catalog.GetString ("State:");
			this.table2.Add (this.label6);
			global::Gtk.Table.TableChild w20 = ((global::Gtk.Table.TableChild)(this.table2 [this.label6]));
			w20.TopAttach = ((uint)(2));
			w20.BottomAttach = ((uint)(3));
			w20.XOptions = ((global::Gtk.AttachOptions)(4));
			w20.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label7 = new global::Gtk.Label ();
			this.label7.Name = "label7";
			this.label7.Xalign = 0F;
			this.label7.LabelProp = global::Mono.Unix.Catalog.GetString ("Min. Player Count:");
			this.table2.Add (this.label7);
			global::Gtk.Table.TableChild w21 = ((global::Gtk.Table.TableChild)(this.table2 [this.label7]));
			w21.TopAttach = ((uint)(3));
			w21.BottomAttach = ((uint)(4));
			w21.XOptions = ((global::Gtk.AttachOptions)(4));
			w21.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label8 = new global::Gtk.Label ();
			this.label8.Name = "label8";
			this.label8.Xalign = 0F;
			this.label8.LabelProp = global::Mono.Unix.Catalog.GetString ("Max. Player Count:");
			this.table2.Add (this.label8);
			global::Gtk.Table.TableChild w22 = ((global::Gtk.Table.TableChild)(this.table2 [this.label8]));
			w22.TopAttach = ((uint)(4));
			w22.BottomAttach = ((uint)(5));
			w22.XOptions = ((global::Gtk.AttachOptions)(4));
			w22.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label9 = new global::Gtk.Label ();
			this.label9.Name = "label9";
			this.label9.Xalign = 0F;
			this.label9.LabelProp = global::Mono.Unix.Catalog.GetString ("Max. Spectator Count:");
			this.table2.Add (this.label9);
			global::Gtk.Table.TableChild w23 = ((global::Gtk.Table.TableChild)(this.table2 [this.label9]));
			w23.TopAttach = ((uint)(5));
			w23.BottomAttach = ((uint)(6));
			w23.XOptions = ((global::Gtk.AttachOptions)(4));
			w23.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.maxPlayerCountLabel = new global::Gtk.Label ();
			this.maxPlayerCountLabel.Name = "maxPlayerCountLabel";
			this.table2.Add (this.maxPlayerCountLabel);
			global::Gtk.Table.TableChild w24 = ((global::Gtk.Table.TableChild)(this.table2 [this.maxPlayerCountLabel]));
			w24.TopAttach = ((uint)(4));
			w24.BottomAttach = ((uint)(5));
			w24.LeftAttach = ((uint)(1));
			w24.RightAttach = ((uint)(2));
			w24.XOptions = ((global::Gtk.AttachOptions)(4));
			w24.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.maxSpectatorCountLabel = new global::Gtk.Label ();
			this.maxSpectatorCountLabel.Name = "maxSpectatorCountLabel";
			this.table2.Add (this.maxSpectatorCountLabel);
			global::Gtk.Table.TableChild w25 = ((global::Gtk.Table.TableChild)(this.table2 [this.maxSpectatorCountLabel]));
			w25.TopAttach = ((uint)(5));
			w25.BottomAttach = ((uint)(6));
			w25.LeftAttach = ((uint)(1));
			w25.RightAttach = ((uint)(2));
			w25.XOptions = ((global::Gtk.AttachOptions)(4));
			w25.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.minPlayerCountLabel = new global::Gtk.Label ();
			this.minPlayerCountLabel.Name = "minPlayerCountLabel";
			this.table2.Add (this.minPlayerCountLabel);
			global::Gtk.Table.TableChild w26 = ((global::Gtk.Table.TableChild)(this.table2 [this.minPlayerCountLabel]));
			w26.TopAttach = ((uint)(3));
			w26.BottomAttach = ((uint)(4));
			w26.LeftAttach = ((uint)(1));
			w26.RightAttach = ((uint)(2));
			w26.XOptions = ((global::Gtk.AttachOptions)(4));
			w26.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.nameLabel = new global::Gtk.Label ();
			this.nameLabel.Name = "nameLabel";
			this.table2.Add (this.nameLabel);
			global::Gtk.Table.TableChild w27 = ((global::Gtk.Table.TableChild)(this.table2 [this.nameLabel]));
			w27.LeftAttach = ((uint)(1));
			w27.RightAttach = ((uint)(2));
			w27.XOptions = ((global::Gtk.AttachOptions)(4));
			w27.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.stateLabel = new global::Gtk.Label ();
			this.stateLabel.Name = "stateLabel";
			this.table2.Add (this.stateLabel);
			global::Gtk.Table.TableChild w28 = ((global::Gtk.Table.TableChild)(this.table2 [this.stateLabel]));
			w28.TopAttach = ((uint)(2));
			w28.BottomAttach = ((uint)(3));
			w28.LeftAttach = ((uint)(1));
			w28.RightAttach = ((uint)(2));
			w28.XOptions = ((global::Gtk.AttachOptions)(4));
			w28.YOptions = ((global::Gtk.AttachOptions)(4));
			this.hbox1.Add (this.table2);
			global::Gtk.Box.BoxChild w29 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.table2]));
			w29.Position = 0;
			w29.Expand = false;
			w29.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.vbox3 = new global::Gtk.VBox ();
			this.vbox3.Name = "vbox3";
			this.vbox3.Spacing = 6;
			// Container child vbox3.Gtk.Box+BoxChild
			this.frame4 = new global::Gtk.Frame ();
			this.frame4.Name = "frame4";
			this.frame4.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frame4.Gtk.Container+ContainerChild
			this.GtkAlignment1 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment1.Name = "GtkAlignment1";
			this.GtkAlignment1.LeftPadding = ((uint)(12));
			// Container child GtkAlignment1.Gtk.Container+ContainerChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.playersView = new global::Gtk.NodeView ();
			this.playersView.CanFocus = true;
			this.playersView.Name = "playersView";
			this.GtkScrolledWindow.Add (this.playersView);
			this.GtkAlignment1.Add (this.GtkScrolledWindow);
			this.frame4.Add (this.GtkAlignment1);
			this.GtkLabel2 = new global::Gtk.Label ();
			this.GtkLabel2.Name = "GtkLabel2";
			this.GtkLabel2.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Players</b>");
			this.GtkLabel2.UseMarkup = true;
			this.frame4.LabelWidget = this.GtkLabel2;
			this.vbox3.Add (this.frame4);
			global::Gtk.Box.BoxChild w33 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.frame4]));
			w33.Position = 0;
			// Container child vbox3.Gtk.Box+BoxChild
			this.frame5 = new global::Gtk.Frame ();
			this.frame5.Name = "frame5";
			this.frame5.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frame5.Gtk.Container+ContainerChild
			this.GtkAlignment2 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment2.Name = "GtkAlignment2";
			this.GtkAlignment2.LeftPadding = ((uint)(12));
			// Container child GtkAlignment2.Gtk.Container+ContainerChild
			this.GtkScrolledWindow1 = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow1.Name = "GtkScrolledWindow1";
			this.GtkScrolledWindow1.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow1.Gtk.Container+ContainerChild
			this.spectatorsView = new global::Gtk.NodeView ();
			this.spectatorsView.CanFocus = true;
			this.spectatorsView.Name = "spectatorsView";
			this.GtkScrolledWindow1.Add (this.spectatorsView);
			this.GtkAlignment2.Add (this.GtkScrolledWindow1);
			this.frame5.Add (this.GtkAlignment2);
			this.GtkLabel3 = new global::Gtk.Label ();
			this.GtkLabel3.Name = "GtkLabel3";
			this.GtkLabel3.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Spectators</b>");
			this.GtkLabel3.UseMarkup = true;
			this.frame5.LabelWidget = this.GtkLabel3;
			this.vbox3.Add (this.frame5);
			global::Gtk.Box.BoxChild w37 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.frame5]));
			w37.Position = 1;
			this.hbox1.Add (this.vbox3);
			global::Gtk.Box.BoxChild w38 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.vbox3]));
			w38.Position = 1;
			this.notebook.Add (this.hbox1);
			// Notebook tab
			this.label3 = new global::Gtk.Label ();
			this.label3.Name = "label3";
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("Basic information");
			this.notebook.SetTabLabel (this.hbox1, this.label3);
			this.label3.ShowAll ();
			// Container child notebook.Gtk.Notebook+NotebookChild
			this.GtkScrolledWindow2 = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow2.Name = "GtkScrolledWindow2";
			this.GtkScrolledWindow2.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow2.Gtk.Container+ContainerChild
			this.charStats = new global::Gtk.NodeView ();
			this.charStats.CanFocus = true;
			this.charStats.Name = "charStats";
			this.GtkScrolledWindow2.Add (this.charStats);
			this.notebook.Add (this.GtkScrolledWindow2);
			global::Gtk.Notebook.NotebookChild w41 = ((global::Gtk.Notebook.NotebookChild)(this.notebook [this.GtkScrolledWindow2]));
			w41.Position = 1;
			// Notebook tab
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Character statistics");
			this.notebook.SetTabLabel (this.GtkScrolledWindow2, this.label1);
			this.label1.ShowAll ();
			// Container child notebook.Gtk.Notebook+NotebookChild
			this.GtkScrolledWindow3 = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow3.Name = "GtkScrolledWindow3";
			this.GtkScrolledWindow3.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow3.Gtk.Container+ContainerChild
			this.roleStats = new global::Gtk.NodeView ();
			this.roleStats.CanFocus = true;
			this.roleStats.Name = "roleStats";
			this.GtkScrolledWindow3.Add (this.roleStats);
			this.notebook.Add (this.GtkScrolledWindow3);
			global::Gtk.Notebook.NotebookChild w43 = ((global::Gtk.Notebook.NotebookChild)(this.notebook [this.GtkScrolledWindow3]));
			w43.Position = 2;
			// Notebook tab
			this.label2 = new global::Gtk.Label ();
			this.label2.Name = "label2";
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Role statistics");
			this.notebook.SetTabLabel (this.GtkScrolledWindow3, this.label2);
			this.label2.ShowAll ();
			this.GtkAlignment.Add (this.notebook);
			this.frame3.Add (this.GtkAlignment);
			this.GtkLabel4 = new global::Gtk.Label ();
			this.GtkLabel4.Name = "GtkLabel4";
			this.GtkLabel4.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Session Information</b>");
			this.GtkLabel4.UseMarkup = true;
			this.frame3.LabelWidget = this.GtkLabel4;
			this.Add (this.frame3);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.label15.MnemonicWidget = this.dodgeCityLabel;
			this.label16.MnemonicWidget = this.highNoonLabel;
			this.label18.MnemonicWidget = this.wildWestShowLabel;
			this.label10.MnemonicWidget = this.hasPlayerPasswordLabel;
			this.label11.MnemonicWidget = this.hasSpectatorPasswordLabel;
			this.label14.MnemonicWidget = this.gamesPlayedLabel;
			this.label4.MnemonicWidget = this.nameLabel;
			this.label5.MnemonicWidget = this.descriptionLabel;
			this.label6.MnemonicWidget = this.stateLabel;
			this.label7.MnemonicWidget = this.minPlayerCountLabel;
			this.label8.MnemonicWidget = this.maxPlayerCountLabel;
			this.label9.MnemonicWidget = this.maxSpectatorCountLabel;
			this.Hide ();
			this.DestroyEvent += new global::Gtk.DestroyEventHandler (this.OnDestroyEvent);
		}
	}
}