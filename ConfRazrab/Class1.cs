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
using System.Net;
#endif


namespace ConfRazrab
{
    public class start
    {
        public const double  LINE = 10;
        //задаем начальную точку
        double startX = 0;
        double startY = 0;

        //считаем конечную точку
        double endX;
        double endY;

        Editor ed1 = Application.DocumentManager.MdiActiveDocument.Editor;
        Editor ed2 = Application.DocumentManager.MdiActiveDocument.Editor;

        [CommandMethod("qwe")]
        public void loop()
        {
           // begin();
         //   AddLine(startX, startY);
            AddDimenssion(startX, startY,"250");
            startX = endX;
            startY = endY;

        //    AddBlock(endX, endY);
        //    AddLine(startX, startY);
         //   startX = endX;
        //    startY = endY;
         //   AddBlock(endX, endY);

            //рисуем линию


            //Эээээксперименты


            //AddLine(startX, startY, endX, endY);
        }
        public void AddLine(double startPointX, double startPointY)
        {
            //спрашиваем угол
            PromptIntegerOptions pio = new PromptIntegerOptions("Введи угол или я введу в твое очко ");
            PromptIntegerResult pir = ed2.GetInteger(pio);

            double ugol = pir.Value;

            //считаем конечную точку
            endX = startX + LINE * Math.Cos((Math.PI * ugol) / 180);
            endY = startY + LINE * Math.Sin((Math.PI * ugol) / 180);


            var database = Application.DocumentManager.MdiActiveDocument.Database;
            using (var trans = database.TransactionManager.StartTransaction())
            {
                BlockTable bt = trans.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord ms = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                //var polyLine = new Polyline2d();
                var line = new Line(new Point3d(startPointX, startPointY, 0), new Point3d(endX, endY, 0));
                //var line2 = new Line();
                ms.AppendEntity(line);
                trans.Commit();
            }
        }
        public void AddBlock(double endPointX, double endPointY)
        {
            // Получаем ссылки на активный документ
            Document doc = Application.DocumentManager.MdiActiveDocument;

            // Получаем ссылки на редактор докумена
            Editor ed = doc.Editor;

            // Получаем ссылки на базу данных документа
            Database db = doc.Database;

            // Начало транзакции с базой данных документа
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                // Открываем таблицу блоков для чтения
                BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

                // Перебираем таблицу блоков в цикле
                foreach (ObjectId btr in blockTable)
                {
                    // Открываем каждую запись таблицы
                    BlockTableRecord obj = trans.GetObject(btr, OpenMode.ForRead) as BlockTableRecord;

                    if (obj.Name == "11")
                    {
                        // Добавляем втсавку блока с атрибутами в пространство модели
                        BlockTableRecord modelSpace = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                        ObjectId btrID = obj.ObjectId;
                        // Создаем вставку блока для добавления ее в пространство модели
                        BlockReference blockReference = new BlockReference(new Point3d(endPointX, endPointY, 0), btrID);                       
                        modelSpace.AppendEntity(blockReference);
                    }                          
                }

                // Завершаем транзакцию
                trans.Commit();
            }
        }
        public void begin()
        {
            //спрашиваем где точка
            PromptPointResult pPtRes;
            PromptPointOptions pPtOpts = new PromptPointOptions("Укажите точку: ");
            pPtOpts.Message = "\nУкажите точку: ";

            //считываем точку
            pPtRes = ed1.GetPoint(pPtOpts);

            //задаем начальную точку
            startX = pPtRes.Value.X;
            startY = pPtRes.Value.Y;


        }
        public void endPoint()
        {
        }
        public void AddDimenssion(double startPointX, double startPointY, string number)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
          //  Database db = HostApplicationServices.WorkingDatabase;
            Transaction tr = db.TransactionManager.StartTransaction();
            using (tr)
            {
                // Open our dimension style table to add our
                // new dimension style
                DimStyleTable dst = tr.GetObject(db.DimStyleTableId, OpenMode.ForRead) as DimStyleTable;

  //              foreach (Teigha.DatabaseServices.ObjectId btr in blockTable)
  //              {
  //                  // Открываем каждую запись таблицы
  //                  BlockTableRecord obj = trans.GetObject(btr, Teigha.DatabaseServices.OpenMode.ForRead) as BlockTableRecord;
  //                  // Выводим в командную строку
  //                 ed1.WriteMessage($"\nTable record name and type: {obj.Name} , {obj.GetType()} ");
  //              }



                foreach (ObjectId dimStyleID in dst)
                {
                    // Открываем каждую запись таблицы
            //        BlockTableRecord obj = tr.GetObject(btr, OpenMode.ForRead) as BlockTableRecord;
                    DimStyleTableRecord dstr = tr.GetObject(dimStyleID,OpenMode.ForRead) as DimStyleTableRecord;

                    ed1.WriteMessage($"\nDimension style name: {dstr.Name} , {dstr.GetType} ");

  //                  if (dstr.Name == "ISO-25")
  //                  {
  //                   //   ObjectId dsId = dst.Add(dstr);
  //
  //                      ObjectId dsID = dstr.ObjectId;
  //                      // Добавляем втсавку блока с атрибутами в пространство модели
  //                      //  BlockTableRecord modelSpace = tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
  //                      BlockTableRecord modelSpace = tr.GetObject(dst[BlockTableRecord.ModelSpace], OpenMode.ForWrite)as BlockTableRecord;
  //                      AlignedDimension ad1 = new AlignedDimension(Point3d.Origin, new Point3d(5.0, 0.0, 0.0), new Point3d(2.5, 2.0, 0.0), number, dsID);

                        //    ObjectId btrID = obj.ObjectId;
                        // Создаем вставку блока для добавления ее в пространство модели
                        //    BlockReference blockReference = new BlockReference(new Point3d(endPointX, endPointY, 0), btrID);

                        // Now let's open the current space and add our two
                        // dimensions
 //                       BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
 //                       btr.AppendEntity(ad1);
 //                       tr.AddNewlyCreatedDBObject(ad1, true);

                     //   modelSpace.AppendEntity(ad1);
 //                   }
                }

                // Now create two identical dimensions, one
                // next to the other, using our dimension
                // style





                // And commit the transaction, of course
                tr.Commit();
            }
        }

        private DBObjectCollection SquareOfLines(double size)
        {
            // A function to generate a set of entities for our block

            DBObjectCollection ents = new DBObjectCollection();

            Point3d[] pts = { new Point3d(-size, -size, 0), new Point3d(size, -size, 0), new Point3d(size, size, 0), new Point3d(-size, size, 0) };

            int max = pts.GetUpperBound(0);

            for (int i = 0; i <= max; i++)

            {
                int j = (i == max ? 0 : i + 1);
                Line ln = new Line(pts[i], pts[j]);
                ents.Add(ln);
            }
            return ents;

        }





    }


}
