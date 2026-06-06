namespace NinjaTraderOrderHelper
{
    /// <summary>
    /// 表示交易计划根据 K 线形态识别出的交易方向。
    /// </summary>
    public enum TradeDirection
    {
        /// <summary>
        /// 无法识别有效方向，通常表示价格区间无效。
        /// </summary>
        None = 0,

        /// <summary>
        /// 阳线方向，对应 Buy Stop 挂单计划。
        /// </summary>
        Long = 1,

        /// <summary>
        /// 阴线方向，对应 Sell Stop 挂单计划。
        /// </summary>
        Short = 2,

        /// <summary>
        /// 十字星方向，仅展示有效占比，不允许发送挂单。
        /// </summary>
        Doji = 3
    }
}
