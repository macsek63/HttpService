namespace HttpService
{
    partial class ConnectForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

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
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.tb_Login = new System.Windows.Forms.TextBox();
            this.tb_Haslo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button_ok = new System.Windows.Forms.Button();
            this.button_wy = new System.Windows.Forms.Button();
            this.tb_Serwer = new System.Windows.Forms.TextBox();
            this.tb_Baza = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tb_Login
            // 
            this.tb_Login.Font = new System.Drawing.Font("Arial", 9F);
            this.tb_Login.Location = new System.Drawing.Point(76, 33);
            this.tb_Login.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tb_Login.Name = "tb_Login";
            this.tb_Login.Size = new System.Drawing.Size(136, 21);
            this.tb_Login.TabIndex = 0;
            this.tb_Login.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_user_KeyPress);
            // 
            // tb_Haslo
            // 
            this.tb_Haslo.Font = new System.Drawing.Font("Arial", 9F);
            this.tb_Haslo.Location = new System.Drawing.Point(76, 60);
            this.tb_Haslo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tb_Haslo.Name = "tb_Haslo";
            this.tb_Haslo.PasswordChar = '*';
            this.tb_Haslo.Size = new System.Drawing.Size(136, 21);
            this.tb_Haslo.TabIndex = 1;
            this.tb_Haslo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_user_KeyPress);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Arial", 9F);
            this.label1.Location = new System.Drawing.Point(18, 34);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 14);
            this.label1.TabIndex = 13;
            this.label1.Text = "Login:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Arial", 9F);
            this.label2.Location = new System.Drawing.Point(16, 61);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 18);
            this.label2.TabIndex = 12;
            this.label2.Text = "Hasło:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Arial", 9F);
            this.label3.Location = new System.Drawing.Point(14, 88);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 18);
            this.label3.TabIndex = 11;
            this.label3.Text = "Serwer:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Arial", 9F);
            this.label4.Location = new System.Drawing.Point(28, 113);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 18);
            this.label4.TabIndex = 10;
            this.label4.Text = "Baza:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // button_ok
            // 
            this.button_ok.BackColor = System.Drawing.Color.YellowGreen;
            this.button_ok.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.button_ok.Location = new System.Drawing.Point(132, 170);
            this.button_ok.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(78, 68);
            this.button_ok.TabIndex = 6;
            this.button_ok.Text = "OK";
            this.button_ok.UseVisualStyleBackColor = false;
            this.button_ok.Click += new System.EventHandler(this.button_ok_Click);
            this.button_ok.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_user_KeyPress);
            // 
            // button_wy
            // 
            this.button_wy.BackColor = System.Drawing.Color.Orange;
            this.button_wy.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.button_wy.Location = new System.Drawing.Point(38, 170);
            this.button_wy.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button_wy.Name = "button_wy";
            this.button_wy.Size = new System.Drawing.Size(78, 68);
            this.button_wy.TabIndex = 7;
            this.button_wy.Text = "Przerwij";
            this.button_wy.UseVisualStyleBackColor = false;
            this.button_wy.Click += new System.EventHandler(this.button_wy_Click);
            this.button_wy.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_user_KeyPress);
            // 
            // tb_Serwer
            // 
            this.tb_Serwer.Font = new System.Drawing.Font("Arial", 9F);
            this.tb_Serwer.Location = new System.Drawing.Point(76, 86);
            this.tb_Serwer.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tb_Serwer.Name = "tb_Serwer";
            this.tb_Serwer.Size = new System.Drawing.Size(136, 21);
            this.tb_Serwer.TabIndex = 8;
            this.tb_Serwer.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_user_KeyPress);
            // 
            // tb_Baza
            // 
            this.tb_Baza.Font = new System.Drawing.Font("Arial", 9F);
            this.tb_Baza.Location = new System.Drawing.Point(76, 112);
            this.tb_Baza.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tb_Baza.Name = "tb_Baza";
            this.tb_Baza.Size = new System.Drawing.Size(136, 21);
            this.tb_Baza.TabIndex = 9;
            this.tb_Baza.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_user_KeyPress);
            // 
            // ConnectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(241, 266);
            this.Controls.Add(this.tb_Baza);
            this.Controls.Add(this.tb_Serwer);
            this.Controls.Add(this.button_wy);
            this.Controls.Add(this.button_ok);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tb_Haslo);
            this.Controls.Add(this.tb_Login);
            this.Location = new System.Drawing.Point(0, 52);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Menu = this.mainMenu1;
            this.Name = "ConnectForm";
            this.Text = "Połączenie  SQL";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tb_Login;
        private System.Windows.Forms.TextBox tb_Haslo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button_wy;
        private System.Windows.Forms.TextBox tb_Serwer;
        private System.Windows.Forms.TextBox tb_Baza;
        public System.Windows.Forms.Button button_ok;
    }
}