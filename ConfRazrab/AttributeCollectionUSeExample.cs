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
    public partial class Commands
    {
        [CommandMethod("ATTRIBUTECOLLECTIONREADING")]
        public void AttributeCollectionUSeExample()
        {
            // Получение ссылки на активный документ
            Document doc = Application.DocumentManager.MdiActiveDocument;

            // Получение ссылки на редактор докумена
            Editor ed = doc.Editor;

            // Получение ссылки на базу данных документа
            Database db = doc.Database;

            // Создаем определение блока, к которому будем добавлять атрибуты.
            // В блоке будет фигура крест-круг, состоящая из двух полилиний и окружности:
            BlockTableRecord btr = new BlockTableRecord();
            btr.Name = "TestBlock";
            Polyline polyline = new Polyline();
            polyline.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0);
            polyline.AddVertexAt(1, new Point2d(1000, 1000), 0, 0, 0);

            Polyline polyline1 = new Polyline();
            polyline1.AddVertexAt(0, new Point2d(0, 1000), 0, 0, 0);
            polyline1.AddVertexAt(1, new Point2d(1000, 0), 0, 0, 0);

            Circle circle = new Circle();
            circle.Center = new Point3d(500, 500, 0);
            circle.Radius = 500;

            btr.AppendEntity(polyline);
            btr.AppendEntity(polyline1);
            btr.AppendEntity(circle);

            // Начинаем транзакцию с базой данных для добавления нового блока в чертеж
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                // Добавляем определение нового блока в базу данных чертежа,
                // предварительно проверив, нет ли блока с таким же именем
                BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
                ObjectId btrID = default;
                if (!blockTable.Has(btr.Name)) btrID = blockTable.Add(btr);

                // Создаем вставку блока для добавления ее в пространство модели
                BlockReference blockReference = new BlockReference(new Point3d(0, 0, 0), btrID);

                // Создаем атрибуты для нашего блока
                AttributeReference attribute = new AttributeReference(new Point3d(blockReference.Position.X + 150, 1150, 0), "Cross-circle", "Block name", default);
                attribute.Height = 100;
                AttributeReference attribute1 = new AttributeReference(new Point3d(attribute.Position.X, 1000, 0), circle.Diameter.ToString(), "Size", default);
                attribute1.Height = 100;

                // Добавляем атрибуты в коллекцию атрибутов вставки блока
                AttributeCollection attributeCollection = blockReference.AttributeCollection;
                attributeCollection.AppendAttribute(attribute);
                attributeCollection.AppendAttribute(attribute1);

                // Добавляем втсавку блока с атрибутами в пространство модели
                BlockTableRecord modelSpace = trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                modelSpace.AppendEntity(blockReference);

                // Проверяем коллекцию атрибутов блока, выводя её параметры в командную строку nanoCAD
                ed.WriteMessage($"Attribute collection has {attributeCollection.Count} items:");
                foreach (ObjectId attrId in attributeCollection)
                {
                    AttributeReference attribute_ = trans.GetObject(attrId, OpenMode.ForRead) as AttributeReference;
                    ed.WriteMessage($"{attribute_.Tag} : {attribute_.TextString}");
                }

                // Завершаем транзакцию
                trans.Commit();
            }
        }
    }
}