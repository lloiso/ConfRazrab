using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HostMgd.ApplicationServices;
using HostMgd.EditorInput;
using Teigha.DatabaseServices;
using Teigha.Runtime;

namespace ConfRazrab
{
    public class Commands3
    {
        [CommandMethod("qq")]
        public static void Test()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            var pr = ed.GetString("\nEnter Handle: ");
            if (pr.Status != PromptStatus.OK)
                return;
            if (long.TryParse(
                pr.StringResult,
                System.Globalization.NumberStyles.HexNumber,
                System.Globalization.CultureInfo.CurrentCulture,
                out long value))
            {
                if (db.TryGetObjectId(new Handle(value), out ObjectId id))
                {
                    if (id.ObjectClass.IsDerivedFrom(RXObject.GetClass(typeof(Entity))))
                    {
                        using (var tr = db.TransactionManager.StartTransaction())
                        {
                            var entity = (Entity)tr.GetObject(id, OpenMode.ForWrite);
                            entity.Highlight();
                            tr.Commit();
                        }
                    }
                    else
                    {
                        ed.WriteMessage("\nNot an entity Handle.");
                    }
                }
                else
                {
                    ed.WriteMessage("\nInvalid Handle.");
                }
            }
            else
            {
                ed.WriteMessage("\nNot an hex number");
            }
        }
    }
}
