#ifndef PCH_H
#define PCH_H

// _USRDLL 是 VS 创建 dll 项目时定义的宏, 其他环境不一定能用
#ifdef _USRDLL
#   define API_DEF __declspec(dllexport)
#else
#   define API_DEF __declspec(dllimport)
#endif // COMPILER_EXPORTS

#ifndef CV_HAL_GEMM_1_T
#   define CV_HAL_GEMM_1_T 1
#endif // !CV_HAL_GEMM_1_T

#ifndef CV_HAL_GEMM_2_T
#   define CV_HAL_GEMM_2_T 2
#endif // !CV_HAL_GEMM_2_T

#ifndef CV_HAL_GEMM_3_T
#   define CV_HAL_GEMM_3_T 4
#endif // !CV_HAL_GEMM_3_T


#endif //PCH_H
