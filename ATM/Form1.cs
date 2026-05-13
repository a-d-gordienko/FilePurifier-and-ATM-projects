using System.ComponentModel;
using ATM.Domain;
using ATM.Infrastructure;
using ATM.Services;

namespace ATM;

public partial class frmATM : Form
{
    private readonly IAtmService _atmService;
    private readonly ICassetteRepository _cassetteRepository;
    private readonly BindingList<CassetteItem> _cassetteList;
    private readonly BindingList<CassetteItem> _outCash;

    public frmATM() : this(
        new AtmService(new InMemoryCassetteRepository()),
        new InMemoryCassetteRepository())
    {
    }

    public frmATM(IAtmService atmService, ICassetteRepository cassetteRepository)
    {
        _atmService = atmService;
        _cassetteRepository = cassetteRepository;

        _cassetteList = new BindingList<CassetteItem>();
        _outCash = new BindingList<CassetteItem>();

        InitializeComponent();

        BindDataDataGridView(dgvCashCassetes, _cassetteList);
        BindDataDataGridView(dgvOutCash, _outCash);
        BindDataCbDenomination();

        UpdateBalanceLabel();
        RefreshCassetteList();

        DataGridViewCasseteItemsCustomStyle(dgvCashCassetes);
        DataGridViewCasseteItemsCustomStyle(dgvOutCash);
    }

    private void RefreshCassetteList()
    {
        _cassetteList.Clear();
        foreach (var cassette in _atmService.GetCassettes())
        {
            _cassetteList.Add(new CassetteItem
            {
                Denomination = cassette.Denomination,
                Count = cassette.Count
            });
        }
        _cassetteList.ResetBindings();
    }

    private void BindDataDataGridView(DataGridView gridView, BindingList<CassetteItem> list)
    {
        gridView.DataSource = list;
        gridView.Columns["Denomination"].HeaderText = "Номинал";
        gridView.Columns["Count"].HeaderText = "Количество";
        gridView.ReadOnly = true;
    }

    private void DataGridViewCasseteItemsCustomStyle(DataGridView gridView)
    {
        gridView.BackgroundColor = Color.White;
        gridView.DefaultCellStyle.BackColor = Color.White;
        gridView.EnableHeadersVisualStyles = false;
        gridView.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
        gridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
        gridView.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.LightGray;
        gridView.ClearSelection();
    }

    private void BindDataCbDenomination()
    {
        cbDenomination.DataSource = _cassetteList;
        cbDenomination.DisplayMember = "Denomination";
        cbDenomination.ValueMember = "Denomination";
    }

    private void bAdd_Click(object sender, EventArgs e)
    {
        if (cbDenomination.SelectedValue == null || nudCount.Value == 0) return;

        int selectedNominal = (int)cbDenomination.SelectedValue;
        int countToAdd = (int)nudCount.Value;

        var (success, message) = _atmService.Refill(selectedNominal, countToAdd);
        if (!success)
        {
            MessageBox.Show(message, "Ошибка пополнения", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        RefreshCassetteList();
        UpdateBalanceLabel();
        nudCount.Value = 0;
    }

    private void tbRequstedAmount_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
        {
            e.Handled = true;
        }
    }

    private void bGetCash_Click(object sender, EventArgs e)
    {
        if (!int.TryParse(tbRequstedAmount.Text, out int reqAmount) || reqAmount <= 0)
        {
            MessageBox.Show("Введите корректное положительное число", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var dialogResult = MessageBox.Show("Выдать с разменом?", "Подтверждение",
                                          MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        var request = new WithdrawalRequest(reqAmount, dialogResult == DialogResult.Yes);
        var result = _atmService.Withdraw(request);

        if (!result.Success)
        {
            MessageBox.Show(result.Message, "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        RefreshCassetteList();
        UpdateBalanceLabel();
        UpdateOutCashList(result.Dispensed ?? new List<Cassette>());
    }

    private void UpdateOutCashList(IReadOnlyList<Cassette> dispensed)
    {
        _outCash.Clear();
        foreach (var cassette in dispensed)
        {
            _outCash.Add(new CassetteItem
            {
                Denomination = cassette.Denomination,
                Count = cassette.Count
            });
        }
        _outCash.ResetBindings();
    }

    private void tbRequstedAmount_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            bGetCash.PerformClick();
            e.SuppressKeyPress = true;
        }
    }

    private void UpdateBalanceLabel()
    {
        int total = _atmService.GetTotalBalance();
        lBalance.Text = $"Баланс: {total:N0} руб.";
    }
}