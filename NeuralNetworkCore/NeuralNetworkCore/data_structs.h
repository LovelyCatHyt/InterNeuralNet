#pragma once
#include "pch.h"

#pragma pack(8)
typedef struct Mat_Base
{
	int width;
	int height;
	void* ptr;
};

#pragma pack(8)
typedef struct Mat_Double
{
	int width;
	int height;
	double* ptr;
};

#pragma pack(8)
typedef struct Mat_Float
{
	int width;
	int height;
	float* ptr;
};