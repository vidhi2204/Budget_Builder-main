namespace Budget_Builder.Models;

public class DataViewModel
{
    public List<string> MonthList { get; set; }

    public List<CategoryViewModel> IncomeCategoryList { get; set; }

    public List<CategoryViewModel> ExpenseCategoryList { get; set; }

    public List<decimal> TotalIncome { get; set; }

    public List<decimal> TotalExpense { get; set; }
    public List<decimal> ProfitLoss { get; set; }

    public List<decimal> OpeningBalance { get; set; }

    public List<decimal> ClosingBalance { get; set; }


}
