#include <iostream>
#include "data_structs.h"

int main()
{
	std::cout << sizeof(Mat) << std::endl;
	auto m = Mat();
	std::cout << sizeof(m.ptr) << std::endl;
}