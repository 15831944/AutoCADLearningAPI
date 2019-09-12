using System;
using System.Windows;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Autodesk.AutoCAD.Geometry;

[assembly: CommandClass(typeof(AutoCADLearningAPI.Class1))]
namespace AutoCADLearningAPI
{
    // the reason that Class1 inherits from IExtensionApplication is that IExt. has pre-constructor methods
        // that we can call like constructors. The same for when the program is terminated.

    public class Class1 : IExtensionApplication
    {
        #region init.
        void IExtensionApplication.Initialize()
        {
            MessageBox.Show("AutoCADLearningAPI loaded successfully.");
        }
        #endregion

        [CommandMethod("Greetings")]
        public void Greetings()
        {
            // Get the current document and database, and start a transaction
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            // Starts a new transaction with the Transaction Manager
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table record for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                             OpenMode.ForRead) as BlockTable;

                // Open the Block table record Model space for write
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                OpenMode.ForWrite) as BlockTableRecord;

                /* Creates a new MText object and assigns it a location,
                text value and text style */
                using (MText objText = new MText())
                {
                    // Specify the insertion point of the MText object
                    objText.Location = new Autodesk.AutoCAD.Geometry.Point3d(10, 10, 0);

                    // Set the text string for the MText object
                    objText.Contents = "Greetings! Welcome to AutoCAD .NET!";

                    // Set the text style for the MText object
                    objText.TextStyleId = acCurDb.Textstyle;

                    // Appends the new MText object to model space
                    acBlkTblRec.AppendEntity(objText);

                    // Appends to new MText object to the active transaction
                    acTrans.AddNewlyCreatedDBObject(objText, true);
                }

                // Saves the changes to the database and closes the transaction
                acTrans.Commit();
            }
        }
        [CommandMethod("DrawLine")]
        public void DrawLine()
        {
            // Get the current document and database, and start a transaction
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            // 'using' a Transaction with the Transaction Manager
            using (Transaction ACDrawLine = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable blockTable = ACDrawLine.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord blockTableRecord = ACDrawLine.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite)
                    as BlockTableRecord;
                Point3d Point1 = new Point3d(0, 0, 0);
                Point3d Point2 = new Point3d(100, 200, 0);
                Line LineBetween = new Line(Point1, Point2);
                blockTableRecord.AppendEntity(LineBetween);
                ACDrawLine.AddNewlyCreatedDBObject(LineBetween, true);
                ACDrawLine.Commit();
            }
        }
        [CommandMethod("DrawUserDefLine")]
        public void DrawUserDefinedLine()
        {

        }
        #region ending
        void IExtensionApplication.Terminate()
        {
        }
        #endregion
    }
}