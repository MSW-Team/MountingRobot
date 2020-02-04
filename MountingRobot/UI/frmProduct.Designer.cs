namespace MountingRobot.UI
{
    partial class FrmProduct
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
            this.cobASourceName = new System.Windows.Forms.ComboBox();
            this.btnAAddNew = new System.Windows.Forms.Button();
            this.txtATargetName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnADelete = new System.Windows.Forms.Button();
            this.cobAProductDelete = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.timerMessage = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.cobBSourceName = new System.Windows.Forms.ComboBox();
            this.btnBAddNew = new System.Windows.Forms.Button();
            this.txtBTargetName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cobBProductDelete = new System.Windows.Forms.ComboBox();
            this.btnBDelete = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // cobASourceName
            // 
            this.cobASourceName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.cobASourceName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cobASourceName.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cobASourceName.ForeColor = System.Drawing.Color.White;
            this.cobASourceName.FormattingEnabled = true;
            this.cobASourceName.Location = new System.Drawing.Point(147, 36);
            this.cobASourceName.Name = "cobASourceName";
            this.cobASourceName.Size = new System.Drawing.Size(121, 24);
            this.cobASourceName.TabIndex = 0;
            // 
            // btnAAddNew
            // 
            this.btnAAddNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAAddNew.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnAAddNew.Location = new System.Drawing.Point(179, 120);
            this.btnAAddNew.Name = "btnAAddNew";
            this.btnAAddNew.Size = new System.Drawing.Size(75, 36);
            this.btnAAddNew.TabIndex = 2;
            this.btnAAddNew.Text = "新增";
            this.btnAAddNew.UseVisualStyleBackColor = true;
            this.btnAAddNew.Click += new System.EventHandler(this.btnAAddNew_Click);
            // 
            // txtATargetName
            // 
            this.txtATargetName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.txtATargetName.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtATargetName.ForeColor = System.Drawing.Color.White;
            this.txtATargetName.Location = new System.Drawing.Point(147, 80);
            this.txtATargetName.Name = "txtATargetName";
            this.txtATargetName.Size = new System.Drawing.Size(121, 26);
            this.txtATargetName.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(10, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "A轨源产品名称:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(10, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(136, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "A轨新增产品名称:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(10, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 16);
            this.label4.TabIndex = 9;
            this.label4.Text = "A轨产品名称:";
            // 
            // btnADelete
            // 
            this.btnADelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnADelete.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnADelete.Location = new System.Drawing.Point(146, 105);
            this.btnADelete.Name = "btnADelete";
            this.btnADelete.Size = new System.Drawing.Size(75, 36);
            this.btnADelete.TabIndex = 8;
            this.btnADelete.Text = "删除";
            this.btnADelete.UseVisualStyleBackColor = true;
            this.btnADelete.Click += new System.EventHandler(this.btnADelete_Click);
            // 
            // cobAProductDelete
            // 
            this.cobAProductDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.cobAProductDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cobAProductDelete.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cobAProductDelete.ForeColor = System.Drawing.Color.White;
            this.cobAProductDelete.FormattingEnabled = true;
            this.cobAProductDelete.Location = new System.Drawing.Point(116, 51);
            this.cobAProductDelete.Name = "cobAProductDelete";
            this.cobAProductDelete.Size = new System.Drawing.Size(121, 24);
            this.cobAProductDelete.TabIndex = 6;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cobBSourceName);
            this.groupBox1.Controls.Add(this.btnBAddNew);
            this.groupBox1.Controls.Add(this.txtBTargetName);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cobASourceName);
            this.groupBox1.Controls.Add(this.btnAAddNew);
            this.groupBox1.Controls.Add(this.txtATargetName);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(12, 23);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(304, 319);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "新增产品";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cobBProductDelete);
            this.groupBox2.Controls.Add(this.btnBDelete);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.cobAProductDelete);
            this.groupBox2.Controls.Add(this.btnADelete);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(344, 23);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(304, 319);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "删除产品";
            // 
            // btnExit
            // 
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnExit.Location = new System.Drawing.Point(558, 373);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 36);
            this.btnExit.TabIndex = 12;
            this.btnExit.Text = "退出";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblMessage.Location = new System.Drawing.Point(12, 373);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(72, 16);
            this.lblMessage.TabIndex = 13;
            this.lblMessage.Text = "消息显示";
            // 
            // timerMessage
            // 
            this.timerMessage.Interval = 1000;
            this.timerMessage.Tick += new System.EventHandler(this.timerMessage_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(10, 180);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 16);
            this.label1.TabIndex = 9;
            this.label1.Text = "B轨源产品名称:";
            // 
            // cobBSourceName
            // 
            this.cobBSourceName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.cobBSourceName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cobBSourceName.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cobBSourceName.ForeColor = System.Drawing.Color.White;
            this.cobBSourceName.FormattingEnabled = true;
            this.cobBSourceName.Location = new System.Drawing.Point(147, 176);
            this.cobBSourceName.Name = "cobBSourceName";
            this.cobBSourceName.Size = new System.Drawing.Size(121, 24);
            this.cobBSourceName.TabIndex = 6;
            // 
            // btnBAddNew
            // 
            this.btnBAddNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBAddNew.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnBAddNew.Location = new System.Drawing.Point(179, 260);
            this.btnBAddNew.Name = "btnBAddNew";
            this.btnBAddNew.Size = new System.Drawing.Size(75, 36);
            this.btnBAddNew.TabIndex = 7;
            this.btnBAddNew.Text = "新增";
            this.btnBAddNew.UseVisualStyleBackColor = true;
            this.btnBAddNew.Click += new System.EventHandler(this.btnBAddNew_Click);
            // 
            // txtBTargetName
            // 
            this.txtBTargetName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.txtBTargetName.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtBTargetName.ForeColor = System.Drawing.Color.White;
            this.txtBTargetName.Location = new System.Drawing.Point(147, 220);
            this.txtBTargetName.Name = "txtBTargetName";
            this.txtBTargetName.Size = new System.Drawing.Size(121, 26);
            this.txtBTargetName.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(10, 225);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(136, 16);
            this.label5.TabIndex = 10;
            this.label5.Text = "B轨新增产品名称:";
            // 
            // cobBProductDelete
            // 
            this.cobBProductDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.cobBProductDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cobBProductDelete.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cobBProductDelete.ForeColor = System.Drawing.Color.White;
            this.cobBProductDelete.FormattingEnabled = true;
            this.cobBProductDelete.Location = new System.Drawing.Point(116, 193);
            this.cobBProductDelete.Name = "cobBProductDelete";
            this.cobBProductDelete.Size = new System.Drawing.Size(121, 24);
            this.cobBProductDelete.TabIndex = 10;
            // 
            // btnBDelete
            // 
            this.btnBDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBDelete.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnBDelete.Location = new System.Drawing.Point(146, 249);
            this.btnBDelete.Name = "btnBDelete";
            this.btnBDelete.Size = new System.Drawing.Size(75, 36);
            this.btnBDelete.TabIndex = 11;
            this.btnBDelete.Text = "删除";
            this.btnBDelete.UseVisualStyleBackColor = true;
            this.btnBDelete.Click += new System.EventHandler(this.btnBDelete_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(10, 197);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 16);
            this.label6.TabIndex = 12;
            this.label6.Text = "B轨产品名称:";
            // 
            // FrmProduct
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.ClientSize = new System.Drawing.Size(660, 420);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmProduct";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmProduct";
            this.Load += new System.EventHandler(this.frmProduct_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cobASourceName;
        private System.Windows.Forms.Button btnAAddNew;
        private System.Windows.Forms.TextBox txtATargetName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnADelete;
        private System.Windows.Forms.ComboBox cobAProductDelete;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Timer timerMessage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cobBSourceName;
        private System.Windows.Forms.Button btnBAddNew;
        private System.Windows.Forms.TextBox txtBTargetName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cobBProductDelete;
        private System.Windows.Forms.Button btnBDelete;
        private System.Windows.Forms.Label label6;
    }
}