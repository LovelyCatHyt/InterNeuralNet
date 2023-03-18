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

void API_DEF print(const mat_float& mat, int number_width = 4);
