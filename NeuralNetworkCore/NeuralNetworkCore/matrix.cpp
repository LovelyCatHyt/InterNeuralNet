#include "pch.h"
#include "matrix.h"
#include <opencv2/core/hal/hal.hpp>
#include <opencv2/imgproc/hal/hal.hpp>
#include <stdlib.h>
#include <iostream>
#include <deque>

// 既不会全部引入导致污染命名空间, 又不用浪费时间打 "std::"
using std::cout;
using std::endl;
using std::deque;

float get(const mat_float& mat, int row, int col)
{
    return *(mat.ptr + row * mat.width + col);
}
void set(const mat_float& mat, int row, int col, float value)
{
    *(mat.ptr + row * mat.width + col) = value;
}

double get(const mat_double& mat, int row, int col)
{
    return *(mat.ptr + row * mat.width + col);
}
void set(const mat_double& mat, int row, int col, double value)
{
    *(mat.ptr + row * mat.width + col) = value;
}

/// <summary>
/// 构造一个矩阵, 其中具体数据的存储空间用 malloc 分配. 放在这里是因为不希望显式导出含内存分配相关的接口, 但池化和卷积运算可能用得上
/// </summary>
/// <param name="width"></param>
/// <param name="height"></param>
/// <returns></returns>
mat_float create_mat_float(int width, int height)
{
    mat_float res{ width, height };
    res.ptr = (float*)malloc(sizeof(float) * width * height);
    return res;
}

void add(const mat_float& a, const mat_float& b, const mat_float& dst)
{
    cv::hal::add32f
    (
        a.ptr,
        a.width * sizeof(float),
        b.ptr,
        b.width * sizeof(float),
        dst.ptr,
        dst.width * sizeof(float),
        a.width,
        a.height,
        0   /* 最后一个void*参数完全猜不出是干什么的, 反正填个NULL能运行...就先不管了 */
    );
}

void add_scalar(const mat_float& a, float scalar, const mat_float& dst)
{
    auto len = a.width * a.height;
    float* __restrict a_ptr = a.ptr;
    float* __restrict dst_ptr = dst.ptr;
    for (size_t i = 0; i < len; i++)
    {
        dst_ptr[i] = a_ptr[i] + scalar;
    }
}

void multiply(const mat_float& a, const mat_float& b, const mat_float& dst)
{
    cv::hal::gemm32f
    (
        a.ptr,
        a.width * sizeof(float),
        b.ptr,
        b.width * sizeof(float),
        1,
        NULL,
        0,
        0,
        dst.ptr,
        dst.width * sizeof(float),
        a.height,
        a.width,
        b.width,
        0
    );
}

void multiply_flag(const mat_float& a, const mat_float& b, const mat_float& dst, int flag)
{
    cv::hal::gemm32f
    (
        a.ptr,
        a.width * sizeof(float),
        b.ptr,
        b.width * sizeof(float),
        1,
        NULL,
        0,
        0,
        dst.ptr,
        dst.width * sizeof(float),
        a.height,
        a.width,
        b.width,
        flag
    );
}

void print(const mat_float& mat, int number_width)
{
    auto ptr = mat.ptr;
    for (size_t row = 0; row < mat.height; row++)
    {
        auto line_ptr = ptr + mat.width * row;
        for (size_t col = 0; col < mat.width - 1; col++)
        {
            cout.width(number_width);
            cout << std::right << line_ptr[col] << ' ';
        }
        cout.width(number_width);
        cout << line_ptr[mat.width - 1] << endl;
    }
}

//template<class T>
//void line_pooling_max(T* src, int src_step, int src_len, T* dst, int dst_step, int size, int stride = 1)
//{
//    deque<float> q;
//    // 读取[0, size)范围内的数字, 初始化单调队列
//    for (size_t i = 0; i < size; i++)
//    {
//        auto num = src[i * src_step];
//        while (!q.empty() && q.back() < num)
//        {
//            q.pop_back();
//        }
//        q.push_back(num);
//    }
//    dst[0] = q.front();
//    for (size_t i = size; i < src_len; i += stride)
//    {
//        for (size_t j = 0; j < stride; j++)
//        {
//            auto num = src[(i + j) * src_step];
//            while (!q.empty() && q.back() < num)
//            {
//                q.pop_back();
//            }
//            q.push_back(num);
//        }
//        while (q.size() > size) q.pop_front();
//        dst[(i / stride - size + 1) * dst_step] = q.front();
//    }
//}
//
//template<class T>
//void line_pooling_min(T* src, int src_step, int src_len, T* dst, int dst_step, int size)
//{
//    deque<float> q;
//    // 从上面复制的, 只是改了符号
//    for (size_t i = 0; i < size; i++)
//    {
//        auto num = src[i * src_step];
//        while (!q.empty() && q.back() > num)
//        {
//            q.pop_back();
//        }
//        q.push_back(num);
//    }
//    dst[0] = q.front();
//    for (size_t i = size; i < src_len; i++)
//    {
//        auto num = src[i * src_step];
//        while (!q.empty() && q.back() > num)
//        {
//            q.pop_back();
//        }
//        q.push_back(num);
//        if (q.size() > size) q.pop_front();
//        dst[(i - size + 1) * dst_step] = q.front();
//    }
//}

