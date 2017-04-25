﻿/*-----------------------------------------------------------------------------
The MIT License (MIT)

This source file is part of GDGeek
    (Game Develop & Game Engine Extendable Kits)
For the latest info, see http://gdgeek.com/

Copyright (c) 2014-2015 GDGeek Software Ltd

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
-----------------------------------------------------------------------------
*/
using UnityEngine;

/// <summary>
/// Tween the object's position.
/// </summary>
namespace GDGeek{
	public class TweenWorldPosition : Tween
	{
		public Vector3 from;
		public Vector3 to;
		
		Transform mTrans;
		
		public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }
		public Vector3 position { get { return cachedTransform.position; } set { cachedTransform.position = value; } }
		
		override protected void OnUpdate (float factor, bool isFinished) { cachedTransform.position = from * (1f - factor) + to * factor; }
		
		/// <summary>
		/// Start the tweening operation.
		/// </summary>
		
		static public TweenWorldPosition Begin (GameObject go, float duration, Vector3 pos)
		{
			TweenWorldPosition comp = Tween.Begin<TweenWorldPosition>(go, duration);
			comp.from = comp.position;
			comp.to = pos;
			
			if (duration <= 0f)
			{
				comp.Sample(1f, true);
				comp.enabled = false;
			}
			return comp;
		}
	}
}