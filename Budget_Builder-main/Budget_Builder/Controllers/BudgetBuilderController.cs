using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Budget_Builder.Models;

using System.Data;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Threading.Tasks;

namespace Budget_Builder.Controllers;

public class BudgetBuilderController : Controller
{
    private readonly ILogger<BudgetBuilderController> _logger;

    public BudgetBuilderController(ILogger<BudgetBuilderController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult BudgetBuilder()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    #region Budget Sheet Generation
    public IActionResult GenerateBudgetSheet(string monthList)
    {
        DataViewModel dataViewModel = new DataViewModel();
        dataViewModel.MonthList = JsonConvert.DeserializeObject<List<string>>(monthList);
        dataViewModel.IncomeCategoryList = new List<CategoryViewModel>();
        dataViewModel.ExpenseCategoryList = new List<CategoryViewModel>();
        dataViewModel.TotalIncome = new List<decimal>();
        dataViewModel.TotalExpense = new List<decimal>();
        dataViewModel.ProfitLoss = new List<decimal>();
        dataViewModel.OpeningBalance = new List<decimal>();
        dataViewModel.ClosingBalance = new List<decimal>();

        dataViewModel.IncomeCategoryList.Add(new CategoryViewModel
        {
            CatgeoryName = "",
            Amount = new List<decimal>()
        });
        dataViewModel.ExpenseCategoryList.Add(new CategoryViewModel
        {
            CatgeoryName = "",
            Amount = new List<decimal>()
        });

        for (int i = 0; i < dataViewModel.MonthList.Count; i++)
        {
            dataViewModel.IncomeCategoryList[0].Amount.Add(0);
            dataViewModel.ExpenseCategoryList[0].Amount.Add(0);
            dataViewModel.TotalIncome.Add(0);
            dataViewModel.TotalExpense.Add(0);
            dataViewModel.ProfitLoss.Add(0);
            dataViewModel.OpeningBalance.Add(0);
            dataViewModel.ClosingBalance.Add(0);
        }



        return PartialView("_DataSheetPartialJs", dataViewModel);
    }
    #endregion

    // public IActionResult UpdateBudgetSheet(string data)
    // {
    //     DataViewModel datavm = JsonConvert.DeserializeObject<DataViewModel>(data);
    //     for (int i = 0; i < datavm.MonthList.Count; i++)
    //     {
    //         datavm.TotalIncome[i] = 0;
    //         datavm.TotalExpense[i] = 0;
    //         datavm.ProfitLoss[i] = 0;
    //         datavm.OpeningBalance[i] = 0;
    //         datavm.ClosingBalance[i] = 0;
    //     }
    //     for (int i = 0; i < datavm.IncomeCategoryList.Count; i++)
    //     {
    //         for (int j = 0; j < datavm.MonthList.Count; j++)
    //         {

    //             datavm.TotalIncome[j]  += datavm.IncomeCategoryList[i].Amount[j];
    //             datavm.TotalExpense[j] += datavm.ExpenseCategoryList[i].Amount[j];
    //             datavm.ProfitLoss[j] = datavm.TotalIncome[j] - datavm.TotalExpense[j];

    //         }
    //     }
    //     for (int i = 0; i < datavm.MonthList.Count; i++)
    //     {
    //         if (i == 0)
    //         {
    //             datavm.OpeningBalance[i] = 0;
    //         }
    //         else
    //         {
    //             datavm.OpeningBalance[i] = datavm.ClosingBalance[i - 1];
    //         }
    //         datavm.ClosingBalance[i] = datavm.OpeningBalance[i] + datavm.ProfitLoss[i];
    //     }

    //     // DataViewModel dataViewModel = new DataViewModel();
    //     // dataViewModel.MonthList = JsonConvert.DeserializeObject<List<string>>(monthList);
    //     // dataViewModel.IncomeCategoryList = JsonConvert.DeserializeObject<List<CategoryViewModel>>(incomeCategoryList);
    //     // dataViewModel.ExpenseCategoryList = JsonConvert.DeserializeObject<List<CategoryViewModel>>(expenseCategoryList);

    //     return PartialView("_DataSheetPartial", datavm);
    // }

    #region Update Budget Sheet partial view
    public IActionResult UpdatePartialView(string DatasheetVM)
    {
        DataViewModel? dataViewModel = JsonConvert.DeserializeObject<DataViewModel>(DatasheetVM);
        return PartialView("_DataSheetPartial", dataViewModel);
    }
    #endregion

    #region delete category
    public IActionResult DeleteCategory(string index, bool isIncome, string DatasheetVM)
    {
        DataViewModel? dataViewModel = JsonConvert.DeserializeObject<DataViewModel>(DatasheetVM);
        int idx = Convert.ToInt32(index);
        if (isIncome)
        {
            dataViewModel.IncomeCategoryList.RemoveAt(idx);
            if (dataViewModel.IncomeCategoryList.Count == 0)
            {
                dataViewModel.IncomeCategoryList.Add(new CategoryViewModel
                {
                    CatgeoryName = "",
                    Amount = new List<decimal>()
                });
                for (int i = 0; i < dataViewModel.MonthList.Count; i++)
                {
                    dataViewModel.IncomeCategoryList[0].Amount.Add(0);
                }
            }
        }
        else
        {
            dataViewModel.ExpenseCategoryList.RemoveAt(idx);
            if (dataViewModel.ExpenseCategoryList.Count == 0)
            {
                dataViewModel.ExpenseCategoryList.Add(new CategoryViewModel
                {
                    CatgeoryName = "",
                    Amount = new List<decimal>()
                });
                for (int i = 0; i < dataViewModel.MonthList.Count; i++)
                {
                    dataViewModel.ExpenseCategoryList[0].Amount.Add(0);
                }
            }
        }
        return PartialView("_DataSheetPartial", dataViewModel);
        // return Json(dataViewModel);
    }
    #endregion

    #region add category
    public IActionResult AddCategory(bool isIncome, string DatasheetVM)
    {
        DataViewModel? dataViewModel = JsonConvert.DeserializeObject<DataViewModel>(DatasheetVM);
        if (isIncome)
        {
            dataViewModel.IncomeCategoryList.Add(new CategoryViewModel
            {
                CatgeoryName = "",
                Amount = new List<decimal>()
            });
            for (int i = 0; i < dataViewModel.MonthList.Count; i++)
            {
                dataViewModel.IncomeCategoryList[dataViewModel.IncomeCategoryList.Count - 1].Amount.Add(0);
            }
        }
        else
        {
            dataViewModel.ExpenseCategoryList.Add(new CategoryViewModel
            {
                CatgeoryName = "",
                Amount = new List<decimal>()
            });
            for (int i = 0; i < dataViewModel.MonthList.Count; i++)
            {
                dataViewModel.ExpenseCategoryList[dataViewModel.ExpenseCategoryList.Count - 1].Amount.Add(0);
            }
        }
        return PartialView("_DataSheetPartial", dataViewModel);
    }
    #endregion

    #region export sheet data to excel
    public async Task<IActionResult> ExportDataToExcel(string DatasheetVM)
    {
        DataViewModel? dataViewModel = JsonConvert.DeserializeObject<DataViewModel>(DatasheetVM);
        var fileData = await FillBudgetSheetExcel(dataViewModel);
        var result = new FileContentResult(fileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = "BudgetSheet.xlsx"
        };
        return result;
    }
    #endregion

    #region excel design for budget sheet
    public Task<byte[]> FillBudgetSheetExcel(DataViewModel dataViewModel)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("budgetSheet");
            var currentRow = 2;
            var currentCol = 2;


            worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1].Merge = true;
            worksheet.Cells[currentRow, currentCol].Value = "Budget Builder ";
            using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#FFFFFF"));
                headingCells.Style.Font.Bold = true;
                headingCells.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }


            int headingRow = currentRow + 4;
            int headingCol = 2;

            worksheet.Cells[headingRow, headingCol].Value = "Category";
            headingCol++;

            for (int i = 0; i < dataViewModel.MonthList.Count; i++)
            {
                worksheet.Cells[headingRow, headingCol].Value = dataViewModel.MonthList[i];
                headingCol++;
            }



            int row = headingRow + 1;
            int col = 2;

            // income 
            worksheet.Cells[row, col].Value = "Income";
            row += 1; // Move to the next column for month data

            //  income data
            for (int i = 0; i < dataViewModel.IncomeCategoryList.Count; i++)
            {
                worksheet.Cells[row, col].Value = dataViewModel.IncomeCategoryList[i].CatgeoryName == "" ? "-" : dataViewModel.IncomeCategoryList[i].CatgeoryName;
                col += 1;

                for (int j = 0; j < dataViewModel.MonthList.Count; j++)
                {
                    worksheet.Cells[row, col].Value = dataViewModel.IncomeCategoryList[i].Amount[j];
                    col += 1;
                }

                row += 1;
                col = 2;
            }

            //  total income
            worksheet.Cells[row, 2].Value = "Total Income";
            col = 3;
            for (int j = 0; j < dataViewModel.MonthList.Count; j++)
            {
                worksheet.Cells[row, col].Value = dataViewModel.TotalIncome[j];
                col += 1;
            }

            row += 1;
            col = 2;

            // expense
            worksheet.Cells[row, col].Value = "Expense";
            row += 1;
            col = 2;
            //expense data
            for (int i = 0; i < dataViewModel.ExpenseCategoryList.Count; i++)
            {
                worksheet.Cells[row, col].Value = dataViewModel.ExpenseCategoryList[i].CatgeoryName == "" ? "-" : dataViewModel.ExpenseCategoryList[i].CatgeoryName;
                col += 1;

                for (int j = 0; j < dataViewModel.MonthList.Count; j++)
                {
                    worksheet.Cells[row, col].Value = dataViewModel.ExpenseCategoryList[i].Amount[j];
                    col += 1;
                }

                row += 1;
                col = 2;
            }

            //total expense
            worksheet.Cells[row, 2].Value = "Total Expense";
            col = 3;
            for (int j = 0; j < dataViewModel.MonthList.Count; j++)
            {
                worksheet.Cells[row, col].Value = dataViewModel.TotalExpense[j];
                col += 1;
            }

            row += 1;
            col = 2;

            //summary
            worksheet.Cells[row, col].Value = "Summary";
            col += 1;
            for (int j = 0; j < dataViewModel.MonthList.Count; j++)
            {
                worksheet.Cells[row, col].Value = dataViewModel.ProfitLoss[j];
                col += 1;
            }
            row += 1;
            col = 2;

            //profit/loss
            worksheet.Cells[row, col].Value = "Profit/Loss";
            col += 1;
            for (int j = 0; j < dataViewModel.MonthList.Count; j++)
            {
                worksheet.Cells[row, col].Value = dataViewModel.ProfitLoss[j];
                col += 1;
            }
            row += 1;
            col = 2;

            //opening balance
            worksheet.Cells[row, col].Value = "Opening Balance";
            col += 1;
            for (int j = 0; j < dataViewModel.MonthList.Count; j++)
            {
                worksheet.Cells[row, col].Value = dataViewModel.OpeningBalance[j];
                col += 1;
            }
            row += 1;
            col = 2;

            //closing balance
            worksheet.Cells[row, col].Value = "Closing Balance";
            col += 1;
            for (int j = 0; j < dataViewModel.MonthList.Count; j++)
            {
                worksheet.Cells[row, col].Value = dataViewModel.ClosingBalance[j];
                col += 1;
            }




            //  It creates a Task that is already completed and contains the specified result 
            // (in this case, the byte array).
            // This is useful when you need to return a Task in an asynchronous method but already have 
            // the result available synchronously.
            return Task.FromResult(package.GetAsByteArray());
        }
    }
    #endregion

    #region export activity to excel
    public async Task<IActionResult> ExportActivityToExcel(string timeArray, string colorArray, string messageArray)
    {
        List<string> timeList = JsonConvert.DeserializeObject<List<string>>(timeArray);
        List<string> colorList = JsonConvert.DeserializeObject<List<string>>(colorArray);
        List<string> messageList = JsonConvert.DeserializeObject<List<string>>(messageArray);
        var fileData = await FillActivityExcel(timeList, colorList, messageList);
        var result = new FileContentResult(fileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = "Activity sheet.xlsx"
        };
        return result;
    }
    #endregion

    #region excel design for activity sheet
    public Task<byte[]> FillActivityExcel(List<string> timeList, List<string> colorList, List<string> messageList)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Activity");
            var currentRow = 2;
            var currentCol = 2;


            worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1].Merge = true;
            worksheet.Cells[currentRow, currentCol].Value = "Activities";
            using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#FFFFFF"));
                headingCells.Style.Font.Bold = true;
                headingCells.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            //return if list is empty
            if (timeList.Count == 0 || colorList.Count == 0 || messageList.Count == 0)
            {
                worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1].Merge = true;
                worksheet.Cells[currentRow + 2, currentCol].Value = "No activities found";
                return Task.FromResult(package.GetAsByteArray());
            }


            int headingRow = currentRow + 4;
            int headingCol = 2;

            worksheet.Cells[headingRow, headingCol, headingRow , headingCol + 1].Merge = true;
            worksheet.Cells[headingRow, headingCol].Value = "Time";
            headingCol+=2;


            worksheet.Cells[headingRow, headingCol, headingRow , headingCol + 5 ].Merge = true;
            worksheet.Cells[headingRow, headingCol].Value = "Activity";
            headingCol+=5;



            int row = headingRow + 1;
            int col = 2;

            // Fill in the data
            for (int i = 0; i < timeList.Count; i++)
            {
                worksheet.Cells[row, col, row , col+ 1].Merge = true;
                worksheet.Cells[row, col].Value = timeList[i];
                col+=2;
                worksheet.Cells[row, col, row, col + 5].Merge = true;
                worksheet.Cells[row, col].Value = messageList[i];
                row ++;
                col = 2;
            }

            
            //  It creates a Task that is already completed and contains the specified result 
            // (in this case, the byte array).
            // This is useful when you need to return a Task in an asynchronous method but already have 
            // the result available synchronously.
            return Task.FromResult(package.GetAsByteArray());
        }
    }

    #endregion



}
