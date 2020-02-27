using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.IO;
using AutomaticInvestmentPlan_Comm;
using AutomaticInvestmentPlan_Model;

namespace AutomaticInvestmentPlan_DBService
{
    public class DbService
    {
        private readonly string _connStr = ConfigurationManager.ConnectionStrings["dbpath"].ConnectionString;

        public void DbFileExistenceCheck()
        {
            if (File.Exists("DB\\investment.db") == false)
            {
                throw new Exception("DB File not exist!");
            }
            SQLiteConnection cn = new SQLiteConnection(_connStr);
            cn.Open();
            cn.Close();
        }

        public HistoryModel SelectBuyResultByDate(string fundId, string buyDate)
        {
            List<HistoryModel> historyModels = new List<HistoryModel>();
            try
            {
                using (SQLiteConnection cn = new SQLiteConnection(_connStr))
                {
                    cn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand
                    {
                        Connection = cn,
                        CommandText = $"select * from t_history where 基金代码='{fundId}' and 买入日期='{buyDate}';"
                    })
                    {
                        using (SQLiteDataReader sr = cmd.ExecuteReader())
                        {
                            while (sr.Read())
                            {
                                HistoryModel m = new HistoryModel();
                                string date = sr["买入日期"].ToString();
                                m.BuyDate = Convert.ToDateTime(date);
                                m.Id = Convert.ToInt32(sr["序号"]);
                                m.ShangHaiIndex = Convert.ToDouble(sr["当日上证指数"]);
                                m.ShangHaiIndexJumpPercentage = Convert.ToString(sr["当日上证涨跌"]);
                                m.FundValue = Convert.ToDouble(sr["当日本基金净值"]);
                                m.FundValueJumpPercentage = Convert.ToDouble(sr["当日本基金涨跌"]);
                                m.BuyAmount = Convert.ToDouble(sr["当日买入金额"]);
                                m.AlreaySold = Convert.ToInt32(sr["是否已卖出"]);
                                if (sr["卖出日期"].ToString() != "")
                                {
                                    m.SellDate = Convert.ToDateTime(sr["卖出日期"]);
                                }
                                m.FundValueInSell = Convert.ToDouble(sr["卖出净值"]);
                                m.SellAmount = Convert.ToDouble(sr["卖出金额"]);
                                m.Profit = Convert.ToDouble(sr["盈利金额"]);
                                m.ProfitPercentage = Convert.ToDouble(sr["盈利百分比"]);
                                historyModels.Add(m);
                            }
                            sr.Close();
                            cn.Close();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                CombineLog.LogError("SelectBuyResultByDate", e);
            }
            return historyModels.Count > 0 ? historyModels[0] : null;
        }

        public HistoryModel SelectSellResultByDate(string fundId, string sellDate)
        {
            List<HistoryModel> historyModels = new List<HistoryModel>();
            try
            {
                using (SQLiteConnection cn = new SQLiteConnection(_connStr))
                {
                    cn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand
                    {
                        Connection = cn,
                        CommandText = $"select * from t_history where 基金代码='{fundId}' and 卖出日期='{sellDate}';"
                    })
                    {
                        using (SQLiteDataReader sr = cmd.ExecuteReader())
                        {
                            while (sr.Read())
                            {
                                HistoryModel m = new HistoryModel();
                                string date = sr["买入日期"].ToString();
                                m.BuyDate = Convert.ToDateTime(date);
                                m.Id = Convert.ToInt32(sr["序号"]);
                                m.ShangHaiIndex = Convert.ToDouble(sr["当日上证指数"]);
                                m.ShangHaiIndexJumpPercentage = Convert.ToString(sr["当日上证涨跌"]);
                                m.FundValue = Convert.ToDouble(sr["当日本基金净值"]);
                                m.FundValueJumpPercentage = Convert.ToDouble(sr["当日本基金涨跌"]);
                                m.BuyAmount = Convert.ToDouble(sr["当日买入金额"]);
                                m.AlreaySold = Convert.ToInt32(sr["是否已卖出"]);
                                if (sr["卖出日期"].ToString() != "")
                                {
                                    m.SellDate = Convert.ToDateTime(sr["卖出日期"]);
                                }
                                m.FundValueInSell = Convert.ToDouble(sr["卖出净值"]);
                                m.SellAmount = Convert.ToDouble(sr["卖出金额"]);
                                m.Profit = Convert.ToDouble(sr["盈利金额"]);
                                m.ProfitPercentage = Convert.ToDouble(sr["盈利百分比"]);
                                historyModels.Add(m);
                            }
                            sr.Close();
                            cn.Close();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                CombineLog.LogError("SelectSellResultByDate", e);
            }
            return historyModels.Count > 0 ? historyModels[0] : null;
        }

        public List<HistoryModel> SelectAllNotSold(string fundId)
        {
            List<HistoryModel> historyModels = new List<HistoryModel>();
            try
            {
                string dateStr = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
                using (SQLiteConnection cn = new SQLiteConnection(_connStr))
                {
                    cn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand
                    {
                        Connection = cn,
                        CommandText = $"select * from t_history t where t.基金代码='{fundId}' " +
                                      $"and t.是否已卖出=0 and t.买入日期 < '{dateStr}' ;"
                    })
                    {
                        using (SQLiteDataReader sr = cmd.ExecuteReader())
                        {
                            while (sr.Read())
                            {
                                HistoryModel m = new HistoryModel();
                                string date = sr["买入日期"].ToString();
                                m.BuyDate = Convert.ToDateTime(date);
                                m.Id = Convert.ToInt32(sr["序号"]);
                                m.ShangHaiIndex = Convert.ToDouble(sr["当日上证指数"]);
                                m.ShangHaiIndexJumpPercentage = Convert.ToString(sr["当日上证涨跌"]);
                                m.FundValue = Convert.ToDouble(sr["当日本基金净值"]);
                                m.FundValueJumpPercentage = Convert.ToDouble(sr["当日本基金涨跌"]);
                                m.BuyAmount = Convert.ToDouble(sr["当日买入金额"]);
                                m.AlreaySold = Convert.ToInt32(sr["是否已卖出"]);
                                if (sr["卖出日期"].ToString() != null && sr["卖出日期"].ToString() !="")
                                {
                                    m.SellDate = Convert.ToDateTime(sr["卖出日期"]);
                                }
                                m.FundValueInSell = Convert.ToDouble(sr["卖出净值"]);
                                m.SellAmount = Convert.ToDouble(sr["卖出金额"]);
                                m.Profit = Convert.ToDouble(sr["盈利金额"]);
                                m.ProfitPercentage = Convert.ToDouble(sr["盈利百分比"]);
                                historyModels.Add(m);
                            }
                            sr.Close();
                            cn.Close();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                CombineLog.LogError("Select", e);
            }
            return historyModels;
        }

        public bool InsertBuyResult(HistoryModel model)
        {
            bool result;
            try
            {
                SQLiteConnection cn = new SQLiteConnection(_connStr);
                cn.Open();
                string sellDate = model.SellDate == null ? "" : model.SellDate.Value.ToString("yyyy-MM-dd");
                SQLiteCommand cmd = new SQLiteCommand
                {
                    Connection = cn,
                    CommandText = $"insert into t_history values(null, '{model.BuyDate:yyyy-MM-dd}'," +
                                  $" '{model.FundId}', '{model.FundName}', {model.ShangHaiIndex}, " +
                                  $"'{model.ShangHaiIndexJumpPercentage}', {model.FundValue}, " +
                                  $"{model.FundValueJumpPercentage}, {model.BuyAmount}," +
                                  $" {model.AlreaySold}, '{sellDate}', {model.FundValueInSell}," +
                                  $" {model.SellAmount}, {model.Profit}, {model.ProfitPercentage});"
                };
                cmd.ExecuteNonQuery();
                result = true;
            }
            catch (Exception e)
            {
                result = false;
                CombineLog.LogError("InsertBuyResult", e);
            }
            return result;
        }

        public bool UpdateSellResult(HistoryModel model)
        {
            bool result;
            try
            {
                SQLiteConnection cn = new SQLiteConnection(_connStr);
                cn.Open();
                string sellDate = model.SellDate == null ? "" : model.SellDate.Value.ToString("yyyy-MM-dd");
                SQLiteCommand cmd = new SQLiteCommand
                {
                    Connection = cn,
                    CommandText = $"update t_history set 是否已卖出={model.AlreaySold}, 卖出日期 = '{sellDate}', " +
                                  $"卖出净值 = {model.FundValueInSell}," +
                                  $" 卖出金额 = {model.SellAmount},盈利金额 = {model.Profit}," +
                                  $"盈利百分比 = {model.ProfitPercentage} where 序号 = {model.Id};"
                };
                cmd.ExecuteNonQuery();
                result = true;
            }
            catch (Exception e)
            {
                result = false;
                CombineLog.LogError("InsertBuyResult", e);
            }
            return result;
        }
    }
}