using PackedTables.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackedTables.Extensions {
  public static class PackedTablesExt {
    public static PackedTables LoadFromFile(this PackedTables packedTables, string fileName) {
      if (File.Exists(fileName)) {
        var encoded = Task.Run(async () => await fileName.ReadAllTextAsync().ConfigureAwait(false)).GetAwaiter().GetResult();
        packedTables.LoadFromBase64String(encoded);
      }
      return packedTables;
    }

    public static void SaveToFile(this PackedTables packedTables, string fileName) {
      var base64 = packedTables.SaveToBase64String();
      Task.Run(async () => await base64.WriteAllTextAsync(fileName).ConfigureAwait(false)).GetAwaiter().GetResult();
    }

    public static string SaveToBase64String(this PackedTables packedTables) {
      var encoded = MessagePack.MessagePackSerializer.Serialize(packedTables.Package);
      var base64 = Convert.ToBase64String(encoded);
      return base64;
    }

    public static PackedTables LoadFromBase64String(this PackedTables packedTables, string base64) {
      if (base64 == null || base64.Length == 0) {
        packedTables.Package = new DataSetPackage();
      } else {
        var decoded = Convert.FromBase64String(base64);
        packedTables.Package = MessagePack.MessagePackSerializer.Deserialize<DataSetPackage>(decoded);
      }
      return packedTables;
    }

    public static string SaveToJson(this PackedTables packedTables) {
      return MessagePack.MessagePackSerializer.SerializeToJson(packedTables.Package);
    }

    public static PackedTables LoadFromJson(this PackedTables packedTables, string json) {
      if (json == null || json.Length == 0) {
        packedTables.Package = new DataSetPackage();
      } else {
        var byteArray = MessagePack.MessagePackSerializer.ConvertFromJson(json);

        packedTables.Package = MessagePack.MessagePackSerializer.Deserialize<DataSetPackage>(byteArray);
      }
      return packedTables;
    }

  }

}
