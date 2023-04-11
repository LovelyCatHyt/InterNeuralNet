using System;

namespace InterNeuralNet.CoreWrapper
{
    /// <summary>
    /// 可通用于不同类型的层的数据描述.
    /// <para>
    /// 在池化层, 全连接层中, 输入与输出的<see cref="count"/>都为1.
    /// </para>
    /// <para>
    /// 而在卷积层中, 输入与输出的<see cref="count"/>取决于对应的频道数.
    /// </para>
    /// </summary>
    public struct Mat_Shape
    {
        public int width;
        public int height;
        public int count;

        public Mat_Shape(int width, int height, int count)
        {
            this.width = width;
            this.height = height;
            this.count = count;
        }

        public override string ToString() => $"w:{width}, h:{height}, c:{count}";
    }

    public abstract class Layer
    {
        public string name;    

        public Layer prev;
        public Layer next;

        public int inChannel;
        public int outChannel;
        /// <summary>
        /// 输入矩阵(数组), 长度等于 <see cref="inChannel"/>
        /// </summary>
        public Mat_Float[] input;
        /// <summary>
        /// 输出矩阵(数组), 长度等于 <see cref="outChannel"/>
        /// </summary>
        public Mat_Float[] output;

        public abstract void Eval(Mat_Float[] input, Mat_Float[] output);

        public void Eval()
        {
            Eval(input, output);
            if (next != null)
            {
                next.Eval();
            }
        }

        public Layer Append(Layer next)
        {
            this.next = next;
            next.prev = this;
            return next;
        }

        public virtual void ApplyParams(Mat_Float[] parameters)
        {
            // 懒得写无参数的代码 所以在基类写 virtual
            // 但是如果有参数 那就必须继承 所以直接抛异常
            if (parameters.Length > 0) throw new NotImplementedException();
        }

        /// <summary>
        /// 获取这一运算层的所有参数的 shape
        /// </summary>
        /// <returns></returns>
        public abstract Mat_Shape[] GetParamShapes();

        public abstract Mat_Shape CalcOutputShape(Mat_Shape shape);
    }
}
