using PackedTables;
using PackedTables.Net;
using PackedTables.Viewer.Extensions;
using PackedTables.Viewer.Models;
using System.Collections.Concurrent;
using System.DirectoryServices.ActiveDirectory;
using System.Runtime;
using System.Windows.Forms;
using System.Xml.Linq;


namespace PackedTables.Viewer {
  public partial class Form1 : Form {
    private SettingsModel _settingsPack;
    private string _fileName = "No File Open";
    private PackedTableSet? _workingPack = null;
    private TableModel? _table = null;

    private string OrderByColumnName = "";
    private bool SortAsc = true;
    public Form1() {
      InitializeComponent();
      _settingsPack = new SettingsModel(ConstExt.SettingsFileName);
      ReloadComboBox();
    }

    delegate void SetLogMsgCallback(string msg);
    public void LogMsg(string msg) {
      if (this.textBox1.InvokeRequired) {
        SetLogMsgCallback d = new(LogMsg);
        this.BeginInvoke(d, new object[] { msg });
      } else {
        if (splitContainer2.Panel2Collapsed) { splitContainer2.Panel2Collapsed = false; }
        if (!textBox1.Visible) textBox1.Visible = true;
        this.textBox1.Text = msg + Environment.NewLine + textBox1.Text;
      }
    }

    private void ReloadComboBox() {
      comboBox1.Items.Clear();
      var RecentlyUsedTable = _settingsPack.RecentlyUsed;
      int selectedIndex = -1;
      RecentlyUsedTable.Rows.OrderByDescending(r => r.Value["LastUsed"].Value).ToList().ForEach(row => {
        if (row.Value["FileName"].Value == _fileName) {
          selectedIndex = comboBox1.Items.Add(row.Value["FileName"].Value);
        } else {
          comboBox1.Items.Add(row.Value["FileName"].Value);
        }
      });
      if (comboBox1.Items.Count > 0) {
        comboBox1.SelectedIndex = selectedIndex == -1 ? 0 : selectedIndex;
        btnOpenClose.Enabled = true;
      } else {
        comboBox1.Enabled = false;
        btnOpenClose.Enabled = false;
      }
    }

    private void btnBrowse_Click(object sender, EventArgs e) {
      if (_fileName == "No File Open") {
        odMain.InitialDirectory = ConstExt.DefaultPath;
      } else {
        string s = _fileName.ParseLast("\\");
        odMain.InitialDirectory = _fileName.Substring(0, _fileName.Length - s.Length);
      }
      DialogResult res = odMain.ShowDialog();
      if (res == DialogResult.OK) {
        _fileName = odMain.FileName;
        DoOpenFileTable();
      }
    }

