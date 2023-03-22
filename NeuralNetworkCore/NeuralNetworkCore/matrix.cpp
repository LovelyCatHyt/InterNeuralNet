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

template<class T>
void line_pooling_max(T* src, int src_step, int src_len, T* dst, int dst_step, int size)
{
    deque<float> q;
    // 读取[0, size)范围内的数字, 初始化单调队列
    for (size_t i = 0; i < size; i++)
    {
        auto num = src[i * src_step];
        while (!q.empty() && q.back() < num)
        {
            q.pop_back();
        }
        q.push_back(num);
    }
    dst[0] = q.front();
    for (size_t i = size; i < src_len; i++)
    {
        auto num = src[i * src_step];
        while (!q.empty() && q.back() < num)
        {
            q.pop_back();
        }
        q.push_back(num);
        if (q.size() > size) q.pop_front();
        dst[(i - size + 1) * dst_step] = q.front();
    }
}

template<class T>
void line_pooling_min(T* src, int src_step, int src_len, T* dst, int dst_step, int size)
{
    deque<float> q;
    // 从上面复制的, 只是改了符号
    for (size_t i = 0; i < size; i++)
    {
        auto num = src[i * src_step];
        while (!q.empty() && q.back() > num)
        {
            q.pop_back();
        }
        q.push_back(num);
    }
    dst[0] = q.front();
    for (size_t i = size; i < src_len; i++)
    {
        auto num = src[i * src_step];
        while (!q.empty() && q.back() > num)
        {
            q.pop_back();
        }
        q.push_back(num);
        if (q.size() > size) q.pop_front();
        dst[(i - size + 1) * dst_step] = q.front();
    }
}

void pooling_max(const mat_float& src, const mat_float& dst, int size)
{
    // 没有任何有效性检查, 因为跨语言抛异常有点抽象了, 最简单的实现也需要特定的函数签名格式
    mat_float temp = create_mat_float(dst.width, src.height);
    // 横向求一维的池化
    for (size_t row = 0; row < src.height; row++)
    {
        line_pooling_max(src.ptr + row * src.width, 1, src.width, temp.ptr + row * temp.width, 1, size);
    }
    // 纵向求一维的池化, 即为最终结果
    for (size_t col = 0; col < dst.width; col++)
    {
        line_pooling_max(temp.ptr + col, temp.width, temp.height, dst.ptr + col, dst.width, size);
    }
}

void pooling_min(const mat_float& src, const mat_float& dst, int size)
{
    // 从上面复制的
    mat_float temp = create_mat_float(dst.width, src.height);
    // 横向求一维的池化
    for (size_t row = 0; row < src.height; row++)
    {
        line_pooling_min(src.ptr + row * src.width, 1, src.width, temp.ptr + row * temp.width, 1, size);
    }
    // 纵向求一维的池化, 即为最终结果
    for (size_t col = 0; col < dst.width; col++)
    {
        line_pooling_min(temp.ptr + col, temp.width, temp.height, dst.ptr + col, dst.width, size);
    }
}

void pooling_mean(const mat_float& src, const mat_float& dst, int size)
{
    // 均值化直接用常规的滑动窗口完事
    // 也可以用上面类似的方法来消掉k倍时间复杂度, 但没必要...吧?
    auto sum = 0.0f;
    // 预算好的除法, 后面就不需要除法
    auto divider = 1.0f / size / size;
    // 初始化, 由于size^2通常较小, 懒得写指针缓存了
    for (size_t row = 0; row < size; row++)
    {
        for (size_t col = 0; col < size; col++)
        {
            sum += get(src, row, col);
        }
    }
    // 为了保留窗口, 蛇形遍历输出的每个元素位置
    for (size_t row = 0; row < dst.height; row++)
    {
        auto dst_row_ptr = dst.ptr + dst.width * row;
        if (row % 2 == 0)
        {
            // 从左往右
            for (size_t col = 0; col < dst.width; col++)
            {
                dst_row_ptr[col] = sum * divider;
                if (col < dst.width - 1)
                {
                    for (size_t k = 0; k < size; k++)
                    {
                        sum -= get(src, row + k, col);
                        sum += get(src, row + k, col + size);
                    }
                }
            }
            // 防止读取越界
            if (row < dst.height - 1)
            {
                for (size_t k = 0; k < size; k++)
                {
                    sum -= get(src, row, src.width - k - 1);
                    sum += get(src, row + size, src.width - k - 1);
                }
            }
        }
        else
        {
            // 从右往左
            for (int col = dst.width - 1; col >= 0; col--)
            {
                dst_row_ptr[col] = sum * divider;
                if (col > 0)
                {
                    for (size_t k = 0; k < size; k++)
                    {
                        sum -= get(src, row + k, col + size - 1);
                        sum += get(src, row + k, col - 1);
                    }
                }
            }
            // 防止读取越界
            if (row < dst.height - 1)
            {
                for (size_t k = 0; k < size; k++)
                {
                    sum -= get(src, row, k);
                    sum += get(src, row + size, k);
                }
            }
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

void API_DEF convolution_flag(const mat_float& src, const mat_float& dst, const mat_float& kernel, int border_type)
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
