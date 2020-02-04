namespace MountingRobot.UI
{
    partial class FrmLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmLogin));
            this.rdbOperator = new System.Windows.Forms.RadioButton();
            this.rdbAdimn = new System.Windows.Forms.RadioButton();
            this.rdbEngineer = new System.Windows.Forms.RadioButton();
            this.txtPwd = new System.Windows.Forms.TextBox();
            this.lblErrInfo = new System.Windows.Forms.Label();
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rdbOperator
            // 
            this.rdbOperator.AutoSize = true;
            this.rdbOperator.BackColor = System.Drawing.Color.Transparent;
            this.rdbOperator.Checked = true;
            this.rdbOperator.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rdbOperator.ForeColor = System.Drawing.Color.White;
            this.rdbOperator.Location = new System.Drawing.Point(317, 122);
            this.rdbOperator.Name = "rdbOperator";
            this.rdbOperator.Size = new System.Drawing.Size(84, 23);
            this.rdbOperator.TabIndex = 1;
            this.rdbOperator.TabStop = true;
            this.rdbOperator.Text = "操作员";
            this.rdbOperator.UseVisualStyleBackColor = false;
            this.rdbOperator.CheckedChanged += new System.EventHandler(this.SetTxtPwdFocus);
            this.rdbOperator.Click += new System.EventHandler(this.SetTxtPwdFocus);
            this.rdbOperator.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPwd_KeyPress);
            // 
            // rdbAdimn
            // 
            this.rdbAdimn.AutoSize = true;
            this.rdbAdimn.BackColor = System.Drawing.Color.Transparent;
            this.rdbAdimn.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rdbAdimn.ForeColor = System.Drawing.Color.White;
            this.rdbAdimn.Location = new System.Drawing.Point(317, 180);
            this.rdbAdimn.Name = "rdbAdimn";
            this.rdbAdimn.Size = new System.Drawing.Size(84, 23);
            this.rdbAdimn.TabIndex = 3;
            this.rdbAdimn.Text = "管理员";
            this.rdbAdimn.UseVisualStyleBackColor = false;
            this.rdbAdimn.CheckedChanged += new System.EventHandler(this.SetTxtPwdFocus);
            this.rdbAdimn.Click += new System.EventHandler(this.SetTxtPwdFocus);
            this.rdbAdimn.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPwd_KeyPress);
            // 
            // rdbEngineer
            // 
            this.rdbEngineer.AutoSize = true;
            this.rdbEngineer.BackColor = System.Drawing.Color.Transparent;
            this.rdbEngineer.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rdbEngineer.ForeColor = System.Drawing.Color.White;
            this.rdbEngineer.Location = new System.Drawing.Point(317, 151);
            this.rdbEngineer.Name = "rdbEngineer";
            this.rdbEngineer.Size = new System.Drawing.Size(84, 23);
            this.rdbEngineer.TabIndex = 2;
            this.rdbEngineer.Text = "工程师";
            this.rdbEngineer.UseVisualStyleBackColor = false;
            this.rdbEngineer.CheckedChanged += new System.EventHandler(this.SetTxtPwdFocus);
            this.rdbEngineer.Click += new System.EventHandler(this.SetTxtPwdFocus);
            this.rdbEngineer.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPwd_KeyPress);
            // 
            // txtPwd
            // 
            this.txtPwd.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtPwd.Location = new System.Drawing.Point(281, 224);
            this.txtPwd.MaxLength = 10;
            this.txtPwd.Name = "txtPwd";
            this.txtPwd.PasswordChar = '*';
            this.txtPwd.Size = new System.Drawing.Size(192, 26);
            this.txtPwd.TabIndex = 0;
            this.txtPwd.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPwd_KeyPress);
            // 
            // lblErrInfo
            // 
            this.lblErrInfo.AutoSize = true;
            this.lblErrInfo.BackColor = System.Drawing.Color.Transparent;
            this.lblErrInfo.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblErrInfo.ForeColor = System.Drawing.Color.Red;
            this.lblErrInfo.Location = new System.Drawing.Point(278, 267);
            this.lblErrInfo.Name = "lblErrInfo";
            this.lblErrInfo.Size = new System.Drawing.Size(49, 14);
            this.lblErrInfo.TabIndex = 3;
            this.lblErrInfo.Text = "label2";
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.Color.Transparent;
            this.btnLogin.FlatAppearance.BorderSize = 2;
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.Location = new System.Drawing.Point(281, 294);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(61, 43);
            this.btnLogin.TabIndex = 4;
            this.btnLogin.Text = "登录";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.FlatAppearance.BorderSize = 2;
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnExit.ForeColor = System.Drawing.Color.White;
            this.btnExit.Location = new System.Drawing.Point(399, 294);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(61, 43);
            this.btnExit.TabIndex = 5;
            this.btnExit.Text = "退出";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // FrmLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackgroundImage = global::MountingRobot.Properties.Resources.login2;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(660, 420);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.lblErrInfo);
            this.Controls.Add(this.txtPwd);
            this.Controls.Add(this.rdbEngineer);
            this.Controls.Add(this.rdbAdimn);
            this.Controls.Add(this.rdbOperator);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.Load += new System.EventHandler(this.FrmLogin_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rdbOperator;
        private System.Windows.Forms.RadioButton rdbAdimn;
        private System.Windows.Forms.RadioButton rdbEngineer;
        private System.Windows.Forms.TextBox txtPwd;
        private System.Windows.Forms.Label lblErrInfo;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnExit;
    }
}