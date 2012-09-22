// Widget.cs
//  
// Author:  WOnder93 <omosnacek@gmail.com>
// 
// Copyright (c) 2011 Ondrej Mosnáček
// 
// Created with the help of the source code of KBang (http://code.google.com/p/kbang)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using Cairo;

namespace BangSharp.Client.GameBoard.Widgets
{
	public class Widget
	{
		private class ChildrenCollection : IList<Widget>
		{
			private Widget parent;
			private int maxChildren;
			private List<Widget> children;

			public ChildrenCollection(Widget parent, int maxChildren)
			{
				this.parent = parent;
				this.maxChildren = maxChildren;
				children = new List<Widget>();
			}

			public int IndexOf(Widget item)
			{
				return children.IndexOf(item);
			}

			public void Insert(int index, Widget item)
			{
				if(Count == maxChildren)
					throw new InvalidOperationException();
				int already = children.IndexOf(item);
				if(already == -1)
				{
					children.Insert(index, item);
					item.Reparent(parent);
				}
				else if(already >= index)
				{
					children.RemoveAt(already);
					children.Insert(index, item);
				}
				else
				{
					children.Insert(index, item);
					children.RemoveAt(already);
				}
			}

			public void RemoveAt(int index)
			{
				children[index].Reparent(null);
				children.RemoveAt(index);
			}

			public Widget this[int index]
			{
				get { return children[index]; }
				set
				{
					int already = children.IndexOf(value);
					children[index].Reparent(null);
					children[index] = value;
					if(already >= 0 && already != index)
						children.RemoveAt(already);
					value.Reparent(parent);
				}
			}

			public IEnumerator<Widget> GetEnumerator()
			{
				return children.GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return children.GetEnumerator();
			}

			public void Add(Widget item)
			{
				if(Count == maxChildren)
					throw new InvalidOperationException();
				int already = children.IndexOf(item);
				if(already >= 0)
					children.RemoveAt(already);
				children.Add(item);
				item.Reparent(parent);
			}

			public void Clear()
			{
				foreach(Widget child in children.ToArray())
					child.Reparent(null);
			}

			public bool Contains(Widget item)
			{
				return children.Contains(item);
			}

			public void CopyTo(Widget[] array, int arrayIndex)
			{
				children.CopyTo(array, arrayIndex);
			}

			public bool Remove(Widget item)
			{
				if(children.Contains(item))
				{
					item.Reparent(null);
					children.Remove(item);
					return true;
				}
				return false;
			}
			public void RemoveQuick(Widget item)
			{
				children.Remove(item);
			}

			public int Count
			{
				get { return children.Count; }
			}

			public bool IsReadOnly
			{
				get { return false; }
			}
		}
		private Widget parent;
		private ChildrenCollection children;
		private Rectangle alloc;

		public Widget Parent
		{
			get { return parent; }
		}
		public IList<Widget> Children
		{
			get { return children; }
		}
		public Rectangle Allocation
		{
			get { return alloc; }
		}

		protected Widget()
		{
			children = new ChildrenCollection(this, int.MaxValue);
		}
		protected Widget(int maxChildren)
		{
			children = new ChildrenCollection(this, maxChildren);
		}

		public static Rectangle Intersect(Rectangle a, Rectangle b)
		{
			double x = Math.Max(a.X, b.X);
			double y = Math.Max(a.Y, b.Y);
			double w = Math.Min(a.X + a.Width, b.X + b.Width) - x;
			double h = Math.Min(a.Y + a.Height, b.Y + b.Height) - y;
			return new Rectangle(x, y, Math.Max(0.0, w), Math.Max(0.0, h));
		}
		public static bool Contains(Rectangle r, double x, double y)
		{
			double dx = x - r.X;
			double dy = y - r.Y;
			if(dx < 0 || dx > r.Width)
				return false;
			if(dy < 0 || dy > r.Height)
				return false;
			return true;
		}

		protected void Expose(Context cr, Rectangle area)
		{
			if(area.Width == 0.0 || area.Height == 0.0)
				return;
			cr.Save();
			OnExposed(cr, area);
			if(cr.Status != Status.Success)
			{
				Console.Error.WriteLine("ERROR: cr.Status == {0}!", cr.Status);
				throw new Exception();
			}
			cr.Restore();
			foreach(Widget child in children)
			{
				cr.Save();
				cr.Rectangle(child.alloc);
				cr.Clip();
				cr.Translate(child.alloc.X, child.alloc.Y);
				Rectangle childArea = Intersect(area, child.alloc);
				childArea = new Rectangle(childArea.X - child.alloc.X, childArea.Y - child.alloc.Y,
					childArea.Width, childArea.Height);
				child.Expose(cr, childArea);
				cr.Restore();
			}
		}
		public void Reallocate(Rectangle newAlloc)
		{
			alloc = newAlloc;
			OnResized();
		}
		protected bool LeftClick(double x, double y)
		{
			for(int i = children.Count - 1; i >= 0; i--)
			{
				Widget child = children[i];
				if(Contains(child.alloc, x, y) && child.LeftClick(x - child.alloc.X, y - child.alloc.Y))
					return true;
			}
			return OnLeftClick(x, y);
		}
		protected bool RightClick(double x, double y)
		{
			for(int i = children.Count - 1; i >= 0; i--)
			{
				Widget child = children[i];
				if(Contains(child.alloc, x, y) && child.RightClick(x - child.alloc.X, y - child.alloc.Y))
					return true;
			}
			return OnRightClick(x, y);
		}

		protected virtual bool OnExposed(Context cr, Rectangle area)
		{
			return false;
		}
		protected virtual void OnResized()
		{
		}
		protected virtual bool OnLeftClick(double x, double y)
		{
			return false;
		}
		protected virtual bool OnRightClick(double x, double y)
		{
			return false;
		}

		public virtual void RequestResize()
		{
			if(parent != null)
				parent.RequestResize();
		}
		public virtual void RequestRedraw()
		{
			if(parent != null)
				parent.RequestRedraw();
		}

		public virtual void SizeRequest(ref double width, ref double height, out double ratio)
		{
			width = -1.0;
			height = -1.0;
			ratio = -1.0;
		}

		private Rectangle GetAbsoluteAllocation(Rectangle rel)
		{
			Rectangle res = new Rectangle(alloc.X + rel.X, alloc.Y + rel.Y, rel.Width, rel.Height);
			if(parent != null)
				return parent.GetAbsoluteAllocation(res);
			else
				return res;
		}
		public Rectangle GetAbsoluteAllocation()
		{
			if(parent != null)
				return parent.GetAbsoluteAllocation(alloc);
			else
				return alloc;
		}

		private void Reparent(Widget parent)
		{
			if(this.parent != null)
			{
				this.parent.children.RemoveQuick(this);
				//this.parent.RequestResize();
			}
			this.parent = parent;
			//RequestResize();
		}
	}
}

