using System;

namespace AudioSystem
{
	[Serializable]
	public struct RangedFloat
	{
		public float MinValue;
		public float MaxValue;

		public RangedFloat(float minValue, float maxValue)
		{
			MinValue = minValue;
			MaxValue = maxValue;
		}
	}
}
