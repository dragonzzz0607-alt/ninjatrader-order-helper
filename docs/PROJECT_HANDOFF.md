# NinjaTrader 半自动交易面板项目交接文档

## 1. 项目是什么

本项目是现有 MT5 项目 `D:\work_data\mt5\order-helper` 的 NinjaTrader 8 迁移版本。原项目核心文件是 `TradeHelperPanel.mq5`，它是一个运行在 MetaTrader 5 图表上的半自动交易 EA。

新项目目标是在 NinjaTrader 8 Desktop 中实现同等核心功能，让用户可以使用 Lucid Trading 自营账户，并通过 Tradovate / NinjaTrader 连接进行半自动交易辅助。

第一版不追求全自动策略，不追求复杂仓位管理。它的定位是：

> 用户手动选择 K 线，工具计算交易计划并预览风险，用户确认后点击按钮发送挂单。

## 2. 为什么要迁移

当前账户实际交易环境是 Tradovate，且该账户也可以登录 NinjaTrader。MT5 的 `.mq5/.ex5` 无法在 Tradovate 或 NinjaTrader 中直接运行。

NinjaTrader 8 的 NinjaScript 基于 C#，适合实现：

- 图表级半自动交易面板。
- K 线数据读取。
- 图表绘图。
- 托管或非托管订单提交。
- Tradovate / NinjaTrader 连接下的期货交易。

因此本项目采用 NinjaTrader 8 作为目标平台，而不是 Tradovate 原生自定义指标或 Tradovate API 外部程序。

## 3. 原 MT5 项目功能清单

原始 MT5 EA 的功能来自 `D:\work_data\mt5\order-helper\TradeHelperPanel.mq5` 和 `readme.md`。

### 3.1 面板功能

- 图表右下区域显示交易助手面板。
- 面板可拖动。
- 面板可折叠和展开。
- 面板可关闭。
- 按钮包括：
  - 清空画图。
  - 昨日价位。
  - 重置跟踪。
  - 取消跟踪。
  - 发送挂单。
- 输入框：
  - 手数输入。
- 信息显示：
  - 有效占比。
  - SL 预估金额。
  - TP 预估金额。

### 3.2 K 线跟踪

- 默认跟踪当前 K 线。
- 用户点击图表上的 K 线后，EA 跟踪该 K 线。
- 点击重置跟踪后回到当前 K 线。
- 点击取消跟踪后清除交易预览线，并进入等待跟踪状态。

### 3.3 有效占比

计算规则：

```text
总区间 = High - Low

阳线:
有效占比 = 1 - ((High - Close) / 总区间)

阴线:
有效占比 = 1 - ((Close - Low) / 总区间)

十字星:
沿用阳线方向的展示算法，但不允许下单
```

展示格式示例：

```text
有效占比: 90.00%
```

### 3.4 交易计划

原 MT5 参数 `TradeOffsetTicks` 默认值为 `1`。

阳线时：

```text
订单类型: Buy Stop
Entry = High + TradeOffsetTicks * TickSize
SL    = Low  - TradeOffsetTicks * TickSize
TP    = High + 2 * (High - Low)
```

阴线时：

```text
订单类型: Sell Stop
Entry = Low  - TradeOffsetTicks * TickSize
SL    = High + TradeOffsetTicks * TickSize
TP    = Low  - 2 * (High - Low)
```

十字星：

```text
不发送挂单
```

### 3.5 画线

- Entry、SL、TP 为水平线段，不是无限延伸线。
- 线段从被跟踪 K 线时间开始，向右延伸固定数量 K 线。
- 昨日价位为水平线：
  - 昨日 High。
  - 昨日 Low。
  - 昨日 Open。
  - 昨日 Close。

### 3.6 下单

MT5 使用 `CTrade`：

- 阳线调用 `BuyStop`。
- 阴线调用 `SellStop`。
- 下单时带入 Entry、SL、TP。

NinjaTrader 中不能直接照搬。后续需要使用 NinjaScript 的托管订单方法、ATM，或 Account API 重新实现。

## 4. NinjaTrader 目标版本范围

目标平台：

```text
NinjaTrader 8 Desktop
```

目标账户：

```text
Lucid Trading 自营账户
Tradovate / NinjaTrader 连接
```

第一版目标：

- 只做半自动交易。
- 只在图表中运行。
- 用户必须手动点击按钮确认发送挂单。
- 不做无人值守自动循环交易。
- 不做高频交易。
- 不做跨账户复制交易。

## 5. 推荐架构

推荐第一版采用：

