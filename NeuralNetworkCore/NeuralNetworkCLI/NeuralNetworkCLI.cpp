#include <iostream>
#include <chrono>
#include <stdlib.h>
#include "matrix.h"
#include "NeuralNetworkCLI.h"

void naive_multiply(const mat_float& a, const mat_float& b, mat_float dst)
{
    for (size_t row = 0; row < dst.height; row++)
    {
        for (size_t col = 0; col < dst.width; col++)
        {
            auto sum = 0.0f;
            auto ptrA = a.ptr + a.width * row;
            for (size_t i = 0; i < a.width; i++)
            {
                // 因为 b 不是按行读取 所以指针计算方式不同
                auto ptrB = b.ptr + b.width * i;
                sum += ptrA[i] * ptrB[col];
            }
            dst.ptr[row * dst.width + col] = sum;
        }
    }
}

using std::chrono::high_resolution_clock;
using std::chrono::duration_cast;
using std::chrono::milliseconds;

void test_multiply()
{
    float raw_mat1[] =
    {
        1,0,6,0,
        2,1,4,3,
        3,0,1,0,
        0,0,0,1,
        -2,1,3,5,
        0,0,1,0,
        0,0,0,1,
        -2,1,3,5,
        0,0,1,0,
        0,0,0,1,
        2,-3,4,1,
        3,1,3,5,
        8,-1,-2,-3,
        -2,3,5,7,
        4,3,2,1,
        1,2,3,4,
    };
    float raw_mat2[] =
    {
        0,2,1,0,2,0,0,1,0,2,1,3,1,2,3,5,
        0,1,0,0,3,0,1,0,0,3,-6,2,-1,3,4,1,
        5,0,1,0,4,0,0,1,0,4,2,0,0,7,2,1,
        -1,0,7,1,6,0,0,0,1,6,0,8,5,3,2,1,
    };

    mat_float mat1 = { 4,16, raw_mat1 };
    mat_float mat2 = { 16,4, raw_mat2 };
    mat_float mat3 = { 16,16 };
    mat3.ptr = reinterpret_cast<float*>(malloc(mat3.width * mat3.height * sizeof(float)));
    std::cout << "mat1: " << std::endl;
    print(mat1);
    std::cout << "mat2: " << std::endl;
    print(mat2);

    std::cout << "------------------Multiply Start--------------------" << std::endl;
    auto repeatCount = 10000;
    auto start = high_resolution_clock::now();

    // opencv 算法
    for (size_t i = 0; i < repeatCount; i++)
    {
        multiply(mat1, mat2, mat3);
    }
    auto count = duration_cast<milliseconds>(high_resolution_clock::now() - start).count();
    std::cout << "opencv algorithm time: " << count << "ms, result is:\n";
    print(mat3);

    // 我的朴素算法
    start = high_resolution_clock::now();
    for (size_t i = 0; i < repeatCount; i++)
    {
        naive_multiply(mat1, mat2, mat3);
    }
    count = duration_cast<milliseconds>(high_resolution_clock::now() - start).count();
    std::cout << "my naive algorithm time: " << count << "ms, result is:\n";
    print(mat3);

    free(mat3.ptr);
}

void test_pooling()
{
    float raw_mat1[] =
    {
        1,0,6,0,
        2,1,4,3,
        3,0,1,0,
        0,0,0,1,
        -2,1,3,5,
        0,0,1,0,
        0,0,0,1,
        -2,1,3,5,
        0,0,1,0,
        0,0,0,1,
        2,-3,4,1,
        3,1,3,5,
        8,-1,-2,-3,
        -2,3,5,7,
        4,3,2,1,
        1,2,3,4,
    };
    mat_float mat1 = { 4,16, raw_mat1 };
    int kernel_size = 3;
    mat_float pooling_output{ mat1.width - kernel_size + 1, mat1.height - kernel_size + 1 };
    pooling_output.ptr = (float*)malloc(sizeof(float) * pooling_output.width * pooling_output.height);

    std::cout << "------------------Pooling Test--------------------" << std::endl;

    pooling_max(mat1, pooling_output, kernel_size);
    std::cout << "------------------Max Pooling--------------------" << std::endl;
    print(pooling_output);

    pooling_min(mat1, pooling_output, kernel_size);
    std::cout << "------------------Min Pooling--------------------" << std::endl;
    print(pooling_output);

    pooling_mean(mat1, pooling_output, kernel_size);
    std::cout << "------------------Mean Pooling--------------------" << std::endl;
    print(pooling_output, 8);
    free(pooling_output.ptr);
}

void test_convolution()
{
    float mat1_raw[] =
    {
           2,1,1,1,1,
           1,1,1,1,1,
           1,1,1,1,1,
           1,1,1,1,1,
           1,1,1,1,1,
    };

    float dst_raw[25] = { -1 };

    float kernel_raw[] =
    {
        1,1,1,
        1,1,1,
        1,1,1,
    };

    mat_float src{ 5,5,mat1_raw };
    mat_float dst{ 5,5,dst_raw };
    mat_float kernel{ 3,3,kernel_raw };
    std::cout << "------------------Convolution--------------------\n";
    std::cout << "src:\n";
    print(src);
    std::cout << "kernel:\n";
    print(kernel);
    try
    {
        convolution_flag(src, dst, kernel, CV_HAL_BORDER_REFLECT);
    }
    catch (const std::exception& e)
    {
        std::cout << "error occorred:\n" << e.what() << std::endl;
    }
    std::cout << "concolution result:\n";
    print(dst);
}

void test_addition()
{
    float mat1_raw[] =
    {
           0,1,1,1,1,
           0,1,1,1,1,
           0,1,1,1,1,
           0,1,1,1,1,
           0,1,1,1,1,
    };
    float mat2_raw[] =
    {
           1.14f,1,1,1,2,
           1.14f,1,1,1,2,
           1.14f,1,1,1,2,
           1.14f,1,1,1,2,
           1.14f,1,1,1,2,
    };
    float dst_raw[25];

    mat_float a{5,5,mat1_raw};
    mat_float b{5,5,mat2_raw};
    mat_float dst{5,5,dst_raw};
    add(a,b,dst);
    print(dst);
}

int main()
{
    // test_multiply();

    // test_pooling();

    // test_convolution();

    test_addition();
}
