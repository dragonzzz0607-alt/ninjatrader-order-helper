# NinjaTrader Order Helper

这是一个把 `order-helper` 里的 MT5 半自动交易助手迁移到 NinjaTrader 8 的新项目。

当前阶段已完成第一阶段纯算法迁移，核心计算代码暂不依赖 NinjaTrader 运行期，便于先在普通 C# 环境中验证。后续开发请先阅读：

- [项目交接文档](D:/work_data/mt5/ninjatrader-order-helper/docs/PROJECT_HANDOFF.md)
- [第一阶段算法设计](D:/work_data/mt5/ninjatrader-order-helper/docs/superpowers/specs/2026-06-06-phase-1-algorithm-design.md)
- [第一阶段实现计划](D:/work_data/mt5/ninjatrader-order-helper/docs/superpowers/plans/2026-06-06-phase-1-algorithm.md)

## 项目目标

在 NinjaTrader 8 Desktop 中实现一个半自动交易面板，用于连接 Tradovate / Lucid Trading 自营账户后辅助下单。第一版目标是复刻现有 MT5 EA 的核心体验：

- 点击图表 K 线后跟踪该 K 线。
- 根据 K 线方向计算有效占比。
- 绘制 Entry、SL、TP 预览线段。
- 输入合约数量后计算预估止损/止盈金额。
- 点击按钮发送 Buy Stop 或 Sell Stop 挂单。
- 显示昨日 High、Low、Open、Close 价位。

## 当前目录状态

```text
ninjatrader-order-helper/
  README.md
  docs/
    PROJECT_HANDOFF.md
    superpowers/
      specs/
        2026-06-06-phase-1-algorithm-design.md
      plans/
        2026-06-06-phase-1-algorithm.md
  src/
    NinjaTrader/
      RiskPreview.cs
      RiskPreviewCalculator.cs
      TradeDirection.cs
      TradePlan.cs
      TradePlanCalculator.cs
  tests/
    NinjaTrader/
      TradePlanCalculatorTests.cs
```

## 第一阶段算法范围

已实现：

- 阳线生成 Buy Stop 计划。
- 阴线生成 Sell Stop 计划。
- 十字星显示有效占比但拒绝下单。
- Entry、SL、TP 按 TickSize 取整。
- 根据合约数量和点值计算 SL/TP 预估金额。
- 非法价格区间、非法合约数量返回中文阻断原因。

暂未实现：

- NinjaTrader Strategy 生命周期。
- 图表内 WPF 面板。
- 图表点击 K 线跟踪。
- Entry、SL、TP 画线。
- 模拟盘或真实账户下单。

## 本地测试

当前环境未发现 `dotnet` 命令，因此第一阶段测试使用 Windows 自带 .NET Framework C# 编译器：

```powershell
& 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe' /nologo /out:tests\NinjaTrader\TradePlanCalculatorTests.exe tests\NinjaTrader\TradePlanCalculatorTests.cs src\NinjaTrader\*.cs
& '.\tests\NinjaTrader\TradePlanCalculatorTests.exe'
```

期望输出：

```text
全部测试通过：8 项。
```

## 重要边界

- 不能直接运行 `.mq5` 或 `.ex5`，必须用 NinjaScript/C# 重写。
- NinjaTrader 中交易数量是合约数或股数，不是 MT5 的手数。
- 自营账户必须遵守 Lucid Trading 和 Tradovate 的交易规则，第一版只做半自动确认下单，不做无人值守全自动策略。