    private void DoOpenFileTable() {
      if (string.IsNullOrEmpty(_fileName)) return;
      try {
        LogMsg($"{DateTime.Now} loading {_fileName}");
        _workingPack = new PackedTableSet(_fileName);
        LoadTreeViewFromWorkingPack();
      } catch (Exception ex) {
        LogMsg("Error opening file: " + ex.Message);
        MessageBox.Show("Error opening file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }
      AddFileToMRUL(_fileName);
      ReloadComboBox();
      label1.Text = $"Open: {_fileName}";
      this.Text = "Viewing " + _fileName;
      btnOpenClose.Text = "Close";
      btnBrowse.Visible = false;
      comboBox1.Visible = false;
    }

    private void DoCloseFileTable() {
      btnOpenClose.Text = "Open";
      btnBrowse.Visible = true;
      comboBox1.Visible = true;
      label1.Text = "Open:";
      treeView1.Nodes.Clear();
      ClearVrMain();
      LogMsg(_fileName + " closed.");
    }


    private void btnOpenClose_Click(object sender, EventArgs e) {
      if (btnOpenClose.Text == "Open") {
        _fileName = comboBox1.SelectedItem?.ToString();
        DoOpenFileTable();
      } else if (btnOpenClose.Text == "Close") {
        _workingPack = null;
        DoCloseFileTable();
      }
    }

    private void AddFileToMRUL(string fileName) {
      if (string.IsNullOrEmpty(_fileName)) return;

      var existingRow = _settingsPack.RecentlyUsed.FindFirst("FileName", _fileName);

      if (existingRow) {
        var row = _settingsPack.RecentlyUsed.Current;
        if (row != null) {
          row["LastUsed"].Value = DateTime.Now;
        }
      } else {
        var newRow = _settingsPack.RecentlyUsed.AddRow();
        newRow["FileName"].Value = _fileName;
        newRow["LastUsed"].Value = DateTime.Now;
      }
      _settingsPack.Save();
    }

    private void LoadTreeViewFromWorkingPack() {
      if (_workingPack == null) return;
      treeView1.Nodes.Clear();
      var packfilename = _fileName.ParseLast("\\");
      var rootnode = new TreeNode(packfilename);
      rootnode.ImageIndex = 0;
      rootnode.SelectedImageIndex = 0;
      rootnode.Tag = _workingPack;
      treeView1.Nodes.Add(rootnode);
      var tables = _workingPack.GetTables();
      foreach (var table in tables) {
        var node = new TreeNode(table.Name);
        node.ImageIndex = 1;
        node.Tag = table;
        node.SelectedImageIndex = 1;
        table.Columns.Values.OrderBy(x => x.Rank).ToList().ForEach(col => {
          var colNode = new TreeNode(col.ColumnName);
          colNode.ImageIndex = 2;
          colNode.SelectedImageIndex = 2;
          colNode.Tag = col;
          node.Nodes.Add(colNode);
        });
        rootnode.Nodes.Add(node);
      }
      treeView1.ExpandAll();
    }

    private void Form1_Shown(object sender, EventArgs e) {
      LogMsg($"PackedTables.Viewer started {DateTime.Now}");
    }

    private void treeView1_AfterSelect(object sender, TreeViewEventArgs e) {
      var target = e.Node;
      if (target == null || target.Tag == null) return;
      if (target.Tag is TableModel table) {
        LogMsg($"Selected table: {table.Name}");
        LoadDataGridViewFromTable(table);
      }
    }

    private void ClearVrMain() {
      if (vrMain.Rows.Count > 0) { vrMain.Rows.Clear(); }
      if (vrMain.Columns.Count > 0) { vrMain.Columns.Clear(); }
    }

    private void LoadDataGridViewFromTable(TableModel table) {
      if (table == null) return;
      _table = table;
      if (vrMain.Enabled) vrMain.Enabled = false;
      ClearVrMain();

      foreach (var col in _table.Columns.Values.OrderBy(x => x.Rank)) {
        var addedId = vrMain.Columns.Add(col.ColumnName, col.ColumnName);
        if (col.ColumnName == OrderByColumnName) {
          if (SortAsc) {
            vrMain.Columns[addedId].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
          } else {
            vrMain.Columns[addedId].HeaderCell.SortGlyphDirection = SortOrder.Descending;
          }
        } else {
          vrMain.Columns[addedId].HeaderCell.SortGlyphDirection = SortOrder.None;
        }
      }
      vrMain.RowCount = _table.Rows.Count;
      _table.OrderByColumnName = OrderByColumnName;
      vrMain.Enabled = true;
      if (!vrMain.Visible) { vrMain.Visible = true; }
    }

    private void vrMain_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e) {
      try {
        var columnName = vrMain.Columns[e.ColumnIndex].Name;
        if (_table.RowIndex.TryGetValue(e.RowIndex, out int RowId)) {
          if (_table.Rows.TryGetValue(RowId, out var row)) {
            e.Value = row?[columnName]?.Value ?? "";

          } else {
            e.Value = "";

          }
        } else {
          e.Value = "";
        }
      } catch (Exception ex) {
        LogMsg(ex.Message);
      }
    }

    private void vrMain_CellValuePushed(object sender, DataGridViewCellValueEventArgs e) {
      try {
        var columnName = vrMain.Columns[e.ColumnIndex].Name;
        if (_table != null && _table.RowIndex.TryGetValue(e.RowIndex, out var tblIndex) && tblIndex != 0) {
          if (_table.Rows.TryGetValue(tblIndex, out var row)) {
            if (row != null) {
              row[columnName].Value = e.Value ?? "";
            }
          }
        }
      } catch (Exception ex) {
        LogMsg(ex.Message);
      }
    }

    private void vrMain_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
      var columnName = vrMain.Columns[e.ColumnIndex].Name;
      if (columnName == OrderByColumnName) {
        SortAsc = !SortAsc;
        var col = vrMain.Columns[e.ColumnIndex];
      } else {
        OrderByColumnName = columnName;
      }
      LoadDataGridViewFromTable(_table!);
    }

    private void tvMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e) {
      var target = treeView1.SelectedNode;
      if (target == null || target.Tag == null) {
        e.Cancel = true;
        return;
      }
      if (target.Tag is PackedTableSet pack) {
        addTableToolStripMenuItem.Visible = true;
        addColumnToolStripMenuItem.Visible = false;
        addRowToolStripMenuItem.Visible = false;
        removeTableToolStripMenuItem.Visible = false;
        removeColumnToolStripMenuItem.Visible = false;
        removeRowToolStripMenuItem.Visible = false;
      }
      if (target.Tag is TableModel table) {
        addTableToolStripMenuItem.Visible = true;
        addColumnToolStripMenuItem.Visible = true;
        addRowToolStripMenuItem.Visible = true;
        removeTableToolStripMenuItem.Visible = true;
        removeColumnToolStripMenuItem.Visible = false;
        removeRowToolStripMenuItem.Visible = false;
      }
      if (target.Tag is ColumnModel column) {
        addTableToolStripMenuItem.Visible = false;
        addColumnToolStripMenuItem.Visible = true;
        addRowToolStripMenuItem.Visible = true;
        removeTableToolStripMenuItem.Visible = true;
        removeColumnToolStripMenuItem.Visible = true;
        removeRowToolStripMenuItem.Visible = false;
      }

    }

    private void addTableToolStripMenuItem_Click(object sender, EventArgs e) {
      if (_workingPack == null) return;
      string newTableName = $"Table{_workingPack.TableCount+1}";
      var newTable = _workingPack.AddTable(newTableName);
      newTable.AddColumn("Id", ColumnType.Int32);
      LoadTreeViewFromWorkingPack();
    }
  }
}
