#pragma once
#include "pch.h"
// 忽略: warning C4091: “typedef ”: 没有声明变量时忽略“mat_base”的左侧
#pragma warning(push)
#pragma warning(disable: 4091)

#pragma pack(push)
#pragma pack(8)
typedef struct mat_base
{
    int width;
    int height;
    void* ptr;
};

typedef struct mat_float
{
    int width;
    int height;
    float* ptr;
};

typedef struct mat_double
{
    int width;
    int height;
    double* ptr;
};
#pragma pack(pop)

#pragma warning(pop)

void API_DEF add(const mat_float& a, const mat_float& b, const mat_float& dst);
void API_DEF add_scalar(const mat_float& a, float scalar, const mat_float& dst);

/// <summary>
/// 矩阵乘法, 调用 opencv 的广义矩阵乘法(gemm)函数
/// 参数描述见 https://docs.opencv.org/4.7.0/d2/dab/group__core__hal__interface__matrix__multiplication.html
/// 但使用的是 https://docs.opencv.org/4.7.0/d3/ddd/group__core__hal__functions.html#ga8894272aecb229343b39d377b908fd1f
/// 暂时不知道有什么区别
/// </summary>
/// <param name="a"></param>
/// <param name="b"></param>
/// <param name="dst"></param>
/// <returns></returns>
void API_DEF multiply(const mat_float& a, const mat_float& b, const mat_float& dst);
/// <summary>
/// 矩阵乘法, 调用 opencv 的广义矩阵乘法(gemm)函数, 与 multiply 相比多一个参数, 用于控制输入是否转置
/// 参数描述见 https://docs.opencv.org/4.7.0/d2/dab/group__core__hal__interface__matrix__multiplication.html
/// 但使用的是 https://docs.opencv.org/4.7.0/d3/ddd/group__core__hal__functions.html#ga8894272aecb229343b39d377b908fd1f
/// 暂时不知道有什么区别
/// </summary>
/// <param name="a"></param>
/// <param name="b"></param>
/// <param name="dst"></param>
/// <returns></returns>
void API_DEF multiply_flag(const mat_float& a, const mat_float& b, const mat_float& dst, int flag = 0);

void API_DEF print(const mat_float& mat, int number_width = 4);

void API_DEF pooling_max(const mat_float& src, const mat_float& dst, int size);

void API_DEF pooling_min(const mat_float& src, const mat_float& dst, int size);

void API_DEF pooling_mean(const mat_float& src, const mat_float& dst, int size);

// 卷积, 但只保证奇数核, 且 padding = size/2 或 0 的两种情况
void API_DEF convolution(const mat_float& src, const mat_float& dst, const mat_float& kernel, int padding = 0);

// 卷积, 但只保证奇数核, 且 padding = size/2 或 0 的两种情况
void API_DEF convolution_flag(const mat_float& src, const mat_float& dst, const mat_float& kernel, int padding = 0, int border_type = CV_HAL_BORDER_REPLICATE);

enum POOLING_TYPE
{
    CORE_POOLING_MAX = 1,
    CORE_POOLING_MIN = 2,
    CORE_POOLING_MEAN = 3
};

void API_DEF pooling(const mat_float& src, const mat_float& dst, int size, POOLING_TYPE pooling_type);