```text
NinjaScript Strategy + 图表内 WPF 控件面板
```

原因：

- Strategy 天然能读取当前图表 K 线。
- Strategy 天然能提交订单。
- Strategy 可以使用 `Draw.Line` 等绘图工具。
- WPF 控件可以实现接近 MT5 面板的按钮、输入框和状态显示。
- 对后续接入 ATM 或 OCO 保护单更友好。

不建议第一版直接使用纯 AddOn：

- AddOn 权限更大，但账户、图表和订单同步复杂度更高。
- 初期实现成本更高。
- 对当前需求来说容易过度设计。

不建议第一版使用 Tradovate API 外部程序：

- 需要额外处理登录、令牌、行情订阅、订单状态同步、网络重连。
- 对自营账户来说，外部程序出错后的风险更高。
- 无法天然复用 NinjaTrader 图表交互。

## 6. 建议文件结构

后续开发建议创建如下结构：

```text
ninjatrader-order-helper/
  README.md
  docs/
    PROJECT_HANDOFF.md
    DEVELOPMENT_PLAN.md
    TESTING_CHECKLIST.md
  src/
    NinjaTrader/
      TradeHelperPanelStrategy.cs
      TradeHelperPanelControl.cs
      TradePlan.cs
      TradePlanCalculator.cs
      RiskPreviewCalculator.cs
      DrawingService.cs
      OrderSubmissionService.cs
      ChartInteractionService.cs
```

文件职责：

```text
TradeHelperPanelStrategy.cs
  NinjaScript Strategy 入口，负责生命周期、数据访问、策略参数、服务组装。

TradeHelperPanelControl.cs
  WPF 面板控件，负责按钮、输入框、状态展示和用户事件。

TradePlan.cs
  交易计划数据对象，包含方向、Entry、SL、TP、跟踪 K 线信息。

TradePlanCalculator.cs
  根据 K 线 OHLC 和 TickSize 计算有效占比、Entry、SL、TP。

RiskPreviewCalculator.cs
  根据 Instrument、数量、Entry、SL、TP 计算预估盈亏金额。

DrawingService.cs
  绘制和清理 Entry、SL、TP、昨日价位。

OrderSubmissionService.cs
  提交 Buy Stop / Sell Stop 订单，并处理止损止盈保护单。

ChartInteractionService.cs
  处理图表鼠标点击，将 X/Y 坐标转换成 K 线索引。
```

## 7. NinjaTrader 与 MT5 的关键差异

### 7.1 语言和生命周期

MT5：

```text
MQL5
OnInit()
OnDeinit()
OnTick()
OnChartEvent()
```

NinjaTrader：

```text
C# / NinjaScript
OnStateChange()
OnBarUpdate()
OnRender()
图表事件 / WPF 事件
```

迁移时不能逐行翻译。需要按 NinjaTrader 生命周期重新组织代码。

### 7.2 数量单位

MT5 的 `lot` 不等于 NinjaTrader 的数量。

NinjaTrader 期货交易通常使用：

```text
1 = 1 张合约
```

因此 UI 上不应该继续写“手数”，建议改成：

```text
合约数量
```

默认值建议：

```text
1
```

### 7.3 盈亏计算

MT5 使用：

```text
OrderCalcProfit()
```

NinjaTrader 需要按合约点值计算。期货常用公式：

```text
价格差 = 目标价格 - 入场价格
点值金额 = Instrument.MasterInstrument.PointValue
预估盈亏 = 价格差 * 点值金额 * 合约数量
```

空单方向要注意符号：

```text
Sell Stop:
SL 金额通常为负数
TP 金额通常为正数
```

实际实现时要使用 NinjaTrader 的价格取整方法，确保价格符合 TickSize。

### 7.4 订单和保护单

MT5 可以在 `BuyStop/SellStop` 中直接带 SL/TP。

NinjaTrader 可选方案：

方案 A：Strategy 托管订单

```text
EnterLongStopMarket()
EnterShortStopMarket()
SetStopLoss()
SetProfitTarget()
```

优点：

- 实现相对简单。
- 适合第一版。

缺点：

- 对多计划、多订单、精细 OCO 管理不够灵活。

方案 B：ATM Strategy

优点：

- 更贴近人工交易和 Chart Trader。
- 保护单模板可视化。

缺点：

- NinjaScript 调用和状态管理更复杂。

方案 C：Unmanaged / Account API

优点：

- 控制力最强。

缺点：

- 最容易写出订单状态 bug。
- 第一版不推荐。

第一版推荐：

