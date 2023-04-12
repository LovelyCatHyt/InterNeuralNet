using InterNeuralNet.NetworkView;

namespace InterNeuralNet.UserEditTool
{
    public interface ITool
    {
        /// <summary>
        /// 在指定坐标上操作
        /// </summary>
        /// <param name="view"></param>
        /// <param name="x">像素x坐标</param>
        /// <param name="y">像素y坐标</param>
        /// <param name="invertBehavior">反转行为模式: 绘制和擦除的互相转换</param>
        public void OperateAt(MatView view, int matId, int x, int y, bool invertBehavior);
    }
}
