namespace NinjaTraderOrderHelper
{
    /// <summary>
    /// 保存一根被跟踪 K 线计算出的交易计划和下单阻断信息。
    /// </summary>
    public sealed class TradePlan
    {
        /// <summary>
        /// 创建交易计划数据对象。
        /// </summary>
        public TradePlan(
            TradeDirection direction,
            string orderType,
            double entryPrice,
            double stopLossPrice,
            double takeProfitPrice,
            double effectiveRatio,
            bool canSubmitOrder,
            string blockingReason)
        {
            Direction = direction;
            OrderType = orderType ?? string.Empty;
            EntryPrice = entryPrice;
            StopLossPrice = stopLossPrice;
            TakeProfitPrice = takeProfitPrice;
            EffectiveRatio = effectiveRatio;
            CanSubmitOrder = canSubmitOrder;
            BlockingReason = blockingReason ?? string.Empty;
        }

        /// <summary>
        /// 交易方向。
        /// </summary>
        public TradeDirection Direction { get; private set; }

        /// <summary>
        /// NinjaTrader 中展示给用户的挂单类型名称。
        /// </summary>
        public string OrderType { get; private set; }

        /// <summary>
        /// 计划入场价格。
        /// </summary>
        public double EntryPrice { get; private set; }

        /// <summary>
        /// 计划止损价格。
        /// </summary>
        public double StopLossPrice { get; private set; }

        /// <summary>
        /// 计划止盈价格。
        /// </summary>
        public double TakeProfitPrice { get; private set; }

        /// <summary>
        /// K 线有效占比，范围通常为 0 到 1。
        /// </summary>
        public double EffectiveRatio { get; private set; }

        /// <summary>
        /// 当前计划是否允许提交挂单。
        /// </summary>
        public bool CanSubmitOrder { get; private set; }

        /// <summary>
        /// 不允许提交挂单时展示给用户或日志的中文原因。
        /// </summary>
        public string BlockingReason { get; private set; }
    }
}
