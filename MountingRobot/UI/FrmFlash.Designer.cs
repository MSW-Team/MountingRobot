namespace MountingRobot.UI
{
    partial class FrmFlash
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
            this.picFlash = new System.Windows.Forms.PictureBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblAlarmText = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picFlash)).BeginInit();
            this.SuspendLayout();
            // 
            // picFlash
            // 
            this.picFlash.Location = new System.Drawing.Point(12, 60);
            this.picFlash.Name = "picFlash";
            this.picFlash.Size = new System.Drawing.Size(900, 570);
            this.picFlash.TabIndex = 0;
            this.picFlash.TabStop = false;
            // 
            // btnExit
            // 
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnExit.Location = new System.Drawing.Point(835, 638);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(60, 34);
            this.btnExit.TabIndex = 1;
            this.btnExit.Text = "确认";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lblAlarmText
            // 
            this.lblAlarmText.AutoSize = true;
            this.lblAlarmText.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblAlarmText.Location = new System.Drawing.Point(33, 21);
            this.lblAlarmText.Name = "lblAlarmText";
            this.lblAlarmText.Size = new System.Drawing.Size(94, 21);
            this.lblAlarmText.TabIndex = 2;
            this.lblAlarmText.Text = "报警文本";
            // 
            // FrmFlash
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.ClientSize = new System.Drawing.Size(934, 682);
            this.Controls.Add(this.lblAlarmText);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.picFlash);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmFlash";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmFlash";
            this.Load += new System.EventHandler(this.FrmFlash_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picFlash)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picFlash;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lblAlarmText;
    }
}