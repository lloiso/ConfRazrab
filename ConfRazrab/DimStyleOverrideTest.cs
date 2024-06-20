#if ACAD
  using Autodesk.AutoCAD.Runtime;
  using Autodesk.AutoCAD.ApplicationServices;
  using Autodesk.AutoCAD.DatabaseServices;
  using Autodesk.AutoCAD.EditorInput;
  using Autodesk.AutoCAD.Geometry;

  using Platform = Autodesk.AutoCAD;
  using PlatformDb = Autodesk.AutoCAD;
#else
using Teigha.DatabaseServices;
using Teigha.Runtime;
using Teigha.Geometry;
using HostMgd.ApplicationServices;
using HostMgd.EditorInput;
using Platform = HostMgd;
using PlatformDb = Teigha;
using System.Diagnostics;
#endif


namespace ConfRazrab
{
    public class Commands4
    {
        [CommandMethod("ODS")]
        public void OverrideDimensionStyle()
        {
            Database db =HostApplicationServices.WorkingDatabase;
            Transaction tr =db.TransactionManager.StartTransaction();
            using (tr)
            {
                // Open our dimension style table to add our
                // new dimension style
                DimStyleTable dst =(DimStyleTable)tr.GetObject(db.DimStyleTableId, OpenMode.ForWrite);

                // Create our new dimension style
                DimStyleTableRecord dstr = new DimStyleTableRecord();
                dstr.Dimtad = 2;
                dstr.Dimgap = 0.3;
                dstr.Name = "MyStyle";

                // Add it to the dimension style table
                ObjectId dsId = dst.Add(dstr);
                tr.AddNewlyCreatedDBObject(dstr, true);

                // Now create two identical dimensions, one
                // next to the other, using our dimension
                // style
                AlignedDimension ad1 = new AlignedDimension( Point3d.Origin, new Point3d(5.0, 0.0, 0.0), new Point3d(2.5, 2.0, 0.0), "Standard dimension",dsId);

                // The only thing we change is the text string
                AlignedDimension ad2 = new AlignedDimension(new Point3d(5.0, 0.0, 0.0),new Point3d(10.0, 0.0, 0.0),new Point3d(7.5, 2.0, 0.0), "Overridden dimension", dsId);

                // Isn't this easier?
                ad2.Dimtad = 4;
                ad2.Dimgap = 0.5;

                // Now let's open the current space and add our two
                // dimensions
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject( db.CurrentSpaceId, OpenMode.ForWrite);
                btr.AppendEntity(ad1);
                btr.AppendEntity(ad2);
                tr.AddNewlyCreatedDBObject(ad1, true);
                tr.AddNewlyCreatedDBObject(ad2, true);

                // And commit the transaction, of course
                tr.Commit();
            }
        }
    }
}
