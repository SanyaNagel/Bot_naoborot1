namespace Bot_naoborot
{
    partial class Authorization
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
            this.button3 = new System.Windows.Forms.Button();
            this.Password = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.email = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(51, 115);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(124, 23);
            this.button3.TabIndex = 31;
            this.button3.Text = "Вход";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Password
            // 
            this.Password.Location = new System.Drawing.Point(51, 85);
            this.Password.Name = "Password";
            this.Password.PasswordChar = '*';
            this.Password.Size = new System.Drawing.Size(124, 20);
            this.Password.TabIndex = 30;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(51, 68);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(54, 13);
            this.label8.TabIndex = 29;
            this.label8.Text = "Пароль:";
            // 
            // email
            // 
            this.email.Location = new System.Drawing.Point(51, 42);
            this.email.Name = "email";
            this.email.Size = new System.Drawing.Size(124, 20);
            this.email.TabIndex = 28;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(50, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 14);
            this.label6.TabIndex = 27;
            this.label6.Text = "E-mail";
            // 
            // Authorization
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(224, 175);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.Password);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.email);
            this.Controls.Add(this.label6);
            this.Name = "Authorization";
            this.Text = "Authorization";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox Password;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox email;
        private System.Windows.Forms.Label label6;
    }
}