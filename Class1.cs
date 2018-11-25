using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{
    /// <summary>
    /// 线段树
    /// </summary>
    /// <typeparam name="T">线段树内的元素类型</typeparam>
    public class SegmentTree<T> where T : struct
    {
        #region private
        private int _left;
        private int _right;
        private T _value;
        private T initValue;
        private MergeMethod merge;
        private AddMethod add;
        private MultiplyMethod multiply;
        private T lazy;
        private bool lazied;
        private SegmentTree<T> leftSon, rightSon;
        private void pushDown()
        {
            if (!lazied) return;
            if (_left == _right) return;
            lazied = false;
            leftSon._value = add(leftSon._value, multiply(lazy, leftSon.Size));
            rightSon._value = add(rightSon._value, multiply(lazy, rightSon.Size));
            leftSon.lazy = add(leftSon.lazy, lazy);
            rightSon.lazy = add(rightSon.lazy, lazy);
            lazy = initValue;
            leftSon.lazied = rightSon.lazied = true;
        }
        private void build(T[] elements)
        {
            if (_left == _right)
            {
                _value = elements[_left];
                leftSon = rightSon = null;
                return;
            }
            int mid = (_left + _right) >> 1;
            leftSon = new SegmentTree<T>(elements, _left, mid, merge, add, multiply, initValue);
            rightSon = new SegmentTree<T>(elements, mid + 1, _right, merge, add, multiply, initValue);
            _value = merge(leftSon._value, rightSon._value);
        }
        #endregion
        /// <summary>
        /// 区间值合并委托类型
        /// </summary>
        /// <param name="x">左区间的值</param>
        /// <param name="y">右区间的值</param>
        /// <returns>合并结果</returns>
        public delegate T MergeMethod(T x, T y);
        /// <summary>
        /// 区间修改方法的委托类型
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <returns>修改结果</returns>
        public delegate T AddMethod(T x, T y);
        /// <summary>
        /// 区间修改累和委托
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="val">val</param>
        /// <returns>返回结果</returns>
        public delegate T MultiplyMethod(T x, int val);
        /// <summary>
        /// 获取线段树锁维护区间的左端点
        /// </summary>
        public int Left
        {
            get
            {
                return _left;
            }
        }
        /// <summary>
        /// 获取线段树锁维护区间的右端点
        /// </summary>
        public int Right
        {
            get
            {
                return _right;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="elements">线段树锁维护的元素数组</param>
        /// <param name="left">维护的区间的左端点</param>
        /// <param name="right">维护的区间的右端点</param>
        /// <param name="mergeMethod">区间合并方法</param>
        /// <param name="addMethod">区间修改方法</param>
        /// <param name="mulMethod">区间修改累和方法</param>
        /// <param name="initVal">初始值(默认为T的初始值)</param>
        public SegmentTree(T[] elements, int left, int right,
            MergeMethod mergeMethod, AddMethod addMethod, MultiplyMethod mulMethod, T initVal = default(T))
        {
            if (right < left)
                throw new ArgumentOutOfRangeException("right", "区间right<left");
            if (left < 0)
                throw new ArgumentOutOfRangeException("left", "left越界");
            if (right > elements.GetUpperBound(0))
                throw new ArgumentOutOfRangeException("right", "right越界");
            lazied = false;
            _left = left;
            _right = right;
            merge = mergeMethod;
            add = addMethod;
            multiply = mulMethod;
            lazy = initValue = initVal;
            build(elements);
        }
        /// <summary>
        /// 获取线段树维护的区间的大小
        /// </summary>
        public int Size
        {
            get
            {
                return _right - _left + 1;
            }
        }
        /// <summary>
        /// 区间修改
        /// </summary>
        /// <param name="left">区间左端点</param>
        /// <param name="right">区间右端点</param>
        /// <param name="val">区间修改参数</param>
        public void Update(int left, int right, T val)
        {
            if (right < left)
                throw new ArgumentOutOfRangeException("right", "线段树修改区间right<left");
            pushDown();
            if (left <= _left && right >= _right)
            {
                _value = add(_value, multiply(val, Size));
                lazy = add(lazy, val);
                lazied = true;
                return;
            }
            int mid = (_left + _right) >> 1;
            if (left <= mid) leftSon.Update(left, right, val);
            if (right >= mid + 1) rightSon.Update(left, right, val);
            _value = merge(leftSon._value, rightSon._value);
        }
        /// <summary>
        /// 区间查询
        /// </summary>
        /// <param name="left">区间左端点</param>
        /// <param name="right">区间右端点</param>
        /// <returns>查询结果</returns>
        public T Query(int left, int right)
        {
            if (right < left)
                throw new ArgumentOutOfRangeException("right", "线段树查询区间right<left");
            pushDown();
            if (left <= _left && right >= _right)
                return _value;
            int mid = (_left + _right) >> 1;
            if (left <= mid && right >= mid + 1)
                return merge(leftSon.Query(left, right), rightSon.Query(left, right));
            else if (left <= mid) return leftSon.Query(left, right);
            else return rightSon.Query(left, right);
        }
    }
}