```text
先用 Strategy 托管订单实现。
如托管订单无法满足 Lucid/Tradovate 的实盘行为，再评估 ATM。
```

## 8. 第一版功能验收标准

### 8.1 面板

必须支持：

- 显示在图表上。
- 可输入合约数量。
- 显示有效占比。
- 显示 SL 预估金额。
- 显示 TP 预估金额。
- 提供发送挂单按钮。
- 提供取消跟踪按钮。
- 提供重置跟踪按钮。
- 提供昨日价位按钮。

可以暂缓：

- 完整拖拽。
- 完整折叠动画。
- 高度还原 MT5 配色。

### 8.2 K 线跟踪

必须支持：

- 默认使用当前 K 线。
- 点击历史 K 线后切换跟踪目标。
- 重置后回到当前 K 线。
- 取消后清除预览线，不允许下单。

### 8.3 交易计划

必须支持：

- 阳线生成 Buy Stop。
- 阴线生成 Sell Stop。
- 十字星拒绝下单。
- Entry、SL、TP 与 MT5 原规则保持一致。
- 所有价格按 TickSize 取整。

### 8.4 下单保护

必须支持：

- 合约数量必须大于 0。
- 未跟踪 K 线时拒绝下单。
- 十字星拒绝下单。
- 同一 Strategy 实例不能连续重复发送同一计划。
- 下单前后打印中文日志。
- 订单失败时显示中文错误信息。

建议支持：

- 当前账户连接状态检查。
- 当前品种交易时段检查。
- 当前是否已有未完成订单检查。

### 8.5 Lucid / Tradovate 风控边界

必须遵守：

- 不实现高频交易。
- 不实现无人值守循环自动交易。
- 不实现跨账户复制。
- 不实现对冲规避规则的逻辑。
- 不实现低延迟刷单。

建议在代码和文档中明确：

```text
本工具是半自动辅助下单工具，所有挂单都必须由用户手动点击按钮触发。
```

## 9. 开发阶段建议

### 阶段 1：纯算法迁移

目标：

- 实现 `TradePlan`。
- 实现 `TradePlanCalculator`。
- 实现有效占比计算。
- 实现 Entry、SL、TP 计算。

验证：

- 用固定 OHLC 数据写单元测试。
- 对比 MT5 计算结果。

### 阶段 2：NinjaScript Strategy 骨架

目标：

- 创建 Strategy。
- 暴露参数：
  - `TradeOffsetTicks`。
  - `DefaultQuantity`。
  - `TradeLineBars`。
- 在 `OnBarUpdate()` 中更新当前 K 线计划。

验证：

- Strategy 能加载到 NinjaTrader 图表。
- 日志能输出中文初始化信息。

### 阶段 3：绘图

目标：

- 绘制 Entry、SL、TP 线段。
- 绘制昨日价位。
- 清理本工具创建的绘图对象。

验证：

- 图表上价格位置正确。
- 切换 K 线后旧线被替换。
- 取消跟踪后预览线消失。

### 阶段 4：WPF 面板

目标：

- 嵌入图表面板。
- 实现按钮和输入框。
- 实现状态显示。

验证：

- 面板不遮挡关键交易区域。
- 按钮点击能触发 Strategy 方法。
- 数量输入异常时显示中文提示。

### 阶段 5：图表点击跟踪

目标：

- 捕获图表鼠标点击。
- 将点击位置转换为 K 线索引。
- 更新跟踪目标。

验证：

- 点击当前 K 线、历史 K 线都能正确更新。
- 点击面板区域不会误触发跟踪。

### 阶段 6：模拟盘下单

目标：

- 实现 Buy Stop / Sell Stop 挂单。
- 实现 SL/TP 保护。
- 打印中文下单日志。

验证：

- 只在 Sim 账户测试。
- 检查订单价格、方向、数量、保护单。
- 检查取消、拒单、成交后的状态。

### 阶段 7：Tradovate / Lucid 小规模验证

目标：

- 使用 Lucid 允许的模拟或评估环境验证。
- 先用微型合约和最小数量。

验证：

- 确认订单进入正确账户。
- 确认保护单行为符合预期。
- 确认没有违反 Lucid 规则的自动行为。

## 10. 建议测试用例

### 10.1 阳线

输入：

```text
Open = 100
High = 110
Low = 95
Close = 108
TickSize = 0.25
TradeOffsetTicks = 1
Quantity = 1
PointValue = 50
```

期望：

