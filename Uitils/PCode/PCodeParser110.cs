using System.Runtime.CompilerServices;
using PbdViewer.Uitils.PbClass;

namespace PbdViewer.Uitils.PCode
{
	internal class PCodeParser110 : PCodeParser105
	{
		[CompilerGenerated]
		private readonly byte[] _003CPCodeLenArray_003Ek__BackingField = new byte[583]
		{
			0, 1, 1, 1, 1, 0, 0, 0, 0, 0,
			1, 3, 3, 1, 3, 3, 4, 0, 1, 5,
			0, 3, 0, 3, 3, 0, 4, 3, 5, 4,
			1, 1, 2, 0, 0, 0, 0, 0, 0, 1,
			0, 3, 2, 3, 4, 2, 3, 1, 1, 1,
			1, 1, 2, 2, 2, 2, 2, 2, 2, 2,
			1, 2, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 2, 1, 2, 1, 0, 0, 0, 0,
			0, 0, 0, 2, 0, 2, 2, 2, 2, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 2,
			0, 2, 2, 2, 2, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 2, 2, 2, 2, 0,
			0, 0, 0, 0, 0, 0, 0, 2, 2, 2,
			2, 0, 0, 0, 0, 0, 0, 0, 0, 2,
			2, 2, 2, 0, 0, 0, 0, 0, 0, 0,
			0, 2, 2, 2, 2, 0, 2, 2, 2, 2,
			2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
			2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
			2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
			2, 1, 0, 0, 0, 1, 1, 1, 1, 1,
			1, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 0,
			0, 1, 1, 0, 0, 0, 0, 3, 3, 2,
			2, 3, 3, 4, 4, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 2, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 2, 2, 1, 1, 2, 3, 2, 3, 4,
			1, 1, 1, 1, 1, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			1, 0, 0, 0, 0, 0, 1, 2, 0, 0,
			0, 0, 0, 0, 1, 1, 1, 1, 0, 1,
			1, 0, 0, 1, 0, 0, 0, 0, 0, 0,
			1, 0, 1, 1, 1, 1, 1, 5, 1, 4,
			1, 0, 2, 3, 3, 5, 3, 5, 1, 4,
			2, 2, 2, 2, 2, 3, 3, 3, 3, 0,
			3, 0, 2, 2, 2, 3, 1, 1, 4, 3,
			1, 1, 1, 0, 0, 2, 1, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 2, 0, 0, 0, 1,
			0, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 0, 0, 0, 0, 0,
			0, 2, 1, 1, 1, 1, 1, 1, 1, 2,
			2, 2, 2, 2, 1, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 3, 2, 3, 4,
			3, 1, 1, 0, 1, 1, 0, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 0, 0, 0, 0, 0,
			0, 2, 2, 1, 2, 2, 2, 0, 0, 0,
			0, 0, 0
		};

		protected override byte[] PCodeLenArray
		{
			[CompilerGenerated]
			get
			{
				return _003CPCodeLenArray_003Ek__BackingField;
			}
		}

		protected override bool OnParsePcode(int pCodeOp, CodeLine codeLine)
		{
			if (pCodeOp <= 408)
			{
				return base.OnParsePcode(pCodeOp, codeLine);
			}
			if (pCodeOp <= 416)
			{
				return base.OnParsePcode(pCodeOp + 1, codeLine);
			}
			if (pCodeOp <= 419)
			{
				return base.OnParsePcode(pCodeOp + 2, codeLine);
			}
			return base.OnParsePcode(pCodeOp + 3, codeLine);
		}

		public PCodeParser110(PbFunction pbFunction)
			: base(pbFunction)
		{
		}
	}
}
