# TouchSocket 使用示例合集

## 项目简介

本仓库是为解答 TouchSocket 使用问题而创建的示例代码合集。每个 Demo 文件夹都包含了针对特定问题或使用场景的完整示例代码，旨在帮助开发者更好地理解和使用 TouchSocket 框架。

欢迎开发者们提交新的示例代码，共同完善这个知识库！

## TouchSocket 是什么？

TouchSocket 是一个基于 .NET 的高性能网络通信框架，支持 TCP、UDP、HTTP、WebSocket、DMTP 等多种协议，提供了丰富的插件系统和 RPC 功能。

## 示例目录

### Demo001 - ASP.NET Core 集成 DMTP 服务
**位置**: [Demo001/](Demo001/)

演示如何在 ASP.NET Core Web 应用中集成 TouchSocket 的 DMTP 服务。包含：
- Web API 项目结构
- DMTP 服务器配置
- 客户端连接示例
- 令牌验证机制

### Demo002 - Web 应用与类库项目
**位置**: [Demo002/](Demo002/)

展示 Web 应用项目与类库项目的组合使用：
- 独立的类库项目
- Web 应用项目
- 项目间的依赖关系

### Demo003 - WebSocket 插件系统
**位置**: [Demo003/](Demo003/)

演示 WebSocket 服务器的插件系统使用：
- HTTP 服务器配置
- WebSocket 升级处理
- 多个 WebSocket 插件
- 连接验证机制
- 自动 Pong 响应

### Demo004 - 跨服务端、跨协议 DMTP RPC 调用
**位置**: [Demo004/](Demo004/)

解决不同 DMTP 客户端如何跨服务端、跨协议进行 RPC 调用的问题：
- HTTP 客户端实现
- TCP 客户端实现
- 服务器端配置
- 跨协议通信

### Demo005 - WebSocket 服务器
**位置**: [Demo005/](Demo005/)

WebSocket 服务器的完整实现：
- ASP.NET Core 集成
- WebSocket 连接管理
- 配置文件设置

### Demo006 - DMTP RPC Actor 问题解决
**位置**: [Demo006/](Demo006/)

解决"DmtpRpcActor为空，可能需要启用DmtpRpc插件"错误：
- Windows Forms 服务端
- 控制台客户端
- RPC 插件正确配置

### Demo007 - TCP 客户端服务端示例
**位置**: [Demo007/](Demo007/)

基础的 TCP 客户端和服务端通信示例：
- 客户端实现
- 服务端实现
- 基本通信协议

### Demo008 - DMTP RPC 调用链路由
**位置**: [Demo008/](Demo008/)

演示 DMTP RPC 调用链的路由问题解决方案：
- 多客户端架构 (ClientB, ClientC)
- 服务器 A 路由配置
- 调用链路由机制

### Demo009 - TCP 基础通信
**位置**: [Demo009/](Demo009/)

TCP 通信的基础示例和最佳实践。

### Demo010 - TCP 服务创建等待客户端超时问题
**位置**: [Demo010/](Demo010/)

解决"TcpService中的Received事件中，创建了CreateWaitingClient，总是提示超时"问题：
- 超时问题诊断
- 正确的客户端创建方式
- 事件处理优化

### Demo011 - WebSocket 客户端连接问题
**位置**: [Demo011/](Demo011/)

解决 WebSocket 客户端连接其他服务器时显示"操作已被取消"的问题：
- 连接超时处理
- 取消操作处理
- 错误诊断方法

### Demo012 - TCP 高级功能
**位置**: [Demo012/](Demo012/)

TCP 通信的高级功能演示。

### Demo013 - DMTP RPC 高级用法
**位置**: [Demo013/](Demo013/)

DMTP RPC 的高级使用场景：
- HTTP 客户端 RPC
- TCP 客户端 RPC
- 服务器端 RPC 处理

### Demo014 - HTTP 客户端文件上传边界问题
**位置**: [Demo014/](Demo014/)

解决"HttpClient文件上传会影响GetBoundary"问题：
- 文件上传实现
- 边界处理
- HTTP 客户端优化

### Demo015 - Blazor 应用集成
**位置**: [Demo015/](Demo015/)

演示如何在 Blazor 应用中集成 TouchSocket：
- Bootstrap Blazor 应用
- TouchSocket 服务集成

### Demo016 - 网络桥接服务
**位置**: [Demo016/](Demo016/)

网络桥接服务的实现：
- 桥接服务器
- 客户端连接
- 服务器端处理

### Demo017 - Windows Forms 应用
**位置**: [Demo017/](Demo017/)

Windows Forms 桌面应用中使用 TouchSocket：
- WinForms 界面设计
- TouchSocket 集成
- 桌面应用通信

### Demo018 - 客户端与服务端双向通信
**位置**: [Demo018/](Demo018/)

演示客户端与服务端之间的双向通信：
- 双向数据交换
- 连接状态管理
- 通信协议设计

### Demo019 - 客户端对客户端文件传输
**位置**: [Demo019/](Demo019/)

实现客户端之间直接进行文件传输：
- P2P 文件传输
- DMTP 文件传输插件
- 路由服务器
- RPC 调用协调

### Demo020 - TCP 通信演示
**位置**: [Demo020/](Demo020/)

TCP 通信的完整演示项目：
- 客户端实现
- 服务端实现
- 通信协议

### Demo021 - 自定义协议实现
**位置**: [Demo021/](Demo021/)

演示如何实现自定义通信协议：
- 客户端协议实现
- 服务端协议处理

### Demo022 - 服务器测试套件
**位置**: [Demo022/](Demo022/)

服务器功能的测试套件：
- 测试客户端
- 服务器实现
- 性能测试

### Demo023 - TCP 服务器客户端架构
**位置**: [Demo023/](Demo023/)

完整的 TCP 服务器客户端架构示例：
- 服务器架构设计
- 客户端连接管理
- 项目结构组织

### Demo024 - RPC 更新测试
**位置**: [Demo024/](Demo024/)

RPC 功能的更新和测试：
- RPC 服务器更新
- 旧版本客户端兼容性
- 新版本客户端测试
- 升级迁移指南

---

## 如何使用

1. 克隆本仓库到本地
2. 选择相关的 Demo 文件夹
3. 使用 Visual Studio 或 VS Code 打开解决方案文件
4. 根据 Demo 说明运行项目

## 贡献指南

欢迎提交新的示例代码！请确保：
- 代码结构清晰，注释完整
- 包含问题描述和解决方案
- 提供运行说明
- 遵循现有的目录结构

## 相关链接

- [TouchSocket 官方文档](https://touchsocket.net/)
- [TouchSocket GitHub](https://github.com/RRQM/TouchSocket)
- [TouchSocket Gitee](https://gitee.com/RRQM_Home/TouchSocket)

## 许可证

本项目采用 MIT 许可证，详情请查看 [LICENSE](LICENSE) 文件。