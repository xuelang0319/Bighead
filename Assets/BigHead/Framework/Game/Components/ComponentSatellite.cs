using System;
using System.Collections.Generic;
using System.Linq;
using BigHead.Framework.Core;
using BigHead.Framework.Extension;
using BigHead.Framework.Utility.Helper;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BigHead.Framework.Game.Components
{
    public class ComponentSatellite
    {
        /// <summary>
        /// 卫星集合
        /// </summary>
        private readonly Dictionary<string, SatelliteParent> _satelliteElements =
            new Dictionary<string, SatelliteParent>();

        /// <summary>
        /// 母星位移组件
        /// </summary>
        private readonly Transform _transform;
        
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="transform">母星位移组件</param>
        public ComponentSatellite(Transform transform)
        {
            _transform = transform;
        }

        /// <summary>
        /// 添加卫星元素
        /// </summary>
        /// <param name="type">卫星类型</param>
        /// <param name="transform">卫星位移组件</param>
        /// <param name="speed">卫星位移速度</param>
        /// <param name="radius">卫星半径</param>
        /// <param name="forceChangeSpeedAndRadius">如果卫星父节点存在，强行使用速度与半径参数修改父节点参数</param>
        public int AddSatellite(string type, Transform transform, float speed, float radius,
            bool forceChangeSpeedAndRadius = false)
        {
            if (!_satelliteElements.TryGetValue(type, out var parent))
            {
                parent = new SatelliteParent(_transform, type, speed, radius);
                _satelliteElements.Add(type, parent);
            }
            else if (forceChangeSpeedAndRadius)
            {
                if (Math.Abs(parent.Radius - radius) > 0.001f) parent.SetRadius(radius);
                if (Math.Abs(parent.Speed - speed) > 0.001f) parent.SetRadius(speed);
            }

            return parent.AddSatellite(transform);
        }

        /// <summary>
        /// 获取指定卫星类型的数量
        /// </summary>
        /// <param name="type">卫星类型</param>
        /// <returns>该类型的卫星数量</returns>
        public int GetSatelliteCount(string type)
        {
            return _satelliteElements.ContainsKey(type) ? _satelliteElements[type].Count : 0;
        }

        /// <summary>
        /// 移除卫星元素
        /// </summary>
        /// <param name="type">卫星类型</param>
        /// <param name="id">注册时返回的唯一ID</param>
        /// <param name="callback">移除后对该元素的处理</param>
        public void RemoveSatellite(string type, int id, Action<Transform> callback)
        {
            if (!_satelliteElements.TryGetValue(type, out var parent))
            {
                $"Can't find type - {type} of satellite parent.".Error();
                return;
            }

            parent.RemoveSatellite(id, callback);
        }

        /// <summary>
        /// 移除卫星元素 - 直接删除卫星
        /// </summary>
        /// <param name="type">卫星类型</param>
        /// <param name="id">注册时返回的唯一ID</param>
        public void RemoveSatellite(string type, int id)
        {
            if (!_satelliteElements.TryGetValue(type, out var parent))
            {
                $"Can't find type - {type} of satellite parent.".Error();
                return;
            }

            parent.RemoveSatellite(id, t => Object.Destroy(t.gameObject));
        }

        /// <summary>
        /// 反初始化方法
        /// </summary>
        public void OnDestroy()
        {
            if (_satelliteElements.Count == 0)
                return;

            var array = _satelliteElements.Keys.ToArray();
            for (int i = 0; i < array.Length; i++)
                _satelliteElements[array[i]].OnDestroy();

            _satelliteElements.Clear();
        }

        /// <summary>
        /// 卫星父节点
        /// </summary>
        private class SatelliteParent
        {
            /// <summary> 旋转节点 </summary>
            private readonly Transform _rotateNode;

            /// <summary> 当前旋转速度 </summary>
            public float Speed { get; private set; }

            /// <summary> 卫星旋转半径 </summary>
            public float Radius { get; private set; }

            /// <summary> 卫星类型 </summary>
            private readonly string _type;

            /// <summary> 卫星元素集合 </summary>
            private readonly Dictionary<int, SatelliteElement> _satelliteElements =
                new Dictionary<int, SatelliteElement>();

            /// <summary> 独立编号 </summary>
            private readonly SingleId _singleId = SingleId.New();

            /// <summary> 卫星数量 </summary>
            public int Count => _satelliteElements.Count;

            /// <summary>
            /// 构造方法
            /// </summary>
            /// <param name="parent">实例单位</param>
            /// <param name="type">卫星类型</param>
            /// <param name="speed">旋转速度</param>
            /// <param name="radius">半径</param>
            public SatelliteParent(Transform parent, string type, float speed, float radius)
            {
                _type = type;
                _rotateNode = new GameObject(type).transform;
                _rotateNode.SetParent(parent);
                _rotateNode.Reset();
                Speed = speed;
                Radius = radius;
                BigHeadManager.Instance.UpdateEvent += Update;
            }

            /// <summary>
            /// 反初始化方法
            /// </summary>
            public void OnDestroy()
            {
                BigHeadManager.Instance.UpdateEvent -= Update;
                Object.Destroy(_rotateNode.gameObject);
            }

            /// <summary>
            /// 设置速度
            /// </summary>
            /// <param name="speed">速度值</param>
            public void SetSpeed(float speed)
            {
                Speed = speed;
                UpdateChildren();
            }

            /// <summary>
            /// 设置半径
            /// </summary>
            /// <param name="radius">半径</param>
            public void SetRadius(float radius)
            {
                Radius = radius;
                UpdateChildren();
            }

            /// <summary>
            /// 添加卫星元素
            /// </summary>
            /// <param name="transform">卫星位移组件</param>
            public int AddSatellite(Transform transform)
            {
                var singleId = _singleId.Get();
                var element = new SatelliteElement(transform);
                _satelliteElements.Add(singleId, element);
                transform.SetParent(_rotateNode);
                UpdateChildren();
                return singleId;
            }

            /// <summary>
            /// 移除卫星元素
            /// </summary>
            /// <param name="id">卫星添加时的唯一ID</param>
            /// <param name="callback">对移除卫星的后续操作</param>
            public void RemoveSatellite(int id, Action<Transform> callback)
            {
                if (_satelliteElements.TryGetValue(id, out var satellite))
                {
                    _satelliteElements.Remove(id);
                    callback?.Invoke(satellite.Transform);
                    UpdateChildren();
                }
            }

            /// <summary>
            /// 更新环绕节点
            /// </summary>
            private void UpdateChildren()
            {
                var keys = _satelliteElements.Keys.ToArray();
                var count = keys.Length;
                for (int i = 0; i < count; i++)
                {
                    var angle = 360 / count * i;
                    var key = keys[i];
                    _satelliteElements[key].SetPosition(angle, Radius);
                }
            }

            /// <summary>
            /// 帧调用
            /// </summary>
            /// <param name="deltaTime">帧时间</param>
            private void Update(float deltaTime)
            {
                _rotateNode.localRotation =
                    Quaternion.Euler(0, _rotateNode.localEulerAngles.y + Speed * 60 * deltaTime, 0);
            }
        }

        /// <summary>
        /// 卫星元素
        /// </summary>
        private class SatelliteElement
        {
            /// <summary> 卫星元素节点 </summary>
            public readonly Transform Transform;

            /// <summary>
            /// 构造方法
            /// </summary>
            /// <param name="transform">卫星父节点</param>
            /* 2021.10.18 Eric 卫星预制体原始结构为父节点 - 名称为数字的0，1的子节点，一般节点数量为2，即：0,1
               遵循原始结构，则所有的卫星预制体均为两个子节点，并与策划约定均为2个子节点 */
            public SatelliteElement(Transform transform)
            {
                Transform = transform;
            }

            /// <summary>
            /// 设置位置
            /// </summary>
            /// <param name="angle">角度</param>
            /// <param name="radius">半径</param>
            public void SetPosition(float angle, float radius)
            {
                Transform.localPosition = GameMathf.GetLocalPositionByAngleAndRadius_3D(angle, radius);
                Transform.localRotation = Quaternion.Euler(0, radius, 0);
            }
        }
    }
}