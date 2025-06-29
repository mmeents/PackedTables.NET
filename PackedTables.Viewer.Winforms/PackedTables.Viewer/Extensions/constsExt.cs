using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackedTables.Viewer.Extensions {
  public static class ConstExt {
    public const string CommonPathAdd = "\\PackedTables";           // makes a folder named PackedTables on the Desktop
    public const string SettingsAdd = "\\ViewerSettings.pktbls";     // Default file name for settings file    
    public static string DefaultPath {
      get {
        var DefaultDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + ConstExt.CommonPathAdd;
        if (!Directory.Exists(DefaultDir)) {
          Directory.CreateDirectory(DefaultDir);
        }
        return DefaultDir;
      }
    }
    public static string SettingsFileName { get { return DefaultPath + SettingsAdd; } }
  }
}
