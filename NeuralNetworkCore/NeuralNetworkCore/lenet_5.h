#pragma once
#include <memory>
#include <string>
#include "pch.h"
#include "matrix.h"

/*
lenet参数数据结构:
'conv1.weight'  (6, 1, 5, 5)    padding=2
'conv1.bias'    (6,)            padding=0
'conv2.weight'  (16, 6, 5, 5)
'conv2.bias'    (16,)
'fc1.weight'    (120, 400)
'fc1.bias'      (120,)
'fc2.weight'    (84, 120)
'fc2.bias'      (84,)
'fc3.weight'    (10, 84)
'fc3.bias'      (10,)

运算过程:
28*28                   ->  relu(conv2d(in 1, out 6, 5) padding = 2)    -> 6 channel * (28,28)
6 channel * (28,28)     ->  maxpooling(2)                               -> 6 channel * (14,14)
6 channel * (14,14)     ->  relu(conv2d(in 6, out 16, 5) padding = 0)   -> 16 channel * (10*10)
16 channel * (10*10)    ->  maxpooling(2)                               -> 16 channel * (5*5)
16*5*5                  ->  full connect(16*5*5, 120)                   -> 120
120                     ->  full connect(120, 84)                       -> 84
84                      ->  full connect(84, 10)                        -> 10
*/

class API_DEF lenet
{
public:
    lenet();
    ~lenet();
    void read_from_file(const char* path);
    // 等价于 this->eval(img)
    int operator()(const mat_float& img);
    // 这个函数仅仅是纯粹的 eval, 分配的内存空间会在函数返回前释放
    int eval(const mat_float& img);
    void test(const char* test_file, const char* label_file, int max_test_count = 1000);
    std::unique_ptr<mat_float[]> c1;
    mat_float c1_bias;
    std::unique_ptr<mat_float[]> c2;
    mat_float c2_bias;
    mat_float f1;
    mat_float f1_bias;
    mat_float f2;
    mat_float f2_bias;
    mat_float f3;
    mat_float f3_bias;
private:    
};
