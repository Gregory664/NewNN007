namespace NewNN007
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.LoadTs = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LoadTs
            // 
            this.LoadTs.Location = new System.Drawing.Point(12, 12);
            this.LoadTs.Name = "LoadTs";
            this.LoadTs.Size = new System.Drawing.Size(256, 31);
            this.LoadTs.TabIndex = 0;
            this.LoadTs.Text = "Загрузить обучающую выборку";
            this.LoadTs.UseVisualStyleBackColor = true;
            this.LoadTs.Click += new System.EventHandler(this.LoadTs_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(568, 437);
            this.Controls.Add(this.LoadTs);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button LoadTs;
    }
}

