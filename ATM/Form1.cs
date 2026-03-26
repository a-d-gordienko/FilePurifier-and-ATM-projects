using System.ComponentModel;

namespace ATM
{
    public partial class frmATM : Form
    {
        ATM_Logic atm = new ATM_Logic();
        public frmATM()
        {
            InitializeComponent();

            BindDataDataGridView(dgvCashCassetes, atm.CassetteList);
            BindDataDataGridView(dgvOutCash, atm.outCash);
            BindDataCbDenomination();

            UpdateBalanceLabel();

            DataGridViewCasseteItemsCustomStyle(dgvCashCassetes);
            DataGridViewCasseteItemsCustomStyle(dgvOutCash);
        }
        private void BindDataDataGridView(DataGridView gridView, BindingList<CassetteItem> list)
        {
            // Привязываем список к DataGridView
            gridView.DataSource = list;

            // Настраиваем внешний вид (опционально)
            gridView.Columns["Denomination"].HeaderText = "Номинал";
            gridView.Columns["Count"].HeaderText = "Количество";

            // Запрещаем редактировать ячейки прямо в таблице
            gridView.ReadOnly = true;
        }

        private void DataGridViewCasseteItemsCustomStyle(DataGridView gridView)
        {
            // Устанавливаем цвет пустой области таблицы в белый
            gridView.BackgroundColor = Color.White;

            // Устанавливаем цвет самих ячеек (строк) в белый
            gridView.DefaultCellStyle.BackColor = Color.White;

            //Отключаем системные стили, которые "красят" первый столбец
            gridView.EnableHeadersVisualStyles = false;

            //Явно задаем цвет заголовков (например, стандартный LightGray)
            gridView.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            gridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            gridView.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.LightGray;

            //Снимаем выделение при запуске (чтобы не было синей рамки на первой ячейке)
            gridView.ClearSelection();
        }
        private void BindDataCbDenomination()
        {
            // 1. Указываем источник данных (весь список объектов)
            cbDenomination.DataSource = atm.CassetteList;

            // 2. Указываем, КАКОЕ свойство объекта показывать в списке (название номинала)
            cbDenomination.DisplayMember = "Denomination";

            // 3. Указываем, КАКОЕ свойство возвращать при выборе (удобно для кода)
            cbDenomination.ValueMember = "Denomination";
        }

        private void bAdd_Click(object sender, EventArgs e)
        {
            //Проверяем, выбрано ли что-то в ComboBox
            if (cbDenomination.SelectedValue == null || nudCount.Value == 0) return;

            //Получаем номинал и количество
            int selectedNominal = (int)cbDenomination.SelectedValue!;
            int countToAdd = (int)nudCount.Value;

            if (!atm.Add(selectedNominal, countToAdd))
            {
                MessageBox.Show("Банкомат не может принять купюры по причине отсутствия места в кассете.");
            }

            UpdateBalanceLabel();

            nudCount.Value = 0;
        }

        private void tbRequstedAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Отменяет ввод символа
            }
        }

        private void bGetCash_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(tbRequstedAmount.Text, out int reqAmount) || reqAmount <= 0)
            {
                MessageBox.Show("Введите корректную положительную сумму", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var dialogResult = MessageBox.Show("Выдать с разменом?", "Подтверждение",
                                              MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            string resultWithdraw = atm.Withdraw(reqAmount, dialogResult == DialogResult.Yes);

            if (resultWithdraw != "Успешно")
            {
                MessageBox.Show(resultWithdraw, "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            UpdateBalanceLabel();
        }

        private void tbRequstedAmount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // "Нажимаем" кнопку программно
                bGetCash.PerformClick();

                // Подавляем системный звук "дзынь" при нажатии Enter
                e.SuppressKeyPress = true;
            }
        }

        private void UpdateBalanceLabel()
        {
            int total = atm.GetTotalBalance();
            lBalance.Text = $"Баланс: {total:N0} руб.";
            // :N0 добавит красивые пробелы между тысячами, например: 150 000
        }
    }
}
