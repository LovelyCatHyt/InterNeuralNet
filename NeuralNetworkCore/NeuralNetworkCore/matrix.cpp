#include "pch.h"
#include "matrix.h"
#include <opencv2/core/hal/hal.hpp>
#include <iostream>

// �Ȳ���ȫ�����뵼����Ⱦ�����ռ�, �ֲ����˷�ʱ��� "std::"
using std::cout;
using std::endl;

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
