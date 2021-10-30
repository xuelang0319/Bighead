//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年5月30日   |   模型工具
//

using BigHead.Framework.Core;
using UnityEditor;
using UnityEngine;

namespace BigHead.Editor.Scripts
{
    public static class ModelTools
    {
        /// <summary>
        /// 获取模型的顶点数量和三角面数量
        /// </summary>
        public static void GetSelectionModelVertexesAndTriangularFacet()
        {
            var selections = Selection.gameObjects;
            if (selections.Length.Equals(0))
            {
                "请至少在场景中选择一个GameObject!".Error();
                return;
            }

            for (int i = 0; i < selections.Length; i++)
            {
                var item = selections[i];
                var filters = item.GetComponentsInChildren<MeshFilter>();
                if (filters.Length.Equals(0))
                {
                    $"当前选中的模型：{item.name} 中没有找到MeshFilter组件，请检查。".Error();
                    continue;
                }

                var vertex = 0;
                var triangular = 0;
                for (int j = 0; j < filters.Length; j++)
                {
                    var mesh = filters[j].sharedMesh;
                    vertex += mesh.vertexCount;
                    triangular += mesh.triangles.Length / 3;
                }
                
                $"当前选中的模型：{item.name} 中存在 {filters.Length} 个MeshFilter, 共计顶点数量为：{vertex}, 三角面数为：{triangular}".Highlight();
            }
        }
    }
}