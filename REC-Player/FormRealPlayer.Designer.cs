namespace ry.rec
{
    partial class FormRealPlayer
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
            this.videoBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.videoBox)).BeginInit();
            this.SuspendLayout();
            // 
            // videoBox
            // 
            this.videoBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.videoBox.Image = global::ry.rec.Properties.Resources.no_video;
            this.videoBox.InitialImage = null;
            this.videoBox.Location = new System.Drawing.Point(1, 1);
            this.videoBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.videoBox.Name = "videoBox";
            this.videoBox.Size = new System.Drawing.Size(396, 254);
            this.videoBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.videoBox.TabIndex = 0;
            this.videoBox.TabStop = false;
            this.videoBox.DoubleClick += new System.EventHandler(this.videoBox_DoubleClick);
            this.videoBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.videoBox_MouseClick);
            this.videoBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.videoBox_MouseDown);
            this.videoBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.videoBox_MouseMove);
            this.videoBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.videoBox_MouseUp);
            this.videoBox.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.videoBox_MouseWheel);
            // 
            // FormRealPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Highlight;
            this.ClientSize = new System.Drawing.Size(399, 257);
            this.Controls.Add(this.videoBox);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "FormRealPlayer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "实时预览";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormRealPlayer_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.videoBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox videoBox;
    }
}