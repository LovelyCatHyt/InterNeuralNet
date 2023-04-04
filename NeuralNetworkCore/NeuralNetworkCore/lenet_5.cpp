#include <stdio.h>
#include "pch.h"
#include "lenet_5.h"

using std::unique_ptr;
using std::make_unique;

void allocate(mat_float& mat)
{
    mat.ptr = new float[mat.width * mat.height];
}

void swap_bytes(int* num)
{
    *num = ((*num >> 24) & 0xff) |  // move byte 3 to byte 0
        ((*num << 8) & 0xff0000) |  // move byte 1 to byte 2
        ((*num >> 8) & 0xff00) |    // move byte 2 to byte 1
        ((*num << 24) & 0xff000000);// byte 0 to byte 3
}

void swap_bytes(int* buffer, int count_of_int)
{
    for (size_t i = 0; i < count_of_int; i++)
    {
        swap_bytes(buffer + i);
    }
}

void print_arr(const float* arr, int len)
{
    for (size_t i = 0; i < len; i++)
    {
        printf("%f ", arr[i]);
    }
    printf("\n");
}

lenet::lenet() :
    c1{ new mat_float[6] },
    c1_bias{ 6, 1 },
    c2{ new mat_float[16 * 6] },
    c2_bias{ 6, 16 },
    f1{ 400, 120 },
    f1_bias{ 1, 120 },
    f2{ 120, 84 },
    f2_bias{ 1, 84 },
    f3{ 84, 10 },
    f3_bias{ 1, 10 }
{
    // 由于这两个卷积层的卷积核数据是贴在一起的, 这样分配方便 memcpy 或其他能快速写入目标内存的函数
    auto c1_data = new float[25 * 6];
    for (size_t i = 0; i < 6; i++)
    {
        c1[i] = { 5, 5, c1_data };
        c1_data += 25;
    }
    allocate(c1_bias);

    auto c2_data = new float[25 * 6 * 16];
    for (size_t i = 0; i < 6 * 16; i++)
    {
        c2[i] = { 5, 5, c2_data };
        c2_data += 25;
    }
    allocate(c2_bias);

    allocate(f1);
    allocate(f1_bias);
    allocate(f2);
    allocate(f2_bias);
    allocate(f3);
    allocate(f3_bias);
}

lenet::~lenet()
{
    free(c1[0].ptr);
    free(c1_bias.ptr);
    free(c2[0].ptr);
    free(c2_bias.ptr);
    free(f1.ptr);
    free(f1_bias.ptr);
    free(f2.ptr);
    free(f2_bias.ptr);
    free(f3.ptr);
    free(f3_bias.ptr);
}

void lenet::read_from_file(const char* path)
{
    FILE* f;
    fopen_s(&f, path, "rb");
    if (f == NULL)
    {
        printf("cannot open file at: %s\n", path);
        return;
    }
    // 跳过前面的信息描述
    int info_len;
    fread(&info_len, sizeof(int), 1, f);
    fseek(f, info_len, SEEK_CUR);

    fread(c1[0].ptr, sizeof(float), 25 * 6, f);
    fread(c1_bias.ptr, sizeof(float), 6, f);

    fread(c2[0].ptr, sizeof(float), 16 * 6 * 5 * 5, f);
    fread(c2_bias.ptr, sizeof(float), 16, f);

    fread(f1.ptr, sizeof(float), f1.width * f1.height, f);
    fread(f1_bias.ptr, sizeof(float), f1_bias.width * f1_bias.height, f);

    fread(f2.ptr, sizeof(float), f2.width * f2.height, f);
    fread(f2_bias.ptr, sizeof(float), f2_bias.width * f2_bias.height, f);

    fread(f3.ptr, sizeof(float), f3.width * f3.height, f);
    fread(f3_bias.ptr, sizeof(float), f3_bias.width * f3_bias.height, f);

    fclose(f);
}

void bind_mat_arr(mat_float* arr, int width, int height, int count, float* cache)
{
    for (size_t i = 0; i < count; i++)
    {
        arr[i] = { width, height, cache };
        cache += width * height;
    }
}

int lenet::operator()(const mat_float& img)
{
    return eval(img);
}

int lenet::eval(const mat_float& img)
{
    // 中间变量占用的空间分别为 
    // 4704, 1176,
    // 1600, 400,
    // 120, 84,
    // 10
    // 用双缓冲模式可以使额外内存占用压缩到 4704 + 1176, 且仅调用两次分配和释放
    // 注意单位是 float
    float* cache1 = new float[4704];
    float* cache2 = new float[1176];
    mat_float arr1[16];
    mat_float arr2[16];
    mat_float m1;
    mat_float m2;

    bind_mat_arr(arr1, 28, 28, 6, cache1);
    // conv1
    batch_conv_layer(&img, c1.get(), c1_bias, 1, 6, arr1, 2);
    // print_arr(cache1, 10);

    // 由于池化层的移动窗口没有重叠, 所以6个频道可以同时算. 下面的池化同理
    m1 = { 28, 28 * 6, cache1 };
    m2 = { 14, 14 * 6, cache2 };
    pooling_max(m1, m2, 2);
    // print_arr(cache2 + 20, 10);

    bind_mat_arr(arr1, 10, 10, 16, cache1);
    bind_mat_arr(arr2, 14, 14, 6, cache2);
    // conv2
    batch_conv_layer(arr2, c2.get(), c2_bias, 6, 16, arr1, 0);
    // print_arr(cache1 + 1590, 10);

    m1 = { 10, 10 * 16, cache1 };
    m2 = { 5, 5 * 16, cache2 };
    pooling_max(m1, m2, 2);
    // print_arr(cache2 + 20, 10);

    m1 = { 1, 120, cache1 };
    m2 = { 1, 16 * 5 * 5, cache2 };
    // fc1
    full_connect_layer(m2, f1, f1_bias, m1);
    // print_arr(cache1 + 20, 10);

    m2 = { 1, 84, cache2 };
    // fc2
    full_connect_layer(m1, f2, f2_bias, m2);
    // print_arr(cache2, 84);

    m1 = { 1, 10, cache1 };
    // fc3
    full_connect_layer(m2, f3, f3_bias, m1);
    // print_arr(cache1, 10);

    // argmax
    auto max_id = 0;
    auto max = cache1[0];
    for (size_t i = 1; i < 10; i++)
    {
        if (cache1[i] > max)
        {
            max = cache1[i];
            max_id = i;
        }
    }

    delete[]cache1;
    delete[]cache2;

    return max_id;
}

