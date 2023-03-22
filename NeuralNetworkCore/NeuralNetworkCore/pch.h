#ifndef PCH_H
#define PCH_H

// _USRDLL 是 VS 创建 dll 项目时定义的宏, 其他环境不一定能用
#ifdef _USRDLL
#   define API_DEF __declspec(dllexport)
#else
#   define API_DEF __declspec(dllimport)
#endif // COMPILER_EXPORTS

// 整个库的目的是封装手动实现和基于 opencv 的底层运算, 引用者不需要包含 opencv 的头文件, 因此直接 #define 完事
// 至于库其他部分代码, 由于定义是从 opencv 复制过来的, 因此最多出现重复定义的 warnings, 不会引发其他问题
#define CV_HAL_GEMM_1_T 1
#define CV_HAL_GEMM_2_T 2
#define CV_HAL_GEMM_3_T 4
#define CV_HAL_BORDER_CONSTANT 0
#define CV_HAL_BORDER_REPLICATE 1
#define CV_HAL_BORDER_REFLECT 2
#define CV_HAL_BORDER_WRAP 3
#define CV_HAL_BORDER_REFLECT_101 4
#define CV_HAL_BORDER_TRANSPARENT 5
#define CV_HAL_BORDER_ISOLATED 16

#endif //PCH_H
