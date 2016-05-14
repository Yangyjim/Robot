namespace Robot_main
{
    partial class AxisStatusPic
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.chart_currentAxis = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chart_currentAxis)).BeginInit();
            this.SuspendLayout();
            // 
            // chart_currentAxis
            // 
            this.chart_currentAxis.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea1.Name = "ChartArea1";
            this.chart_currentAxis.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart_currentAxis.Legends.Add(legend1);
            this.chart_currentAxis.Location = new System.Drawing.Point(12, 36);
            this.chart_currentAxis.Name = "chart_currentAxis";
            this.chart_currentAxis.Size = new System.Drawing.Size(483, 250);
            this.chart_currentAxis.TabIndex = 5;
            // 
            // AxisStatusPic
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(505, 343);
            this.Controls.Add(this.chart_currentAxis);
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "AxisStatusPic";
            this.Text = "AxisStatusPic";
            ((System.ComponentModel.ISupportInitialize)(this.chart_currentAxis)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart_currentAxis;
    }
}