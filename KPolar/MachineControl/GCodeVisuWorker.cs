/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2019-2021 Sven Hasemann contact: svenhb@web.de

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 * 2020-12-04 new https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.backgroundworker?view=net-5.0
 * 2021-07-08 code clean up / code quality
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace GrblPlotter
{
    public class VisuWorker : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Button cancelAsyncButton;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ProgressBar progressBar2;
        private System.Windows.Forms.Label resultLabel;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;

        // This event handler is where the actual, potentially time-consuming work is done.
        internal IList<string> tmpGCode = new List<string>();

        public VisuWorker()
        {
            InitializeComponent();
            InitializeBackgroundWorker();
            this.Icon = Properties.Resources.Icon;
        }

        // Set up the BackgroundWorker object by attaching event handlers. 
        private void InitializeBackgroundWorker()
        {
            backgroundWorker1.DoWork += new DoWorkEventHandler(BackgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker1_RunWorkerCompleted);
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(BackgroundWorker1_ProgressChanged);
        }

        public void SetTmpGCode(IList<string> tmp)
        {
            tmpGCode = tmp;
            backgroundWorker1.RunWorkerAsync();
        }
        public int SetTmpGCode()
        {
            tmpGCode = Graphic.GCode.ToString().Split(System.Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //    System.IO.File.WriteAllText(Datapath.AppDataFolder + "\\visuworker.nc", Graphic.GCode.ToString()); // clear file
            backgroundWorker1.RunWorkerAsync();
            return tmpGCode.Count;
        }

        private void CancelAsyncButton_Click(System.Object sender, System.EventArgs e)
        {
            // Cancel the asynchronous operation.
            this.backgroundWorker1.CancelAsync();
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            e.Result = VisuGCode.GetGCodeLines(tmpGCode, worker, e);
        }

        // This event handler deals with the results of the background operation.
        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // First, handle the case where an exception was thrown.
            if (e.Error != null)
            { MessageBox.Show(e.Error.Message); }
            else if (e.Cancelled)
            { resultLabel.Text = "Canceled"; }
            else
            { resultLabel.Text = "Finished"; }

            this.Close();
        }

        // This event handler updates the progress bar.
        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar1.Value = e.ProgressPercentage;
            if (e.UserState is MyUserState)
            {
                MyUserState state = e.UserState as MyUserState;
                this.progressBar2.Value = state.Value;
                this.resultLabel.Text = state.Content;
            }
        }


        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.cancelAsyncButton = new System.Windows.Forms.Button();
            this.resultLabel = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            // 
            // cancelAsyncButton
            // 
            this.cancelAsyncButton.Location = new System.Drawing.Point(18, 72);
            this.cancelAsyncButton.Name = "cancelAsyncButton";
            this.cancelAsyncButton.Size = new System.Drawing.Size(256, 23);
            this.cancelAsyncButton.TabIndex = 2;
            this.cancelAsyncButton.Text = "Cancel process";
            this.cancelAsyncButton.Click += new System.EventHandler(this.CancelAsyncButton_Click);
            // 
            // resultLabel
            // 
            this.resultLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.resultLabel.Location = new System.Drawing.Point(18, 8);
            this.resultLabel.Name = "resultLabel";
            this.resultLabel.Size = new System.Drawing.Size(256, 23);
            this.resultLabel.TabIndex = 3;
            this.resultLabel.Text = "";
            this.resultLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(18, 40);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(256, 8);
            this.progressBar1.Step = 2;
            this.progressBar1.TabIndex = 4;
            // 
            // progressBar1
            // 
            this.progressBar2.Location = new System.Drawing.Point(18, 58);
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(256, 8);
            this.progressBar2.Step = 2;
            this.progressBar2.TabIndex = 4;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            // 
            // FibonacciForm
            // 
            this.ClientSize = new System.Drawing.Size(292, 118);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.progressBar2);
            this.Controls.Add(this.resultLabel);
            this.Controls.Add(this.cancelAsyncButton);
            this.Name = "GraphicWorker";
            this.Text = "Process graphic objects";
            this.ResumeLayout(false);
        }
        #endregion
    }
}