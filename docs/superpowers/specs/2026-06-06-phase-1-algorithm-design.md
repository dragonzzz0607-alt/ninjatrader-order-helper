# NinjaTrader 交易助手第一阶段算法设计

## 目标

第一阶段只迁移 MT5 交易助手中的纯计算逻辑，产出可以在普通 C# 环境中编译和测试的核心算法代码。该阶段不接入 NinjaTrader 运行期、不创建 WPF 面板、不提交真实或模拟订单。

## 范围

- 根据一根 K 线的 Open、High、Low、Close 计算方向。
- 根据文档规则计算有效占比。
- 根据 TickSize 和 TradeOffsetTicks 计算 Entry、SL、TP。
- 根据合约数量和点值计算 SL/TP 预估金额。
- 对十字星、非法价格区间、非法数量等情况返回明确状态。

## 架构

源码放在 `src/NinjaTrader`，保持 NinjaScript/C# 友好，不依赖 NuGet 包或 NinjaTrader 专有类型。测试放在 `tests/NinjaTrader`，使用一个轻量级控制台测试运行器，通过系统自带 C# 编译器直接编译运行。

核心类型：

- `TradeDirection`：交易方向枚举，表达多、空、十字星和无效状态。
- `TradePlan`：交易计划数据对象，保存方向、价格、有效占比和阻断原因。
- `TradePlanCalculator`：根据 OHLC、TickSize、偏移 Tick 计算交易计划。
- `RiskPreview`：风险预览数据对象，保存 SL/TP 预估金额。
- `RiskPreviewCalculator`：根据交易计划、合约数量和点值计算预估盈亏。

## 计算规则

阳线：

```text
Entry = High + TradeOffsetTicks * TickSize
SL    = Low  - TradeOffsetTicks * TickSize
TP    = High + 2 * (High - Low)
```

阴线：

```text
Entry = Low  - TradeOffsetTicks * TickSize
SL    = High + TradeOffsetTicks * TickSize
TP    = Low  - 2 * (High - Low)
```

十字星不允许下单，但仍按文档要求显示有效占比。所有价格按 TickSize 取整。

## 错误处理

第一阶段不抛出交易异常，而是在返回对象中保存中文阻断原因。这样后续 Strategy、WPF 面板和日志可以复用同一套提示文字。

主要阻断原因：

- `下单阻断：价格区间无效，无法生成交易计划。`
- `下单阻断：十字星无明显多空方向，拒绝发送挂单。`
- `下单阻断：请输入正确的合约数量。`

## 测试

测试运行器覆盖交接文档中的阳线、阴线、十字星和非法数量用例，并补充无效 K 线区间、TickSize 取整用例。测试命令不依赖 .NET SDK，优先使用：

```powershell
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe
```

## 明确不做

- 不做 NinjaTrader Strategy 生命周期。
- 不做 WPF 面板。
- 不做图表点击跟踪。
- 不做绘图。
- 不做订单提交。
- 不读取账户或连接状态。
