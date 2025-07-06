namespace PackedTables.Viewer {
  partial class Form1 {
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
      splitContainer1 = new SplitContainer();
      label1 = new Label();
      btnBrowse = new Button();
      imageList1 = new ImageList(components);
      btnOpenClose = new Button();
      comboBox1 = new ComboBox();
      splitContainer2 = new SplitContainer();
      splitContainer3 = new SplitContainer();
      panel1 = new Panel();
      label3 = new Label();
      label2 = new Label();
      cbType = new ComboBox();
      tbColumnName = new TextBox();
      treeView1 = new TreeView();
      tvMenu = new ContextMenuStrip(components);
      addTableToolStripMenuItem = new ToolStripMenuItem();
      addColumnToolStripMenuItem = new ToolStripMenuItem();
      addRowToolStripMenuItem = new ToolStripMenuItem();
      toolStripMenuItem1 = new ToolStripSeparator();
      removeRowToolStripMenuItem = new ToolStripMenuItem();
      removeColumnToolStripMenuItem = new ToolStripMenuItem();
      removeTableToolStripMenuItem = new ToolStripMenuItem();
      toolStripMenuItem2 = new ToolStripSeparator();
      saveToolStripMenuItem = new ToolStripMenuItem();
      saveAsToolStripMenuItem = new ToolStripMenuItem();
      toolStrip1 = new ToolStrip();
      toolStripButton1 = new ToolStripButton();
      toolStripButton2 = new ToolStripButton();
      vrMain = new DataGridView();
      textBox1 = new TextBox();
      odMain = new OpenFileDialog();
      saveDialog = new SaveFileDialog();
      ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
      splitContainer1.Panel1.SuspendLayout();
      splitContainer1.Panel2.SuspendLayout();
      splitContainer1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
      splitContainer2.Panel1.SuspendLayout();
      splitContainer2.Panel2.SuspendLayout();
      splitContainer2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)splitContainer3).BeginInit();
      splitContainer3.Panel1.SuspendLayout();
      splitContainer3.Panel2.SuspendLayout();
      splitContainer3.SuspendLayout();
      panel1.SuspendLayout();
      tvMenu.SuspendLayout();
      toolStrip1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)vrMain).BeginInit();
      SuspendLayout();
      // 
      // splitContainer1
      // 
      splitContainer1.Dock = DockStyle.Fill;
      splitContainer1.Location = new Point(0, 0);
      splitContainer1.Name = "splitContainer1";
      splitContainer1.Orientation = Orientation.Horizontal;
      // 
      // splitContainer1.Panel1
      // 
      splitContainer1.Panel1.Controls.Add(label1);
      splitContainer1.Panel1.Controls.Add(btnBrowse);
      splitContainer1.Panel1.Controls.Add(btnOpenClose);
      splitContainer1.Panel1.Controls.Add(comboBox1);
      // 
      // splitContainer1.Panel2
      // 
      splitContainer1.Panel2.Controls.Add(splitContainer2);
      splitContainer1.Size = new Size(560, 410);
      splitContainer1.SplitterDistance = 55;
      splitContainer1.TabIndex = 0;
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Location = new Point(3, 19);
      label1.Name = "label1";
      label1.Size = new Size(39, 15);
      label1.TabIndex = 22;
      label1.Text = "Open:";
      // 
      // btnBrowse
      // 
      btnBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      btnBrowse.ImageAlign = ContentAlignment.MiddleLeft;
      btnBrowse.ImageIndex = 2;
      btnBrowse.ImageList = imageList1;
      btnBrowse.Location = new Point(430, 16);
      btnBrowse.Margin = new Padding(3, 2, 3, 2);
      btnBrowse.Name = "btnBrowse";
      btnBrowse.Size = new Size(68, 23);
      btnBrowse.TabIndex = 21;
      btnBrowse.Text = "Browse";
      btnBrowse.TextAlign = ContentAlignment.MiddleRight;
      btnBrowse.UseVisualStyleBackColor = true;
      btnBrowse.Click += btnBrowse_Click;
      // 
      // imageList1
      // 
      imageList1.ColorDepth = ColorDepth.Depth32Bit;
      imageList1.ImageStream = (ImageListStreamer)resources.GetObject("imageList1.ImageStream");
      imageList1.TransparentColor = Color.Transparent;
      imageList1.Images.SetKeyName(0, "database.png");
      imageList1.Images.SetKeyName(1, "page.png");
      imageList1.Images.SetKeyName(2, "doc-magnifying-glass-in.png");
      imageList1.Images.SetKeyName(3, "doc-star-in.png");
      // 
      // btnOpenClose
      // 
      btnOpenClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      btnOpenClose.ImageAlign = ContentAlignment.MiddleLeft;
      btnOpenClose.ImageIndex = 3;
      btnOpenClose.ImageList = imageList1;
      btnOpenClose.Location = new Point(497, 16);
      btnOpenClose.Margin = new Padding(3, 2, 3, 2);
      btnOpenClose.Name = "btnOpenClose";
      btnOpenClose.Size = new Size(58, 23);
      btnOpenClose.TabIndex = 20;
      btnOpenClose.Text = "Open";
      btnOpenClose.TextAlign = ContentAlignment.MiddleRight;
      btnOpenClose.UseVisualStyleBackColor = true;
      btnOpenClose.Click += btnOpenClose_Click;
      // 
      // comboBox1
      // 
      comboBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      comboBox1.FormattingEnabled = true;
      comboBox1.Location = new Point(43, 16);
      comboBox1.Margin = new Padding(3, 2, 3, 2);
      comboBox1.Name = "comboBox1";
      comboBox1.Size = new Size(385, 23);
      comboBox1.TabIndex = 19;
      // 
      // splitContainer2
      // 
      splitContainer2.Dock = DockStyle.Fill;
      splitContainer2.Location = new Point(0, 0);
      splitContainer2.Name = "splitContainer2";
      splitContainer2.Orientation = Orientation.Horizontal;
      // 
      // splitContainer2.Panel1
      // 
      splitContainer2.Panel1.Controls.Add(splitContainer3);
      // 
      // splitContainer2.Panel2
      // 
      splitContainer2.Panel2.Controls.Add(textBox1);
      splitContainer2.Size = new Size(560, 351);
      splitContainer2.SplitterDistance = 265;
      splitContainer2.TabIndex = 0;
      // 
      // splitContainer3
      // 
      splitContainer3.Dock = DockStyle.Fill;
      splitContainer3.Location = new Point(0, 0);
      splitContainer3.Name = "splitContainer3";
      // 
      // splitContainer3.Panel1
      // 
      splitContainer3.Panel1.Controls.Add(panel1);
      splitContainer3.Panel1.Controls.Add(treeView1);
      splitContainer3.Panel1.Resize += splitContainer3_Panel1_Resize;
      // 
      // splitContainer3.Panel2
      // 
      splitContainer3.Panel2.Controls.Add(toolStrip1);
      splitContainer3.Panel2.Controls.Add(vrMain);
      splitContainer3.Panel2.Resize += splitContainer3_Panel2_Resize;
      splitContainer3.Size = new Size(560, 265);
      splitContainer3.SplitterDistance = 277;
      splitContainer3.TabIndex = 0;
      // 
      // panel1
      // 
      panel1.Controls.Add(label3);
      panel1.Controls.Add(label2);
      panel1.Controls.Add(cbType);
      panel1.Controls.Add(tbColumnName);
      panel1.Dock = DockStyle.Bottom;
      panel1.Location = new Point(0, 207);
      panel1.Name = "panel1";
      panel1.Size = new Size(277, 58);
      panel1.TabIndex = 2;
      // 
      // label3
      // 
      label3.AutoSize = true;
      label3.Location = new Point(24, 37);
      label3.Name = "label3";
      label3.Size = new Size(31, 15);
      label3.TabIndex = 3;
      label3.Text = "Type";
      // 
      // label2
      // 
      label2.AutoSize = true;
      label2.Location = new Point(7, 10);
      label2.Name = "label2";
      label2.Size = new Size(50, 15);
      label2.TabIndex = 2;
      label2.Text = "Column";
      // 
      // cbType
      // 
      cbType.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      cbType.FormattingEnabled = true;
      cbType.Items.AddRange(new object[] { "Unknown", "Null", "Boolean", "Int32", "Int64", "Decimal", "DateTime", "Guid", "String" });
      cbType.Location = new Point(63, 33);
      cbType.Name = "cbType";
      cbType.Size = new Size(205, 23);
      cbType.TabIndex = 1;
      cbType.SelectedIndexChanged += cbType_SelectedIndexChanged;
      // 
      // tbColumnName
      // 
      tbColumnName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      tbColumnName.Location = new Point(63, 7);
      tbColumnName.Name = "tbColumnName";
      tbColumnName.Size = new Size(205, 23);
      tbColumnName.TabIndex = 0;
      tbColumnName.KeyPress += tbColumnName_KeyPress;
      tbColumnName.Leave += tbColumnName_Leave;
      // 
      // treeView1
      // 
      treeView1.ContextMenuStrip = tvMenu;
      treeView1.Dock = DockStyle.Top;
      treeView1.ImageIndex = 0;
      treeView1.ImageList = imageList1;
      treeView1.Location = new Point(0, 0);
      treeView1.Name = "treeView1";
      treeView1.SelectedImageIndex = 0;
      treeView1.Size = new Size(277, 201);
      treeView1.TabIndex = 0;
      treeView1.AfterSelect += treeView1_AfterSelect;
      // 
      // tvMenu
      // 
      tvMenu.Items.AddRange(new ToolStripItem[] { addTableToolStripMenuItem, addColumnToolStripMenuItem, addRowToolStripMenuItem, toolStripMenuItem1, removeRowToolStripMenuItem, removeColumnToolStripMenuItem, removeTableToolStripMenuItem, toolStripMenuItem2, saveToolStripMenuItem, saveAsToolStripMenuItem });
      tvMenu.Name = "tvMenu";
      tvMenu.Size = new Size(164, 192);
      tvMenu.Opening += tvMenu_Opening;
      // 
      // addTableToolStripMenuItem
      // 
      addTableToolStripMenuItem.Name = "addTableToolStripMenuItem";
      addTableToolStripMenuItem.Size = new Size(163, 22);
      addTableToolStripMenuItem.Text = "Add Table";
      addTableToolStripMenuItem.Click += addTableToolStripMenuItem_Click;
      // 
      // addColumnToolStripMenuItem
      // 
      addColumnToolStripMenuItem.Name = "addColumnToolStripMenuItem";
      addColumnToolStripMenuItem.Size = new Size(163, 22);
      addColumnToolStripMenuItem.Text = "Add Column";
      addColumnToolStripMenuItem.Click += addColumnToolStripMenuItem_Click;
      // 
      // addRowToolStripMenuItem
      // 
      addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
      addRowToolStripMenuItem.Size = new Size(163, 22);
      addRowToolStripMenuItem.Text = "Add Row";
      addRowToolStripMenuItem.Click += addRowToolStripMenuItem_Click;
      // 
      // toolStripMenuItem1
      // 
      toolStripMenuItem1.Name = "toolStripMenuItem1";
      toolStripMenuItem1.Size = new Size(160, 6);
      // 
      // removeRowToolStripMenuItem
      // 
      removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
      removeRowToolStripMenuItem.Size = new Size(163, 22);
      removeRowToolStripMenuItem.Text = "Remove Row";
      removeRowToolStripMenuItem.Click += removeRowToolStripMenuItem_Click;
      // 
      // removeColumnToolStripMenuItem
      // 
      removeColumnToolStripMenuItem.Name = "removeColumnToolStripMenuItem";
      removeColumnToolStripMenuItem.Size = new Size(163, 22);
      removeColumnToolStripMenuItem.Text = "Remove Column";
      removeColumnToolStripMenuItem.Click += removeColumnToolStripMenuItem_Click;
      // 
      // removeTableToolStripMenuItem
      // 
      removeTableToolStripMenuItem.Name = "removeTableToolStripMenuItem";
      removeTableToolStripMenuItem.Size = new Size(163, 22);
      removeTableToolStripMenuItem.Text = "Remove Table";
      removeTableToolStripMenuItem.Click += removeTableToolStripMenuItem_Click;
      // 
      // toolStripMenuItem2
      // 
      toolStripMenuItem2.Name = "toolStripMenuItem2";
      toolStripMenuItem2.Size = new Size(160, 6);
      // 
      // saveToolStripMenuItem
      // 
      saveToolStripMenuItem.Name = "saveToolStripMenuItem";
      saveToolStripMenuItem.Size = new Size(163, 22);
      saveToolStripMenuItem.Text = "Save";
      saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
      // 
      // saveAsToolStripMenuItem
      // 
      saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
      saveAsToolStripMenuItem.Size = new Size(163, 22);
      saveAsToolStripMenuItem.Text = "Save As";
      saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
      // 
      // toolStrip1
      // 
      toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripButton1, toolStripButton2 });
      toolStrip1.LayoutStyle = ToolStripLayoutStyle.Flow;
      toolStrip1.Location = new Point(0, 0);
      toolStrip1.Margin = new Padding(4, 0, 0, 0);
      toolStrip1.Name = "toolStrip1";
      toolStrip1.Padding = new Padding(4, 0, 1, 0);
      toolStrip1.Size = new Size(279, 23);
      toolStrip1.TabIndex = 2;
      toolStrip1.Text = "toolStrip1";
      // 
      // toolStripButton1
      // 
      toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
      toolStripButton1.Image = (Image)resources.GetObject("toolStripButton1.Image");
      toolStripButton1.ImageTransparentColor = Color.Magenta;
      toolStripButton1.Name = "toolStripButton1";
      toolStripButton1.Size = new Size(23, 20);
      toolStripButton1.Text = "Add Row";
      // 
      // toolStripButton2
      // 
      toolStripButton2.DisplayStyle = ToolStripItemDisplayStyle.Image;
      toolStripButton2.Image = (Image)resources.GetObject("toolStripButton2.Image");
      toolStripButton2.ImageTransparentColor = Color.Magenta;
      toolStripButton2.Name = "toolStripButton2";
      toolStripButton2.Size = new Size(23, 20);
      toolStripButton2.Text = "Remove Selected Row";
      // 
      // vrMain
      // 
      vrMain.AllowUserToAddRows = false;
      vrMain.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
      vrMain.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
      vrMain.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      vrMain.Dock = DockStyle.Bottom;
      vrMain.Location = new Point(0, 56);
      vrMain.Name = "vrMain";
      vrMain.Size = new Size(279, 209);
      vrMain.TabIndex = 0;
      vrMain.VirtualMode = true;
      vrMain.CellValueNeeded += vrMain_CellValueNeeded;
      vrMain.CellValuePushed += vrMain_CellValuePushed;
      vrMain.ColumnHeaderMouseClick += vrMain_ColumnHeaderMouseClick;
      vrMain.SelectionChanged += vrMain_SelectionChanged;
      // 
      // textBox1
      // 
      textBox1.Dock = DockStyle.Fill;
      textBox1.Location = new Point(0, 0);
      textBox1.Multiline = true;
      textBox1.Name = "textBox1";
      textBox1.Size = new Size(560, 82);
      textBox1.TabIndex = 0;
      // 
      // odMain
      // 
      odMain.CheckFileExists = false;
      odMain.DefaultExt = "pktbls";
      odMain.Filter = "PackedTables|*.pktbls|All files|*.*";
      odMain.Title = "Open Archinve";
      // 
      // saveDialog
      // 
      saveDialog.DefaultExt = "pktbls";
      saveDialog.Filter = "PackedTables|*.pktbls|All files|*.*";
      saveDialog.Title = "Save As";
      // 
      // Form1
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(560, 410);
      Controls.Add(splitContainer1);
      Icon = (Icon)resources.GetObject("$this.Icon");
      Name = "Form1";
      Text = "Form1";
      Shown += Form1_Shown;
      splitContainer1.Panel1.ResumeLayout(false);
      splitContainer1.Panel1.PerformLayout();
      splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
      splitContainer1.ResumeLayout(false);
      splitContainer2.Panel1.ResumeLayout(false);
      splitContainer2.Panel2.ResumeLayout(false);
      splitContainer2.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
      splitContainer2.ResumeLayout(false);
      splitContainer3.Panel1.ResumeLayout(false);
      splitContainer3.Panel2.ResumeLayout(false);
      splitContainer3.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)splitContainer3).EndInit();
      splitContainer3.ResumeLayout(false);
      panel1.ResumeLayout(false);
      panel1.PerformLayout();
      tvMenu.ResumeLayout(false);
      toolStrip1.ResumeLayout(false);
      toolStrip1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)vrMain).EndInit();
      ResumeLayout(false);
    }

    #endregion

    private SplitContainer splitContainer1;
    private Label label1;
    private Button btnBrowse;
    private Button btnOpenClose;
    private ComboBox comboBox1;
    private SplitContainer splitContainer2;
    private SplitContainer splitContainer3;
    private TreeView treeView1;
    private TextBox textBox1;
    private OpenFileDialog odMain;
    private ImageList imageList1;
    private DataGridView vrMain;
    private ContextMenuStrip tvMenu;
    private ToolStripMenuItem addTableToolStripMenuItem;
    private ToolStripMenuItem removeTableToolStripMenuItem;
    private ToolStripMenuItem addColumnToolStripMenuItem;
    private ToolStripMenuItem removeColumnToolStripMenuItem;
    private ToolStripMenuItem addRowToolStripMenuItem;
    private ToolStripMenuItem removeRowToolStripMenuItem;
    private Panel panel1;
    private TextBox tbColumnName;
    private Label label2;
    private ComboBox cbType;
    private Label label3;
    private ToolStripSeparator toolStripMenuItem1;
    private ToolStripSeparator toolStripMenuItem2;
    private ToolStripMenuItem saveToolStripMenuItem;
    private ToolStripMenuItem saveAsToolStripMenuItem;
    private SaveFileDialog saveDialog;
    private ToolStrip toolStrip1;
    private ToolStripButton toolStripButton1;
    private ToolStripButton toolStripButton2;
  }
}
