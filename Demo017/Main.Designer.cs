namespace TouchSocketTestApp
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btn_Start = new Button();
            btn_End = new Button();
            SuspendLayout();
            // 
            // btn_Start
            // 
            btn_Start.Location = new Point(12, 12);
            btn_Start.Name = "btn_Start";
            btn_Start.Size = new Size(75, 31);
            btn_Start.TabIndex = 0;
            btn_Start.Text = "开启服务";
            btn_Start.UseVisualStyleBackColor = true;
            btn_Start.Click += btn_Start_Click;
            // 
            // btn_End
            // 
            btn_End.Location = new Point(93, 12);
            btn_End.Name = "btn_End";
            btn_End.Size = new Size(75, 31);
            btn_End.TabIndex = 1;
            btn_End.Text = "关闭服务";
            btn_End.UseVisualStyleBackColor = true;
            btn_End.Click += btn_End_ClickAsync;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(192, 55);
            Controls.Add(btn_End);
            Controls.Add(btn_Start);
            Name = "Main";
            Text = "Main";
            ResumeLayout(false);
        }

        #endregion

        private Button btn_Start;
        private Button btn_End;
    }
}
