namespace ATM
{
    partial class frmATM
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
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle7 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle8 = new DataGridViewCellStyle();
            grpInputCurrency = new GroupBox();
            lBalance = new Label();
            bAdd = new Button();
            label2 = new Label();
            nudCount = new NumericUpDown();
            label1 = new Label();
            cbDenomination = new ComboBox();
            dgvCashCassetes = new DataGridView();
            grpOutputCurrency = new GroupBox();
            bGetCash = new Button();
            tbRequstedAmount = new TextBox();
            label3 = new Label();
            dgvOutCash = new DataGridView();
            bClose = new Button();
            grpInputCurrency.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvCashCassetes).BeginInit();
            grpOutputCurrency.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvOutCash).BeginInit();
            SuspendLayout();
            // 
            // grpInputCurrency
            // 
            grpInputCurrency.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpInputCurrency.Controls.Add(lBalance);
            grpInputCurrency.Controls.Add(bAdd);
            grpInputCurrency.Controls.Add(label2);
            grpInputCurrency.Controls.Add(nudCount);
            grpInputCurrency.Controls.Add(label1);
            grpInputCurrency.Controls.Add(cbDenomination);
            grpInputCurrency.Controls.Add(dgvCashCassetes);
            grpInputCurrency.Location = new Point(12, 12);
            grpInputCurrency.Name = "grpInputCurrency";
            grpInputCurrency.Size = new Size(600, 238);
            grpInputCurrency.TabIndex = 0;
            grpInputCurrency.TabStop = false;
            grpInputCurrency.Text = "Принимать";
            // 
            // lBalance
            // 
            lBalance.AutoSize = true;
            lBalance.Location = new Point(275, 206);
            lBalance.Name = "lBalance";
            lBalance.Size = new Size(0, 15);
            lBalance.TabIndex = 7;
            // 
            // bAdd
            // 
            bAdd.Location = new Point(519, 38);
            bAdd.Name = "bAdd";
            bAdd.Size = new Size(75, 22);
            bAdd.TabIndex = 5;
            bAdd.Text = "Добавить";
            bAdd.UseVisualStyleBackColor = true;
            bAdd.Click += bAdd_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(275, 111);
            label2.Name = "label2";
            label2.Size = new Size(72, 15);
            label2.TabIndex = 4;
            label2.Text = "Количество";
            // 
            // nudCount
            // 
            nudCount.Location = new Point(275, 129);
            nudCount.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            nudCount.Name = "nudCount";
            nudCount.Size = new Size(174, 23);
            nudCount.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(275, 19);
            label1.Name = "label1";
            label1.Size = new Size(59, 15);
            label1.TabIndex = 2;
            label1.Text = "Номинал";
            // 
            // cbDenomination
            // 
            cbDenomination.DropDownStyle = ComboBoxStyle.DropDownList;
            cbDenomination.FormattingEnabled = true;
            cbDenomination.Location = new Point(275, 38);
            cbDenomination.Name = "cbDenomination";
            cbDenomination.Size = new Size(174, 23);
            cbDenomination.TabIndex = 1;
            // 
            // dgvCashCassetes
            // 
            dgvCashCassetes.AllowUserToAddRows = false;
            dgvCashCassetes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = Color.Transparent;
            dataGridViewCellStyle5.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            dataGridViewCellStyle5.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = DataGridViewTriState.True;
            dgvCashCassetes.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            dgvCashCassetes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = SystemColors.Window;
            dataGridViewCellStyle6.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle6.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = SystemColors.Window;
            dataGridViewCellStyle6.SelectionForeColor = SystemColors.ControlText;
            dataGridViewCellStyle6.WrapMode = DataGridViewTriState.False;
            dgvCashCassetes.DefaultCellStyle = dataGridViewCellStyle6;
            dgvCashCassetes.Enabled = false;
            dgvCashCassetes.Location = new Point(6, 22);
            dgvCashCassetes.MultiSelect = false;
            dgvCashCassetes.Name = "dgvCashCassetes";
            dgvCashCassetes.ReadOnly = true;
            dgvCashCassetes.RowHeadersVisible = false;
            dgvCashCassetes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCashCassetes.Size = new Size(252, 199);
            dgvCashCassetes.TabIndex = 0;
            dgvCashCassetes.TabStop = false;
            // 
            // grpOutputCurrency
            // 
            grpOutputCurrency.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpOutputCurrency.Controls.Add(bGetCash);
            grpOutputCurrency.Controls.Add(tbRequstedAmount);
            grpOutputCurrency.Controls.Add(label3);
            grpOutputCurrency.Controls.Add(dgvOutCash);
            grpOutputCurrency.Location = new Point(12, 256);
            grpOutputCurrency.Name = "grpOutputCurrency";
            grpOutputCurrency.Size = new Size(600, 237);
            grpOutputCurrency.TabIndex = 1;
            grpOutputCurrency.TabStop = false;
            grpOutputCurrency.Text = "Выдавать";
            // 
            // bGetCash
            // 
            bGetCash.Location = new Point(519, 40);
            bGetCash.Name = "bGetCash";
            bGetCash.Size = new Size(75, 23);
            bGetCash.TabIndex = 4;
            bGetCash.Text = "Снять";
            bGetCash.UseVisualStyleBackColor = true;
            bGetCash.Click += bGetCash_Click;
            // 
            // tbRequstedAmount
            // 
            tbRequstedAmount.Location = new Point(275, 40);
            tbRequstedAmount.Name = "tbRequstedAmount";
            tbRequstedAmount.Size = new Size(174, 23);
            tbRequstedAmount.TabIndex = 3;
            tbRequstedAmount.KeyDown += tbRequstedAmount_KeyDown;
            tbRequstedAmount.KeyPress += tbRequstedAmount_KeyPress;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(275, 22);
            label3.Name = "label3";
            label3.Size = new Size(89, 15);
            label3.TabIndex = 2;
            label3.Text = "Сумма выдачи";
            // 
            // dgvOutCash
            // 
            dgvOutCash.AllowUserToAddRows = false;
            dgvOutCash.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle7.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = Color.Transparent;
            dataGridViewCellStyle7.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            dataGridViewCellStyle7.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = DataGridViewTriState.True;
            dgvOutCash.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            dgvOutCash.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle8.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = SystemColors.Window;
            dataGridViewCellStyle8.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle8.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = SystemColors.Window;
            dataGridViewCellStyle8.SelectionForeColor = SystemColors.ControlText;
            dataGridViewCellStyle8.WrapMode = DataGridViewTriState.False;
            dgvOutCash.DefaultCellStyle = dataGridViewCellStyle8;
            dgvOutCash.Enabled = false;
            dgvOutCash.Location = new Point(6, 22);
            dgvOutCash.MultiSelect = false;
            dgvOutCash.Name = "dgvOutCash";
            dgvOutCash.ReadOnly = true;
            dgvOutCash.RowHeadersVisible = false;
            dgvOutCash.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvOutCash.Size = new Size(252, 199);
            dgvOutCash.TabIndex = 1;
            dgvOutCash.TabStop = false;
            // 
            // bClose
            // 
            bClose.DialogResult = DialogResult.Cancel;
            bClose.Location = new Point(531, 510);
            bClose.Name = "bClose";
            bClose.Size = new Size(75, 23);
            bClose.TabIndex = 2;
            bClose.Text = "Закрыть";
            bClose.UseVisualStyleBackColor = true;
            // 
            // frmATM
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(624, 541);
            Controls.Add(bClose);
            Controls.Add(grpOutputCurrency);
            Controls.Add(grpInputCurrency);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximumSize = new Size(640, 580);
            MinimumSize = new Size(640, 580);
            Name = "frmATM";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Банкомат";
            grpInputCurrency.ResumeLayout(false);
            grpInputCurrency.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvCashCassetes).EndInit();
            grpOutputCurrency.ResumeLayout(false);
            grpOutputCurrency.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvOutCash).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox grpInputCurrency;
        private GroupBox grpOutputCurrency;
        private DataGridView dgvCashCassetes;
        private Button bAdd;
        private Label label2;
        private NumericUpDown nudCount;
        private Label label1;
        private ComboBox cbDenomination;
        private Button bClose;
        private DataGridView dgvOutCash;
        private Label label3;
        private TextBox tbRequstedAmount;
        private Button bGetCash;
        private Label lBalance;
    }
}
