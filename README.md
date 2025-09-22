# IC-Test-SN74# 集成电路芯片测试项目 - SN74LS245N

这是一个针对德州仪器 (TI) 的 SN74LS245N 八路总线收发器芯片的ATE (自动测试设备) 测试程序。项目在一个基于 P1site 软件的环境中完成，后完成上机测试。覆盖了从测试计划制定、测试代码编写，测试patterns编写到最终上机调试的全过程。


## 🔧 测试环境

* **软件环境**: P1site 
* **目标芯片**: **SN74LS245N** - Octal Bus Transceiver with 3-State Outputs ([查看官方数据手册](https://www.ti.com/lit/ds/symlink/sn74ls245.pdf)).

##  项目文件结构与说明

* `SN74LS245N_YM.xlsm`: **测试计划 (Test Plan)**。使用 Excel 定义了芯片的 Pin Map、Power Sequence、DC/AC 测试项的参数及上下限 (Test Limits)。
* `TestItems.cs`: **C# 测试代码**。包含了项目中所有测试项的实现逻辑，例如 Open/Short 测试、功能测试 (Functional Test) 等。
* `*.atp`: **ATE Test Patterns**。定义了施加于芯片的数字激励信号，用于功能和时序测试。

##  核心技能体现

* **IC 测试原理**: 熟悉数字芯片的基本测试流程 (Continuity, Open/Short, DC, AC, Functional)。
* **Datasheet 解读能力**: 能够从芯片手册中提取测试所需的关键参数。
* **ATE 软件应用**: 具备在 P1site 等专业测试软件环境下开发和调试测试程序的能力。
* **Pattern 设计能力**: 能够根据测试需求（功能、时序、信号捕获）设计和编写不同类型的 ATP Test Patterns。
* **编程能力**: 能够使用 C# 等语言编写测试算法和逻辑。

##  ATP Pattern 设计与分析 

### 1. `ti245_func.atp` - 核心功能验证 Pattern

* **测试目的**:
    验证芯片最核心的功能：**A/B总线间的双向数据传输**。
* **实现逻辑**:
    该Pattern分为两个主要阶段：
    1.  [cite_start]**A -> B 传输测试**: 设置方向控制引脚 `DIR` 为高电平 (`1`) [cite: 76-331][cite_start]，使能输出 `OE` 为低电平 (`0`) [cite: 76-331][cite_start]。此时，在 A 总线 (`A1-A8`) 上施加一个从全0到全1的**计数信号**，同时在 B 总线 (`B1-B8`) 上用 `L` 和 `H` 来**期望 (Expect)** 接收到完全一样的数据 [cite: 76-331]。
    2.  [cite_start]**B -> A 传输测试**: 设置 `DIR` 为低电平 (`0`) [cite: 332-587][cite_start]，反转数据流向。此时，在 B 总线上施加激励信号，并在 A 总线上进行比较 [cite: 332-587]。
* **物理意义**:
    通过覆盖所有256种数据组合并测试两个方向，该Pattern确保了芯片的8个并行数据通道都是通畅且无误的。

### 2. `ti245_time.atp` - AC时序参数测试 Pattern

* **测试目的**:
    专门用于测试芯片的**AC时序特性**，特别是其三态门 (3-State Outputs) 的**输出使能 (Enable) 和禁止 (Disable) 时间**。
* **实现逻辑**:
    1.  [cite_start]首先将芯片置于一个确定的数据传输状态（例如 A->B 传输全1数据）[cite: 14-17]。
    2.  [cite_start]然后，关键的一步是**翻转 `OE` (Output Enable) 引脚的状态**（从 `0` 变为 `1`）[cite: 12, 15]。
    3.  [cite_start]在 `OE` 变为 `1` 的瞬间，Pattern在输出总线B上设置了 `M` (Mask) 状态 [cite: 12, 15]。`M` 告诉测试机在该周期**忽略**此引脚的输出，因为此时总线应处于**高阻态 (High-Z)**。
* **物理意义**:
    通过精确控制 `OE` 的翻转和 `M` (Mask) 的使用，这个Pattern创造了可以用来测量 `tPZL` (从低电平到高阻态的延迟) 和 `tPHZ` (从高电平到高阻态的延迟) 等关键AC时序参数的条件，验证了芯片的动态响应速度是否达标。

### 3. `ti245_cap.atp` - 数字信号捕获 Pattern

* **测试目的**:
    用于更高级的**信号表征 (Characterization) 或调试**，通过硬件直接捕获芯片输出的数字波形。
* **实现逻辑**:
    1.  [cite_start]**仪器配置**: Pattern的头部包含了特殊的 `instruments` 定义，将 `B8` 引脚配置为一个**数字捕获**通道 `DigCap` [cite: 589]。
    2.  [cite_start]**触发与捕获**: Pattern中包含 `trig` 关键字，它作为硬件捕获的触发信号 [cite: 716][cite_start]。在触发后的向量中，B总线的某些引脚被标记为 `V` (Capture) [cite: 717-724]。



##  工作流程

1.  **Datasheet 分析**: 深入阅读 SN74LS245N 芯片的数据手册，提取其电气特性 (Electrical Characteristics) 和时序特性 (Switching Characteristics) 的关键参数。
2.  **测试计划制定**: 在 `SN74LS245N_YM.xlsm` 文件中，根据 Datasheet 定义了芯片的管脚配置，并为 DC、AC 和功能测试设定了详细的测试条件与 Pass/Fail 标准。
3.  **测试代码开发**: 在 `TestItems.cs` 中，使用 C# 语言和 P1site 提供的 API，编写了多个测试函数，以编程方式实现了测试计划中的各个项目。
4.  **软件调试**: 在 P1site 软件环境中，加载测试程序并进行调试，通过分析测试结果，验证了测试计划的正确性。
5.  **上机测试**：用P1测试机连接芯片上电运行测试程序，最终验证了芯片的正确功能。

##  上机实测流程 

本项目已成功在真实的ATE机台上完成测试。完整的工作流程如下：

### 1. 软件编译与加载
在 P1site 软件环境中，将 `TestItems.cs` 中的测试逻辑与 `.atp` 文件进行编译和链接，生成机台可执行的测试程序 。
下图展示了在 P1site 中成功编译并加载测试程序后的界面：
**** ![P1site 编译与加载界面]https://i.postimg.cc/D0BBsH0Y/4724f58b-3080-407d-97aa-9a1d60bec2b0.png
### 2. 硬件连接与准备
* **安放芯片**: 将 SN74LS245N 芯片正确放置在测试板的 ZIF Socket 中。
* **连接机台**: 将测试板安装到对应测试头slot带上，并与机台完成对接。
* **加载程序**: 在机台控制电脑上，加载第一步生成的测试程序。

下文件为本次测试所用 Loadboard 的连接原理图，清晰地表明了芯片管脚与 ATE 测试通道之间的对应关系：

**[ 点击此处查看 Loadboard 原理图 (PDF)](./PowerValue_Loadboard_DIP48&SOP8.pdf)**


### 3. 执行测试与结果分析
* **运行程序**: 启动测试程序，程序会自动控制机台的电源 (DPS)、数字通道 (Pin Electronics) 等模块，按照 `Test Plan` 的设定，对芯片执行一系列测试。
* **结果分析**: 查看程序运行后生成的测试报告 (Test Report) 或日志 (Log)，确认所有测试项 (Test Items) 均显示 **PASS** 状态，证明芯片功能完好。


