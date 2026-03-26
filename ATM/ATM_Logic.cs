using System.ComponentModel;

public class CassetteItem
{
    public int Denomination { get; set; }
    public int Count { get; set; } 
}

internal class ATM_Logic
{
    const int MaxDenominationCount = 500;
    // Список для отображения
    public BindingList<CassetteItem> CassetteList { get; set; } = new BindingList<CassetteItem>
        {
            new CassetteItem { Denomination = 10, Count = 500 },
            new CassetteItem { Denomination = 50, Count = 100 },
            new CassetteItem { Denomination = 100, Count = 50 },
            new CassetteItem { Denomination = 500, Count = 10 },
            new CassetteItem { Denomination = 1000, Count = 5 },
            new CassetteItem { Denomination = 2000, Count = 3 },
            new CassetteItem { Denomination = 5000, Count = 2 }
        };

    public BindingList<CassetteItem> outCash { get; set; } = new BindingList<CassetteItem>();

    public bool Add (int Denomination, int Count)
    {
        //Ищем нужную купюру в логике и обновляем
        var item = CassetteList.FirstOrDefault(x => x.Denomination == Denomination);
        if (item != null)
        {
            if(item.Count + Count > MaxDenominationCount)
            {
                return false;
            }
            item.Count += Count;

            CassetteList.ResetBindings();
            return true;
        }
        return false;
    }

    public string Withdraw(int amount, bool preferExchange)
    {
        if (amount <= 0) return "Введите сумму больше нуля";

        int minDenomination = CassetteList.Min(x => x.Denomination);

        if (amount % minDenomination != 0)
        {
            return $"Сумма должна быть кратна {minDenomination} руб.";
        }

        outCash.Clear();

        bool success = false;
        if (preferExchange)
        {
            success = TryCalculate(amount, isExchangeMode: true);
        }

        if (!success)
        {
            outCash.Clear();
            success = TryCalculate(amount, isExchangeMode: false);
        }

        if (success)
        {
            ApplyWithdrawal();
            return "Успешно";
        }

        return "Невозможно выдать сумму";
    }

    private bool TryCalculate(int amount, bool isExchangeMode)
    {
        int remaining = amount;
        // Временный список для "примерки"
        List<CassetteItem> tempOutput = new List<CassetteItem>();

        var available = CassetteList.OrderByDescending(x => x.Denomination).ToList();

        foreach (var item in available)
        {
            int countNeeded = remaining / item.Denomination;
            int actualToGive = Math.Min(countNeeded, item.Count);

            if (isExchangeMode && actualToGive > 0 && remaining == amount)
            {
                actualToGive--;
            }

            if (actualToGive > 0)
            {
                remaining -= actualToGive * item.Denomination;

                tempOutput.Add(new CassetteItem { Denomination = item.Denomination, Count = actualToGive });
            }
        }

        if (remaining == 0)
        {
            foreach (var res in tempOutput)
            {
                outCash.Add(res);
            }
            return true;
        }

        return false;
    }

    private void ApplyWithdrawal()
    {
        foreach (var issued in outCash)
        {
            var realCassette = CassetteList.FirstOrDefault(x => x.Denomination == issued.Denomination);
            if (realCassette != null) realCassette.Count -= issued.Count;
        }
        CassetteList.ResetBindings();
        outCash.ResetBindings();
    }

    public int GetTotalBalance()
    {
        // Считаем сумму: номинал каждой кассеты умножаем на количество купюр в ней
        return CassetteList.Sum(x => x.Denomination * x.Count);
    }

}
