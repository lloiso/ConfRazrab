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
    public class Commands2
    {
        [CommandMethod("CalcBlocks1")]
        static public void CalcBlocks1()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
         //   Database db = HostApplicationServices.WorkingDatabase;
            PromptResult res = ed.GetString("\nType name of block: ");

            if (res.Status == PromptStatus.OK)
            {
                TypedValue[] flt =
                {
                  //  new TypedValue(0, "INSERT"),
                    new TypedValue(2, res.StringResult),
                 //   new TypedValue(410, "Model"),
                 //   new TypedValue(1, "")
                };

                PromptSelectionResult rs = ed.SelectAll(new SelectionFilter(flt));

                if (rs.Status == PromptStatus.OK && rs.Value.Count > 0)
                {
                    ed.WriteMessage("\nNumber of blocks with name <{0}> in Model Space is {1}", res.StringResult, rs.Value.Count);
                }
                else
                {
                    ed.WriteMessage("\nNo blocks with name <{0}>", res.StringResult);
                }
            }
        }
    }
}
