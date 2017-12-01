namespace AutoCADAPI.Lab3.UI
{
    partial class CompuertasUI
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.OR = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // OR
            // 
            this.OR.Image = global::AutoCADAPI.Lab3.Properties.Resources.OR;
            this.OR.Location = new System.Drawing.Point(13, 16);
            this.OR.Name = "OR";
            this.OR.Size = new System.Drawing.Size(220, 87);
            this.OR.TabIndex = 0;
            this.OR.Text = "button1";
            this.OR.UseVisualStyleBackColor = true;
            this.OR.Click += new System.EventHandler(this.button1_Click);
            // 
            // CompuertasUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.OR);
            this.Name = "CompuertasUI";
            this.Size = new System.Drawing.Size(310, 444);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button OR;
    }
}
