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
using Autodesk.AutoCAD.EditorInput;

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
            // Get the current document and database, and start a transaction
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            // 'using' a Transaction with the Transaction Manager
            using (Transaction Trans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable BlkTbl = Trans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord BlkTblRcrd = Trans.GetObject(BlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) 
                    as BlockTableRecord;
                // this is the point prompter
                PromptPointOptions prPtOpt = new PromptPointOptions("\nSpecify start point: ");
                prPtOpt.AllowArbitraryInput = false;
                prPtOpt.AllowNone = true;
                // an object that represents the screen, as it were
                Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
                // save the result of the first point selection
                PromptPointResult prPtRes1 = ed.GetPoint(prPtOpt);
                // if the selected point isn't valid, break
                if (prPtRes1.Status != PromptStatus.OK) return;
                // if it WAS valid, save that selected point as a ... point
                Point3d pnt1 = prPtRes1.Value;
                // save this as the base point
                prPtOpt.BasePoint = pnt1;
                // starting from here
                prPtOpt.UseBasePoint = true;
                // shows a dashed line between the base point and the second prospective point
                prPtOpt.UseDashedLine = true;
                // prompter message
                prPtOpt.Message = "\nSpecify end point:";
                // save end result
                PromptPointResult prPtRes2 = ed.GetPoint(prPtOpt);
                // if not OK, break
                if (prPtRes2.Status != PromptStatus.OK) return;
                // otherwise save that point as a Point
                Point3d pnt2 = prPtRes2.Value;
                // draw a line between them
                Line LineBetween = new Line(pnt1, pnt2);
                BlkTblRcrd.AppendEntity(LineBetween);
                Trans.AddNewlyCreatedDBObject(LineBetween, true);
                Trans.Commit();
            }
        }
        [CommandMethod("PlaceTestDWG")]
        public void PlaceTestDWG()
        {
            Database database = Application.DocumentManager.MdiActiveDocument.Database;
            using (Transaction acTrans = database.TransactionManager.StartTransaction())
            {
                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;

                ObjectId blkRecId = ObjectId.Null;
                if (!acBlkTbl.Has("CircleBlock"))
                {
                    using (BlockTableRecord acBlkTblRec = new BlockTableRecord())
                    {
                        acBlkTblRec.Name = "CircleBlock";

                        // why the hell is this even an option?
                        acBlkTblRec.Origin = new Point3d(0, 0, 0);

                        // Add a circle to the block
                        using (Circle acCirc = new Circle())
                        {
                            acCirc.Center = new Point3d(12.5, 6.25, 0);
                            acCirc.Radius = 25;

                            acBlkTblRec.AppendEntity(acCirc);

                            acBlkTbl.UpgradeOpen();
                            acBlkTbl.Add(acBlkTblRec);
                            acTrans.AddNewlyCreatedDBObject(acBlkTblRec, true);
                        }

                        blkRecId = acBlkTblRec.Id;
                    }
                }
                else
                {
                    blkRecId = acBlkTbl["CircleBlock"];
                }

                // Insert the block into the current space
                if (blkRecId != ObjectId.Null)
                {
                    using (BlockReference acBlkRef = new BlockReference(new Point3d(0, 0, 0), blkRecId))
                    {
                        BlockTableRecord acCurSpaceBlkTblRec;
                        acCurSpaceBlkTblRec = acTrans.GetObject(database.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

                        acCurSpaceBlkTblRec.AppendEntity(acBlkRef);
                        acTrans.AddNewlyCreatedDBObject(acBlkRef, true);
                    }
                }

                // Save the new object to the database
                acTrans.Commit();

                // Dispose of the transaction
            }
        }
        [CommandMethod("HAI")]
        public void HelloWorld()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("Salutations unto thee, o brave warrior of computer-aided design.");
            System.Threading.Thread.Sleep(500);
            MessageBox.Show("Salutations unto thee, o brave warrior of computer-aided design.");
        }
        // the Terminate method for IExtensionApplication
        // here, it does nothing, but it still needs to be defined
        void IExtensionApplication.Terminate() { }
    }
}