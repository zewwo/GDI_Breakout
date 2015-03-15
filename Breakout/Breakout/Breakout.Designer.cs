namespace Breakout
{
    partial class Breakout
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.UI_TSL_Score = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.UI_TSL_Lives = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel6 = new System.Windows.Forms.ToolStripLabel();
            this.UI_TSL_Time = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.UI_L_CtrlStatus = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer
            // 
            this.timer.Interval = 25;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.White;
            this.toolStrip1.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.UI_TSL_Score,
            this.toolStripLabel4,
            this.UI_TSL_Lives,
            this.toolStripLabel6,
            this.UI_TSL_Time,
            this.toolStripLabel1,
            this.toolStripLabel3,
            this.UI_L_CtrlStatus});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(794, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // UI_TSL_Score
            // 
            this.UI_TSL_Score.Name = "UI_TSL_Score";
            this.UI_TSL_Score.Size = new System.Drawing.Size(52, 22);
            this.UI_TSL_Score.Text = "Score : ";
            this.UI_TSL_Score.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(92, 22);
            this.toolStripLabel4.Text = "                            ";
            // 
            // UI_TSL_Lives
            // 
            this.UI_TSL_Lives.Name = "UI_TSL_Lives";
            this.UI_TSL_Lives.Size = new System.Drawing.Size(49, 22);
            this.UI_TSL_Lives.Text = "Lives : ";
            this.UI_TSL_Lives.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripLabel6
            // 
            this.toolStripLabel6.Name = "toolStripLabel6";
            this.toolStripLabel6.Size = new System.Drawing.Size(92, 22);
            this.toolStripLabel6.Text = "                            ";
            // 
            // UI_TSL_Time
            // 
            this.UI_TSL_Time.Name = "UI_TSL_Time";
            this.UI_TSL_Time.Size = new System.Drawing.Size(89, 22);
            this.UI_TSL_Time.Text = "Game State : ";
            this.UI_TSL_Time.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(92, 22);
            this.toolStripLabel1.Text = "                            ";
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(116, 22);
            this.toolStripLabel3.Text = "Controller Status:";
            // 
            // UI_L_CtrlStatus
            // 
            this.UI_L_CtrlStatus.ForeColor = System.Drawing.Color.Green;
            this.UI_L_CtrlStatus.Name = "UI_L_CtrlStatus";
            this.UI_L_CtrlStatus.Size = new System.Drawing.Size(45, 22);
            this.UI_L_CtrlStatus.Text = "status";
            // 
            // Breakout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 572);
            this.Controls.Add(this.toolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Breakout";
            this.Text = "Breakout";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Breakout_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Breakout_KeyUp);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel UI_TSL_Score;
        private System.Windows.Forms.ToolStripLabel UI_TSL_Lives;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStripLabel UI_TSL_Time;
        private System.Windows.Forms.ToolStripLabel toolStripLabel6;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripLabel UI_L_CtrlStatus;
    }
}

