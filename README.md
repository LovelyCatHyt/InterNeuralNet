# InterNeuralNet
可交互的神经网络参数实时渲染系统.

## Build
如果只在 Unity 模块上开发, 可以直接按照一般的 Unity 项目处理, 但需要运行环境包含 opencv 运行时 (但后续可能直接添加到 `Assets/Plugins/` 中, 减少烦恼).

cpp 模块依赖了 opencv 进行矩阵运算. 如果是 **Visual Studio** 用户, 按照以下步骤配置环境及设置 **NeuralNetworkCore** 项目属性:
1. 环境变量 `Path` 添加 opencv 的运行时 (以"build\x64\vc16\bin"结尾).
2. `C/C++ -> 常规 -> 附加包含目录` 加上 opencv 包含目录 (以"build\include"结尾). 
3. `链接器 -> 常规 -> 附加库目录` 加上 lib 目录 (以"build\x64\vc16\lib"结尾)
4. `链接器 -> 输入 -> 附加依赖项` 加上 "opencv_world470.lib;" (也可以是其他对应的版本, 只要接口相同都可以运行)

如果使用其他构建工具链, 查找对应的配置方法. 除了配置 opencv 依赖外, 还需正确配置 **NeuralNetworkCLI** 对 **NeuralNetworkCore** 的依赖.

Unity 模块已包含一个可运行时自动链接非托管库的脚本 (但后续可能会移除), 只需将新编译的 NeuralNetworkCore 二进制文件复制到 `Assets/Plugins` 下即可在 Unity 中使用.

## License
See [MIT License](./LICENSE)