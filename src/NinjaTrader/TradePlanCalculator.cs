using System;

namespace NinjaTraderOrderHelper
{
    /// <summary>
    /// 根据 K 线 OHLC、TickSize 和偏移 Tick 数计算半自动挂单计划。
    /// </summary>
    public static class TradePlanCalculator
    {
        private const string InvalidRangeMessage = "下单阻断：价格区间无效，无法生成交易计划。";
        private const string DojiMessage = "下单阻断：十字星无明显多空方向，拒绝发送挂单。";

        /// <summary>
        /// 根据单根 K 线数据生成交易计划。
        /// </summary>
        public static TradePlan Calculate(
            double open,
            double high,
            double low,
            double close,
            double tickSize,
            int tradeOffsetTicks)
        {
            double range = high - low;
            if (range <= 0 || tickSize <= 0 || tradeOffsetTicks < 0)
            {
                return new TradePlan(TradeDirection.None, string.Empty, 0, 0, 0, 0, false, InvalidRangeMessage);
            }

            if (close > open)
            {
                double entry = RoundToTick(high + tradeOffsetTicks * tickSize, tickSize);
                double stopLoss = RoundToTick(low - tradeOffsetTicks * tickSize, tickSize);
                double takeProfit = RoundToTick(high + 2 * range, tickSize);
                double effectiveRatio = 1 - ((high - close) / range);

                return new TradePlan(
                    TradeDirection.Long,
                    "Buy Stop",
                    entry,
                    stopLoss,
                    takeProfit,
                    effectiveRatio,
                    true,
                    string.Empty);
            }

            if (close < open)
            {
                double entry = RoundToTick(low - tradeOffsetTicks * tickSize, tickSize);
                double stopLoss = RoundToTick(high + tradeOffsetTicks * tickSize, tickSize);
                double takeProfit = RoundToTick(low - 2 * range, tickSize);
                double effectiveRatio = 1 - ((close - low) / range);

                return new TradePlan(
                    TradeDirection.Short,
                    "Sell Stop",
                    entry,
                    stopLoss,
                    takeProfit,
                    effectiveRatio,
                    true,
                    string.Empty);
            }

            double dojiEffectiveRatio = 1 - ((high - close) / range);
            return new TradePlan(
                TradeDirection.Doji,
                string.Empty,
                0,
                0,
                0,
                dojiEffectiveRatio,
                false,
                DojiMessage);
        }

        /// <summary>
        /// 将价格四舍五入到最接近的 TickSize。
        /// </summary>
        public static double RoundToTick(double price, double tickSize)
        {
            if (tickSize <= 0)
            {
                return price;
            }

            return Math.Round(price / tickSize, 0, MidpointRounding.AwayFromZero) * tickSize;
        }
    }
}
