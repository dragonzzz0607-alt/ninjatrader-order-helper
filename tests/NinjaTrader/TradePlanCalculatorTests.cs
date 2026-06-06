using System;
using NinjaTraderOrderHelper;

public static class TradePlanCalculatorTests
{
    private const double Tolerance = 0.0000001;
    private static int passed;
    private static int failed;

    public static int Main()
    {
        Run("阳线应生成 Buy Stop 交易计划", BullishBarCreatesBuyStopPlan);
        Run("阴线应生成 Sell Stop 交易计划", BearishBarCreatesSellStopPlan);
        Run("十字星应显示有效占比但拒绝下单", DojiShowsEffectiveRatioButBlocksOrder);
        Run("无效价格区间应拒绝生成可下单计划", InvalidRangeBlocksOrderablePlan);
        Run("价格应按 TickSize 取整", PricesAreRoundedToTickSize);
        Run("多单风险预览应计算止损和止盈金额", LongRiskPreviewCalculatesAmounts);
        Run("空单风险预览应计算止损和止盈金额", ShortRiskPreviewCalculatesAmounts);
        Run("非法合约数量应阻断风险预览", InvalidQuantityBlocksRiskPreview);

        if (failed > 0)
        {
            Console.WriteLine("测试失败：{0} 项通过，{1} 项失败。", passed, failed);
            return 1;
        }

        Console.WriteLine("全部测试通过：{0} 项。", passed);
        return 0;
    }

    private static void BullishBarCreatesBuyStopPlan()
    {
        TradePlan plan = TradePlanCalculator.Calculate(100, 110, 95, 108, 0.25, 1);

        AssertEqual(TradeDirection.Long, plan.Direction, "方向");
        AssertTrue(plan.CanSubmitOrder, "阳线应允许下单");
        AssertEqual("Buy Stop", plan.OrderType, "订单类型");
        AssertNear(110.25, plan.EntryPrice, "Entry");
        AssertNear(94.75, plan.StopLossPrice, "SL");
        AssertNear(140, plan.TakeProfitPrice, "TP");
        AssertNear(0.8666666667, plan.EffectiveRatio, "有效占比");
        AssertEqual("", plan.BlockingReason, "阻断原因");
    }

    private static void BearishBarCreatesSellStopPlan()
    {
        TradePlan plan = TradePlanCalculator.Calculate(110, 112, 100, 102, 0.25, 1);

        AssertEqual(TradeDirection.Short, plan.Direction, "方向");
        AssertTrue(plan.CanSubmitOrder, "阴线应允许下单");
        AssertEqual("Sell Stop", plan.OrderType, "订单类型");
        AssertNear(99.75, plan.EntryPrice, "Entry");
        AssertNear(112.25, plan.StopLossPrice, "SL");
        AssertNear(76, plan.TakeProfitPrice, "TP");
        AssertNear(0.8333333333, plan.EffectiveRatio, "有效占比");
        AssertEqual("", plan.BlockingReason, "阻断原因");
    }

    private static void DojiShowsEffectiveRatioButBlocksOrder()
    {
        TradePlan plan = TradePlanCalculator.Calculate(100, 105, 95, 100, 0.25, 1);

        AssertEqual(TradeDirection.Doji, plan.Direction, "方向");
        AssertFalse(plan.CanSubmitOrder, "十字星不应允许下单");
        AssertEqual("", plan.OrderType, "订单类型");
        AssertNear(0.5, plan.EffectiveRatio, "有效占比");
        AssertEqual("下单阻断：十字星无明显多空方向，拒绝发送挂单。", plan.BlockingReason, "阻断原因");
    }

    private static void InvalidRangeBlocksOrderablePlan()
    {
        TradePlan plan = TradePlanCalculator.Calculate(100, 100, 100, 100, 0.25, 1);

        AssertEqual(TradeDirection.None, plan.Direction, "方向");
        AssertFalse(plan.CanSubmitOrder, "无效区间不应允许下单");
        AssertEqual("下单阻断：价格区间无效，无法生成交易计划。", plan.BlockingReason, "阻断原因");
    }

    private static void PricesAreRoundedToTickSize()
    {
        TradePlan plan = TradePlanCalculator.Calculate(100, 110.12, 95.13, 108, 0.25, 1);

        AssertTrue(plan.CanSubmitOrder, "阳线应允许下单");
        AssertNear(110.25, plan.EntryPrice, "Entry 应取整到 TickSize");
        AssertNear(95.00, plan.StopLossPrice, "SL 应取整到 TickSize");
        AssertNear(140.00, plan.TakeProfitPrice, "TP 应取整到 TickSize");
    }

    private static void LongRiskPreviewCalculatesAmounts()
    {
        TradePlan plan = TradePlanCalculator.Calculate(100, 110, 95, 108, 0.25, 1);
        RiskPreview preview = RiskPreviewCalculator.Calculate(plan, 1, 50);

        AssertTrue(preview.CanPreview, "应允许风险预览");
        AssertNear(-775, preview.StopLossAmount, "SL 金额");
        AssertNear(1487.5, preview.TakeProfitAmount, "TP 金额");
        AssertEqual("", preview.BlockingReason, "阻断原因");
    }

    private static void ShortRiskPreviewCalculatesAmounts()
    {
        TradePlan plan = TradePlanCalculator.Calculate(110, 112, 100, 102, 0.25, 1);
        RiskPreview preview = RiskPreviewCalculator.Calculate(plan, 1, 50);

        AssertTrue(preview.CanPreview, "应允许风险预览");
        AssertNear(-625, preview.StopLossAmount, "SL 金额");
        AssertNear(1187.5, preview.TakeProfitAmount, "TP 金额");
        AssertEqual("", preview.BlockingReason, "阻断原因");
    }

    private static void InvalidQuantityBlocksRiskPreview()
    {
        TradePlan plan = TradePlanCalculator.Calculate(100, 110, 95, 108, 0.25, 1);
        RiskPreview preview = RiskPreviewCalculator.Calculate(plan, 0, 50);

        AssertFalse(preview.CanPreview, "非法数量不应允许风险预览");
        AssertEqual("下单阻断：请输入正确的合约数量。", preview.BlockingReason, "阻断原因");
    }

    private static void Run(string name, Action test)
    {
        try
        {
            test();
            passed++;
            Console.WriteLine("[通过] {0}", name);
        }
        catch (Exception ex)
        {
            failed++;
            Console.WriteLine("[失败] {0}", name);
            Console.WriteLine(ex.Message);
        }
    }

    private static void AssertTrue(bool actual, string message)
    {
        if (!actual)
        {
            throw new InvalidOperationException(message + "：预期为 true，实际为 false。");
        }
    }

    private static void AssertFalse(bool actual, string message)
    {
        if (actual)
        {
            throw new InvalidOperationException(message + "：预期为 false，实际为 true。");
        }
    }

    private static void AssertNear(double expected, double actual, string message)
    {
        if (Math.Abs(expected - actual) > Tolerance)
        {
            throw new InvalidOperationException(
                string.Format("{0}：预期 {1}，实际 {2}。", message, expected, actual));
        }
    }

    private static void AssertEqual<T>(T expected, T actual, string message)
    {
        if (!object.Equals(expected, actual))
        {
            throw new InvalidOperationException(
                string.Format("{0}：预期 {1}，实际 {2}。", message, expected, actual));
        }
    }
}
