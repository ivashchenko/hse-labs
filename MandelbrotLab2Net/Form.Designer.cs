﻿
namespace MandelbrotLab2Net
{
    partial class Form
    {
        /// <summary>
        /// Обязательная переменная конструктора.
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
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.checkBoxInverse = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // checkBoxInverse
            // 
            this.checkBoxInverse.AutoSize = true;
            this.checkBoxInverse.Location = new System.Drawing.Point(692, 12);
            this.checkBoxInverse.Name = "checkBoxInverse";
            this.checkBoxInverse.Size = new System.Drawing.Size(76, 17);
            this.checkBoxInverse.TabIndex = 0;
            this.checkBoxInverse.Text = "Инверсия";
            this.checkBoxInverse.UseVisualStyleBackColor = true;
            this.checkBoxInverse.CheckedChanged += new System.EventHandler(this.checkBoxInverse_CheckedChanged);
            // 
            // Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.checkBoxInverse);
            this.Name = "Form";
            this.Text = "Form";
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDoubleClick);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxInverse;
    }
}

