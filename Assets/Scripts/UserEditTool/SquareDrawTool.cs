using InterNeuralNet.NetworkView;
using UnityEngine;

namespace InterNeuralNet.UserEditTool
{
    public class SquareDrawTool : MonoBehaviour, ITool
    {
        [Min(0)] public int radius = 3;

        public void OperateAt(MatView view, int matId, int x, int y, bool invertBehavior)
        {
            var minX = Mathf.Max(0, x - radius);
            var minY = Mathf.Max(0, y - radius);
            var maxX = Mathf.Min(view.Shape.width, x + radius);
            var maxY = Mathf.Min(view.Shape.height, y + radius);
            float value = invertBehavior ? 1 : 0;
            if (view is not MatWritableView wView) return;
            view.EnableAccess();
            for (int r = minY; r < maxY; r++)
            {
                for (int c = minX; c < maxX; c++)
                {
                    wView.WriteAt(matId, r, c, value);
                }
            }
        }
    }
}