```text
方向 = Long
Entry = 110.25
SL = 94.75
TP = 140
有效占比 = 86.6667%
SL 金额 = (94.75 - 110.25) * 50 * 1 = -775
TP 金额 = (140 - 110.25) * 50 * 1 = 1487.5
```

### 10.2 阴线

输入：

```text
Open = 110
High = 112
Low = 100
Close = 102
TickSize = 0.25
TradeOffsetTicks = 1
Quantity = 1
PointValue = 50
```

期望：

```text
方向 = Short
Entry = 99.75
SL = 112.25
TP = 76
有效占比 = 83.3333%
SL 金额 = (99.75 - 112.25) * 50 * 1 = -625
TP 金额 = (99.75 - 76) * 50 * 1 = 1187.5
```

### 10.3 十字星

输入：

```text
Open = 100
High = 105
Low = 95
Close = 100
```

期望：

```text
允许显示有效占比
不生成可下单交易计划
点击发送挂单时显示中文拒绝信息
```

### 10.4 非法数量

输入：

```text
Quantity = 0
```

期望：

```text
不发送订单
显示或打印：下单阻断：请输入正确的合约数量。
```

### 10.5 取消跟踪

操作：

```text
点击取消跟踪
点击发送挂单
```

期望：

```text
预览线消失
面板显示等待跟踪
不发送订单
显示或打印：下单阻断：请先点击K线开始跟踪。
```

## 11. 开发注意事项

### 11.1 中文注释和日志

如果新增 Java 代码，需要按上级 `AGENTS.md` 要求写注释。但本项目目标语言是 C#/NinjaScript，不是 Java。

尽管如此，本项目仍建议：

- 关键 C# 类写中文 XML 注释。
- 关键 public 方法写中文 XML 注释。
- 交易、拒单、异常日志尽量用中文。

日志示例：

```text
交易助手初始化完成，等待用户选择K线。
下单阻断：十字星无明显多空方向，拒绝发送挂单。
Buy Stop 挂单已提交，合约数量：1，Entry：110.25，SL：94.75，TP：140。
```

### 11.2 不要清理用户不相关文件

原 MT5 功能有“清空画图”。NinjaTrader 版本中第一版只应该清理本工具创建的绘图对象。

不要默认删除用户手动画线或其它指标绘图，避免误删分析内容。

### 11.3 下单必须保守

第一版任何下单逻辑都应该保守：

- 默认数量为 1。
- 用户点击按钮才下单。
- 不自动重复下单。
- 不在 `OnBarUpdate()` 中自动触发真实订单。
- 同一跟踪 K 线和同一价格计划只允许提交一次，除非用户重置或重新选择 K 线。

### 11.4 先 Sim 后真实账户

所有交易功能必须先在 NinjaTrader Sim 账户验证。

验证顺序：

```text
Playback / Market Replay
Sim101
Lucid 评估环境
真实可交易环境
```

不要跳过模拟环境。

## 12. 后续开发者从哪里开始

建议按下面顺序继续：

1. 在本目录创建 `src/NinjaTrader`。
2. 先写 `TradePlan.cs` 和 `TradePlanCalculator.cs`。
3. 用普通 C# 测试或手工测试验证交易计划算法。
4. 创建 NinjaTrader Strategy 骨架。
5. 在 NinjaTrader 8 中导入并编译 Strategy。
6. 实现画线。
7. 实现 WPF 面板。
8. 实现图表点击跟踪。
9. 实现模拟盘下单。
10. 最后再连接 Tradovate / Lucid 环境验证。

## 13. 官方文档入口

开发时优先查 NinjaTrader 官方文档：

- NinjaScript Strategy 生命周期：`OnStateChange`
- K 线更新：`OnBarUpdate`
- 托管订单：`EnterLongStopMarket`、`EnterShortStopMarket`
- 止损止盈：`SetStopLoss`、`SetProfitTarget`
- 绘图：`Draw.Line`、`Draw.HorizontalLine`
- AddOn / WPF：AddOn Development Overview
- 账户与订单高级控制：Account API、Unmanaged Approach

## 14. 当前未决策事项

以下事项需要在写代码前确认：

1. 第一版是否只支持期货合约。
2. 第一版是否只支持当前图表品种。
3. 第一版是否必须使用 ATM 模板。
4. 是否需要完全复刻 MT5 面板的拖拽和折叠。
5. 是否需要保留“清空画图”功能，还是改为“清理本工具画线”。

当前推荐默认值：

```text
只支持期货合约。
只支持当前图表品种。
第一版不用 ATM，先用 Strategy 托管订单。
拖拽和折叠放到第二版。
清图只清理本工具创建的对象。
```
