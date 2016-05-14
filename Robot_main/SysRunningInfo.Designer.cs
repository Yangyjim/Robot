namespace Robot_main
{
    partial class SysRunningInfo
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
            this.listView_runninginfo = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button_save = new System.Windows.Forms.Button();
            this.button_hide = new System.Windows.Forms.Button();
            this.button_clear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listView_runninginfo
            // 
            this.listView_runninginfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView_runninginfo.FullRowSelect = true;
            this.listView_runninginfo.GridLines = true;
            this.listView_runninginfo.Location = new System.Drawing.Point(13, 13);
            this.listView_runninginfo.Margin = new System.Windows.Forms.Padding(4);
            this.listView_runninginfo.MultiSelect = false;
            this.listView_runninginfo.Name = "listView_runninginfo";
            this.listView_runninginfo.Size = new System.Drawing.Size(812, 336);
            this.listView_runninginfo.TabIndex = 10;
            this.listView_runninginfo.UseCompatibleStateImageBehavior = false;
            this.listView_runninginfo.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "时间";
            this.columnHeader1.Width = 120;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "信息";
            this.columnHeader2.Width = 1037;
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(199, 359);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(75, 23);
            this.button_save.TabIndex = 11;
            this.button_save.Text = "保存";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // button_hide
            // 
            this.button_hide.Location = new System.Drawing.Point(479, 359);
            this.button_hide.Name = "button_hide";
            this.button_hide.Size = new System.Drawing.Size(75, 23);
            this.button_hide.TabIndex = 11;
            this.button_hide.Text = "退出";
            this.button_hide.UseVisualStyleBackColor = true;
            this.button_hide.Click += new System.EventHandler(this.button2_Click);
            // 
            // button_clear
            // 
            this.button_clear.Location = new System.Drawing.Point(325, 359);
            this.button_clear.Name = "button_clear";
            this.button_clear.Size = new System.Drawing.Size(75, 23);
            this.button_clear.TabIndex = 11;
            this.button_clear.Text = "清空";
            this.button_clear.UseVisualStyleBackColor = true;
            this.button_clear.Click += new System.EventHandler(this.button_clear_Click);
            // 
            // SysRunningInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(839, 394);
            this.ControlBox = false;
            this.Controls.Add(this.button_clear);
            this.Controls.Add(this.button_hide);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.listView_runninginfo);
            this.Name = "SysRunningInfo";
            this.Text = "运行信息";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView_runninginfo;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.Button button_hide;
        private System.Windows.Forms.Button button_clear;
    }
}