void lenet::test(const char* test_file, const char* label_file, int max_test_count)
{
    // 不知道为什么上面用了C的库来进行文件IO
    // 反正都这样写了, 那就继续用吧:D
    // 把下面这两段python翻译一下就行辣
    /*
    def read_images(path: str, dtype:np.dtype=np.float32) -> np.ndarray:
    with open(path, mode='rb') as f:
        f.read(4)
        image_count = int.from_bytes(f.read(4))
        row = int.from_bytes(f.read(4))
        col = int.from_bytes(f.read(4))
        print('file contains {} images, size = ({},{})'.format(image_count, row, col))
        res = np.frombuffer(f.read(row*col*image_count), dtype=np.uint8)
        res = res.reshape(image_count, row, col)
        res = (res / 256).astype(dtype)
    return res
    */
    /*
    def read_labels(path: str) -> np.ndarray:
    with open(path, mode='rb') as f:
        f.read(4)
        size = int.from_bytes(f.read(4))
        temp = np.frombuffer(f.read(size),dtype=np.uint8)
        res = np.zeros((size,10),dtype=np.float32)
        res[np.arange(size),temp] = 1
    return res
    */

    FILE* f;
    // 读数据文件
    fopen_s(&f, test_file, "rb");
    if (f == NULL)
    {
        printf("cannot open file at: %s\n", test_file);
        return;
    }
    fseek(f, 4, SEEK_CUR);
    int img_count;
    int row, col;
    fread(&img_count, 4, 1, f);
    fread(&row, 4, 1, f);
    fread(&col, 4, 1, f);
    swap_bytes(&img_count);
    swap_bytes(&row);
    swap_bytes(&col);
    printf("file contains %d images, size = (%d,%d)\n", img_count, row, col);

    auto element_count = row * col * img_count;
    std::unique_ptr<float> cache_smart{ new float[element_count] };
    std::unique_ptr<mat_float> imgs_smart{ new mat_float[img_count] };
    
    // 注意输入的数据类型是uint8 所以还得手动转一下
    auto temp = new unsigned char[element_count];
    auto cache = cache_smart.get();
    fread(temp, 1, element_count, f);
    for (size_t i = 0; i < element_count; i++)
    {
        cache[i] = temp[i] / 256.0f;
    }
    bind_mat_arr(imgs_smart.get(), col, row, img_count, cache);
    delete[]temp;
    fclose(f);

    // 读标签文件
    fopen_s(&f, label_file, "rb");
    if (f == NULL)
    {
        printf("cannot open file at: %s\n", test_file);
        return;
    }
    // 跳过image count, 因为上面读了
    fseek(f, 8, SEEK_CUR);
    std::unique_ptr<char> label_cache_smart{ new char[img_count] };
    fread(label_cache_smart.get(), sizeof(char), img_count, f);
    fclose(f);

    // 开始干正事
    auto correct = 0;
    auto test_count = img_count > max_test_count ? max_test_count : img_count;
    for (size_t i = 0; i < test_count; i++)
    {
        auto img_mat = imgs_smart.get()[i];
        auto img_label = label_cache_smart.get()[i];
        auto predict = eval(img_mat);
        if (predict == img_label) correct++;
        else
        {
            // printf("predict: %d, should be %d\n", predict, img_label);
        }
        if (i % 100 == 99) printf("test %lld ~ %lld finished.\n", i - 99, i);
    }

    printf("test result: acc = %f%%\n", correct * 100.0f / test_count);
}

void lenet::batch_conv_layer(const mat_float* img_arr, const mat_float* kernel_arr, const mat_float& bias, int in, int out, const mat_float* dst_array, int padding)
{
    size_t kernel_id = 0;
    mat_float temp_mat{ dst_array[0].width, dst_array[0].height };
    allocate(temp_mat);
    for (size_t i_out = 0; i_out < out; i_out++)
    {
        auto out_mat = dst_array[i_out];
        fill(temp_mat, bias.ptr[i_out]);
        for (size_t i_in = 0; i_in < in; i_in++)
        {
            convolution(img_arr[i_in], out_mat, kernel_arr[kernel_id], padding);
            add(out_mat, temp_mat, temp_mat);
            kernel_id++;
        }
        relu(temp_mat, out_mat);
    }
    free(temp_mat.ptr);
}
