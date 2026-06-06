namespace NinjaTraderOrderHelper
{
    /// <summary>
    /// 保存交易计划对应的止损和止盈金额预估。
    /// </summary>
    public sealed class RiskPreview
    {
        /// <summary>
        /// 创建风险预览数据对象。
        /// </summary>
        public RiskPreview(bool canPreview, double stopLossAmount, double takeProfitAmount, string blockingReason)
        {
            CanPreview = canPreview;
            StopLossAmount = stopLossAmount;
            TakeProfitAmount = takeProfitAmount;
            BlockingReason = blockingReason ?? string.Empty;
        }

        /// <summary>
        /// 当前计划是否可以展示有效的风险预览。
        /// </summary>
        public bool CanPreview { get; private set; }

        /// <summary>
        /// 触发止损时的预估金额，通常为负数。
        /// </summary>
        public double StopLossAmount { get; private set; }

        /// <summary>
        /// 触发止盈时的预估金额，通常为正数。
        /// </summary>
        public double TakeProfitAmount { get; private set; }

        /// <summary>
        /// 无法计算风险预览时展示给用户或日志的中文原因。
        /// </summary>
        public string BlockingReason { get; private set; }
    }
}
