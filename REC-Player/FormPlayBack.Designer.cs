namespace ry.rec
{
    partial class FormPlayBack
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
            this.leftPanel = new System.Windows.Forms.Panel();
            this.historyList = new System.Windows.Forms.ListView();
            this.dStart = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.dEnd = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.calendar = new Pabo.Calendar.MonthCalendar();
            this.panel2 = new System.Windows.Forms.Panel();
            this.player = new System.Windows.Forms.PictureBox();
            this.ctlPanel = new System.Windows.Forms.Panel();
            this.labelCurrentTime = new System.Windows.Forms.Label();
            this.labelEnd = new System.Windows.Forms.Label();
            this.labelStart = new System.Windows.Forms.Label();
            this.timeSlider = new System.Windows.Forms.TrackBar();
            this.leftPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.player)).BeginInit();
            this.ctlPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeSlider)).BeginInit();
            this.SuspendLayout();
            // 
            // leftPanel
            // 
            this.leftPanel.Controls.Add(this.historyList);
            this.leftPanel.Controls.Add(this.calendar);
            this.leftPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftPanel.Location = new System.Drawing.Point(0, 0);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Size = new System.Drawing.Size(264, 481);
            this.leftPanel.TabIndex = 0;
            // 
            // historyList
            // 
            this.historyList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.dStart,
            this.dEnd});
            this.historyList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historyList.FullRowSelect = true;
            this.historyList.Location = new System.Drawing.Point(0, 184);
            this.historyList.MultiSelect = false;
            this.historyList.Name = "historyList";
            this.historyList.Size = new System.Drawing.Size(264, 297);
            this.historyList.TabIndex = 3;
            this.historyList.UseCompatibleStateImageBehavior = false;
            this.historyList.View = System.Windows.Forms.View.Details;
            this.historyList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.historyList_MouseDoubleClick);
            // 
            // dStart
            // 
            this.dStart.Text = "开始时间";
            this.dStart.Width = 122;
            // 
            // dEnd
            // 
            this.dEnd.Text = "结束时间";
            this.dEnd.Width = 136;
            // 
            // calendar
            // 
            this.calendar.ActiveMonth.Month = 5;
            this.calendar.ActiveMonth.Year = 2017;
            this.calendar.Culture = new System.Globalization.CultureInfo("zh-CN");
            this.calendar.Dock = System.Windows.Forms.DockStyle.Top;
            this.calendar.Footer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.calendar.Header.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.calendar.Header.TextColor = System.Drawing.Color.White;
            this.calendar.ImageList = null;
            this.calendar.Location = new System.Drawing.Point(0, 0);
            this.calendar.MaxDate = new System.DateTime(2027, 5, 23, 17, 7, 23, 553);
            this.calendar.MinDate = new System.DateTime(2007, 5, 23, 17, 7, 23, 553);
            this.calendar.Month.BackgroundImage = null;
            this.calendar.Month.DateFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.calendar.Month.TextFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.calendar.Name = "calendar";
            this.calendar.Size = new System.Drawing.Size(264, 184);
            this.calendar.TabIndex = 2;
            this.calendar.Weekdays.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.calendar.Weeknumbers.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.calendar.DaySelected += new Pabo.Calendar.DaySelectedEventHandler(this.calendar_DaySelected);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.player);
            this.panel2.Controls.Add(this.ctlPanel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(264, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(652, 481);
            this.panel2.TabIndex = 1;
            // 
            // player
            // 
            this.player.Dock = System.Windows.Forms.DockStyle.Fill;
            this.player.Image = global::ry.rec.Properties.Resources.no_video;
            this.player.Location = new System.Drawing.Point(0, 0);
            this.player.Name = "player";
            this.player.Size = new System.Drawing.Size(652, 418);
            this.player.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.player.TabIndex = 1;
            this.player.TabStop = false;
            this.player.MouseClick += new System.Windows.Forms.MouseEventHandler(this.player_MouseClick);
            // 
            // ctlPanel
            // 
            this.ctlPanel.Controls.Add(this.labelCurrentTime);
            this.ctlPanel.Controls.Add(this.labelEnd);
            this.ctlPanel.Controls.Add(this.labelStart);
            this.ctlPanel.Controls.Add(this.timeSlider);
            this.ctlPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ctlPanel.Location = new System.Drawing.Point(0, 418);
            this.ctlPanel.Name = "ctlPanel";
            this.ctlPanel.Size = new System.Drawing.Size(652, 63);
            this.ctlPanel.TabIndex = 0;
            // 
            // labelCurrentTime
            // 
            this.labelCurrentTime.AutoSize = true;
            this.labelCurrentTime.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelCurrentTime.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.labelCurrentTime.Location = new System.Drawing.Point(258, 30);
            this.labelCurrentTime.Name = "labelCurrentTime";
            this.labelCurrentTime.Size = new System.Drawing.Size(114, 24);
            this.labelCurrentTime.TabIndex = 3;
            this.labelCurrentTime.Text = "00:00:00";
            // 
            // labelEnd
            // 
            this.labelEnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelEnd.AutoSize = true;
            this.labelEnd.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.labelEnd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelEnd.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelEnd.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.labelEnd.Location = new System.Drawing.Point(512, 49);
            this.labelEnd.Name = "labelEnd";
            this.labelEnd.Size = new System.Drawing.Size(140, 14);
            this.labelEnd.TabIndex = 2;
            this.labelEnd.Text = "2000-10-10 10:10:10";
            // 
            // labelStart
            // 
            this.labelStart.AutoSize = true;
            this.labelStart.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.labelStart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelStart.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelStart.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.labelStart.Location = new System.Drawing.Point(0, 49);
            this.labelStart.Name = "labelStart";
            this.labelStart.Size = new System.Drawing.Size(140, 14);
            this.labelStart.TabIndex = 1;
            this.labelStart.Text = "2000-10-10 10:10:10";
            // 
            // timeSlider
            // 
            this.timeSlider.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timeSlider.Location = new System.Drawing.Point(0, 0);
            this.timeSlider.Name = "timeSlider";
            this.timeSlider.Size = new System.Drawing.Size(652, 63);
            this.timeSlider.TabIndex = 0;
            this.timeSlider.ValueChanged += new System.EventHandler(this.timeSlider_ValueChanged);
            this.timeSlider.MouseDown += new System.Windows.Forms.MouseEventHandler(this.timeSlider_MouseDown);
            this.timeSlider.MouseUp += new System.Windows.Forms.MouseEventHandler(this.timeSlider_MouseUp);
            // 
            // FormPlayBack
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(916, 481);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.leftPanel);
            this.Name = "FormPlayBack";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "历史回放";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormPlayBack_FormClosed);
            this.Shown += new System.EventHandler(this.FormPlayBack_Shown);
            this.Resize += new System.EventHandler(this.FormPlayBack_Resize);
            this.leftPanel.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.player)).EndInit();
            this.ctlPanel.ResumeLayout(false);
            this.ctlPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeSlider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel leftPanel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox player;
        private System.Windows.Forms.Panel ctlPanel;
        private System.Windows.Forms.ListView historyList;
        private Pabo.Calendar.MonthCalendar calendar;
        private System.Windows.Forms.ColumnHeader dStart;
        private System.Windows.Forms.ColumnHeader dEnd;
        private System.Windows.Forms.TrackBar timeSlider;
        private System.Windows.Forms.Label labelEnd;
        private System.Windows.Forms.Label labelStart;
        private System.Windows.Forms.Label labelCurrentTime;
    }
}