namespace NinjaTraderOrderHelper
{
    /// <summary>
    /// 根据交易计划、合约数量和品种点值计算预估止损与止盈金额。
    /// </summary>
    public static class RiskPreviewCalculator
    {
        private const string InvalidQuantityMessage = "下单阻断：请输入正确的合约数量。";
        private const string InvalidPlanMessage = "下单阻断：当前交易计划不可下单，无法计算风险预览。";

        /// <summary>
        /// 计算指定交易计划的风险预览金额。
        /// </summary>
        public static RiskPreview Calculate(TradePlan plan, int quantity, double pointValue)
        {
            if (quantity <= 0)
            {
                return new RiskPreview(false, 0, 0, InvalidQuantityMessage);
            }

            if (pointValue <= 0 || plan == null || !plan.CanSubmitOrder)
            {
                return new RiskPreview(false, 0, 0, InvalidPlanMessage);
            }

            if (plan.Direction == TradeDirection.Long)
            {
                double stopLossAmount = (plan.StopLossPrice - plan.EntryPrice) * pointValue * quantity;
                double takeProfitAmount = (plan.TakeProfitPrice - plan.EntryPrice) * pointValue * quantity;
                return new RiskPreview(true, stopLossAmount, takeProfitAmount, string.Empty);
            }

            if (plan.Direction == TradeDirection.Short)
            {
                double stopLossAmount = (plan.EntryPrice - plan.StopLossPrice) * pointValue * quantity;
                double takeProfitAmount = (plan.EntryPrice - plan.TakeProfitPrice) * pointValue * quantity;
                return new RiskPreview(true, stopLossAmount, takeProfitAmount, string.Empty);
            }

            return new RiskPreview(false, 0, 0, InvalidPlanMessage);
        }
    }
}