void pooling_max(const mat_float& src, const mat_float& dst, int size)
{
    // 没有任何有效性检查, 因为跨语言抛异常有点抽象了, 最简单的实现也需要特定的函数签名格式
    for (size_t row = 0; row * size < src.height; row++)
    {
        for (size_t col = 0; col * size < src.width; col++)
        {
            auto max = src.ptr[row * size * src.width + col * size];
            for (size_t y = 0; y < size; y++)
            {
                for (size_t x = 0; x < size; x++)
                {
                    max = MAX(max, src.ptr[(row * size + y) * src.width + col * size + x]);
                }
            }
            dst.ptr[row * dst.width + col] = max;
        }
    }
}

void pooling_min(const mat_float& src, const mat_float& dst, int size)
{
    // 基本从上面复制的
    for (size_t row = 0; row * size < src.height; row++)
    {
        for (size_t col = 0; col * size < src.width; col++)
        {
            auto min = src.ptr[row * size * src.width + col * size];
            for (size_t y = 0; y < size; y++)
            {
                for (size_t x = 0; x < size; x++)
                {
                    min = MIN(min, src.ptr[(row * size + y) * src.width + col * size + x]);
                }
            }
            dst.ptr[row * dst.width + col] = min;
        }
    }
}

void pooling_mean(const mat_float& src, const mat_float& dst, int size)
{
    auto sum = 0.0f;
    // 预算好的除法, 后面就不需要除法
    auto divider = 1.0f / size / size;
    for (size_t row = 0; row * size < src.height; row++)
    {
        for (size_t col = 0; col * size < src.width; col++)
        {
            auto sum = 0;
            for (size_t y = 0; y < size; y++)
            {
                for (size_t x = 0; x < size; x++)
                {
                    sum += src.ptr[(row * size + y) * src.width + col * size + x];
                }
            }
            dst.ptr[row * dst.width + col] = sum * divider;
        }
    }
}

void convolution(const mat_float& src, const mat_float& dst, const mat_float& kernel)
{
    cv::hal::filter2D
    (
        CV_32FC1,
        CV_32FC1,
        CV_32FC1,
        (uchar*)src.ptr,
        src.width * sizeof(float),
        (uchar*)dst.ptr,
        dst.width * sizeof(float),
        src.width,
        src.height,
        dst.width,
        dst.height,
        0,
        0,
        (uchar*)kernel.ptr,
        kernel.width * sizeof(float),
        kernel.width,
        kernel.height,
        -1,
        -1,
        0,
        CV_HAL_BORDER_REPLICATE,
        false
    );
}

void convolution_flag(const mat_float& src, const mat_float& dst, const mat_float& kernel, int border_type)
{
    cv::hal::filter2D
    (
        CV_32FC1,
        CV_32FC1,
        CV_32FC1,
        (uchar*)src.ptr,
        src.width * sizeof(float),
        (uchar*)dst.ptr,
        dst.width * sizeof(float),
        src.width,
        src.height,
        dst.width,
        dst.height,
        0,
        0,
        (uchar*)kernel.ptr,
        kernel.width * sizeof(float),
        kernel.width,
        kernel.height,
        -1,
        -1,
        0,
        border_type,
        false
    );
}

void pooling(const mat_float& src, const mat_float& dst, int size, POOLING_TYPE pooling_type)
{
    switch (pooling_type)
    {
    case CORE_POOLING_MAX:
        pooling_max(src, dst, size);
        break;
    case CORE_POOLING_MIN:
        pooling_min(src, dst, size);
        break;
    case CORE_POOLING_MEAN:
        pooling_mean(src, dst, size);
        break;
    default:
        break;
    }
}
