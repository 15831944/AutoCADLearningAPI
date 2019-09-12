﻿using System;
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

[assembly: CommandClass(typeof(AutoCADLearningAPI.Class1))]
namespace AutoCADLearningAPI
{
    public class Class1 : IExtensionApplication
    {
        #region init.
        void IExtensionApplication.Initialize()
        {
            MessageBox.Show("This is your first AutoCAD plugin, cheers.");
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
                    objText.Contents = "Greetings, Welcome to AutoCAD .NET";

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
        #region ending
        void IExtensionApplication.Terminate()
        {
        }
        #endregion
    }
}
