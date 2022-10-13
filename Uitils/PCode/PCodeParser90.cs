using System.Runtime.CompilerServices;
using PbdViewer.Uitils.PbClass;

namespace PbdViewer.Uitils.PCode
{
	internal class PCodeParser90 : PCodeParserBase
	{
		[CompilerGenerated]
		private readonly byte[] _003CPCodeLenArray_003Ek__BackingField = new byte[547]
		{
			2, 1, 1, 1, 0, 0, 0, 0, 0, 1,
			3, 3, 1, 3, 3, 4, 0, 1, 5, 0,
			3, 0, 3, 3, 0, 4, 3, 1, 1, 2,
			0, 0, 0, 0, 0, 0, 1, 0, 3, 2,
			3, 4, 2, 3, 1, 1, 1, 1, 1, 2,
			2, 2, 2, 2, 2, 2, 2, 1, 2, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 2,
			1, 2, 1, 0, 0, 0, 0, 0, 0, 0,
			2, 0, 2, 2, 2, 2, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 2, 0, 2, 2,
			2, 2, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 2, 2, 2, 2, 0, 0, 0, 0,
			0, 0, 0, 0, 2, 2, 2, 2, 0, 0,
			0, 0, 0, 0, 0, 0, 2, 2, 2, 2,
			0, 0, 0, 0, 0, 0, 0, 0, 2, 2,
			2, 2, 0, 2, 2, 2, 2, 2, 2, 2,
			2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
			2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
			2, 2, 2, 2, 2, 2, 2, 2, 1, 0,
			0, 0, 1, 1, 1, 1, 1, 1, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 0, 0, 1, 1,
			0, 0, 0, 0, 3, 3, 2, 2, 3, 3,
			4, 4, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 2, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 2, 2,
			1, 1, 2, 3, 2, 3, 4, 1, 1, 1,
			1, 1, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 1, 0, 0,
			0, 0, 0, 1, 1, 0, 0, 0, 0, 0,
			0, 1, 0, 1, 1, 0, 1, 1, 1, 0,
			0, 1, 0, 0, 0, 1, 0, 0, 0, 1,
			1, 0, 1, 1, 1, 1, 1, 5, 1, 4,
			1, 0, 2, 3, 3, 5, 3, 5, 1, 4,
			1, 1, 2, 2, 2, 3, 3, 3, 3, 0,
			3, 0, 2, 2, 2, 3, 1, 1, 4, 3,
			1, 1, 1, 0, 0, 2, 1, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 2, 0, 0, 0, 1,
			0, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 0, 0, 0, 0, 0,
			0, 2, 1, 1, 1, 1, 1, 1, 1, 2,
			2, 2, 2, 2, 1, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 3, 2, 3, 4,
			3, 1, 1, 0, 1, 1, 0
		};

		protected override byte[] PCodeLenArray
		{
			[CompilerGenerated]
			get
			{
				return _003CPCodeLenArray_003Ek__BackingField;
			}
		}

		public PCodeParser90(PbFunction pbFunction)
			: base(pbFunction)
		{
		}

		protected override byte OnGetPCodeLen(ushort pcode)
		{
			if (PbFunction.Project.Version < 193 && pcode == 297)
			{
				return 0;
			}
			return base.OnGetPCodeLen(pcode);
		}

		protected override bool OnParsePcode(int pCodeOp, CodeLine codeLine)
		{
			switch (pCodeOp)
			{
			case 0:
				Return(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 1:
				Jump(BufferHelper.GetUShort(codeLine.PCodeParam, 0L), JmpType.JmpIfTrue);
				break;
			case 2:
				Jump(BufferHelper.GetUShort(codeLine.PCodeParam, 0L), JmpType.JmpIfFalse);
				break;
			case 3:
				Jump(BufferHelper.GetUShort(codeLine.PCodeParam, 0L), JmpType.Jmp);
				break;
			case 4:
				SqlOperateTransaction("connect");
				break;
			case 5:
				SqlOperateTransaction("commit");
				break;
			case 6:
				SqlOperateTransaction("rollback");
				break;
			case 7:
				SqlOperateTransaction("disconnect");
				break;
			case 8:
				SqlClose();
				break;
			case 9:
				SqlOpen(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 10:
				SqlDirectInsertUpdateDelete(BufferHelper.GetUInt(codeLine.PCodeParam, 0L), BufferHelper.GetUShort(codeLine.PCodeParam, 4L));
				break;
			case 11:
				SqlDirectInsertUpdateDelete(BufferHelper.GetUInt(codeLine.PCodeParam, 0L), BufferHelper.GetUShort(codeLine.PCodeParam, 4L));
				break;
			case 12:
				SqlExecute(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 13:
				SqlFetch(BufferHelper.GetUShort(codeLine.PCodeParam, 4L));
				break;
			case 14:
				SqlDirectInsertUpdateDelete(BufferHelper.GetUInt(codeLine.PCodeParam, 0L), BufferHelper.GetUShort(codeLine.PCodeParam, 4L));
				break;
			case 15:
				SqlDirectSelect(BufferHelper.GetUInt(codeLine.PCodeParam, 0L), BufferHelper.GetUShort(codeLine.PCodeParam, 4L), BufferHelper.GetUShort(codeLine.PCodeParam, 6L));
				break;
			case 16:
				DestroyObject();
				break;
			case 17:
				Halt(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 18:
				CallSuper(BufferHelper.GetUShort(codeLine.PCodeParam, 0L), BufferHelper.GetUShort(codeLine.PCodeParam, 2L), BufferHelper.GetUShort(codeLine.PCodeParam, 4L), BufferHelper.GetUInt(codeLine.PCodeParam, 6L));
				break;
			case 19:
				PopFunction();
				break;
			case 20:
				SqlExecuteSqlsa(BufferHelper.GetUShort(codeLine.PCodeParam, 4L));
				break;
			case 21:
				SqlPrepareSqlsa();
				break;
			case 22:
				SqlOpenDynamic(BufferHelper.GetUInt(codeLine.PCodeParam, 0L), BufferHelper.GetUShort(codeLine.PCodeParam, 4L));
				break;
			case 23:
				SqlExecuteDynamic(BufferHelper.GetUInt(codeLine.PCodeParam, 0L), BufferHelper.GetUShort(codeLine.PCodeParam, 4L));
				break;
			case 24:
				SqlDescribe();
				break;
			case 25:
				SqlDirectSelect(BufferHelper.GetUInt(codeLine.PCodeParam, 0L), BufferHelper.GetUShort(codeLine.PCodeParam, 4L), BufferHelper.GetUShort(codeLine.PCodeParam, 6L));
				break;
			case 26:
				SqlDirectInsertUpdateDelete(BufferHelper.GetUInt(codeLine.PCodeParam, 0L), (ushort)(BufferHelper.GetUShort(codeLine.PCodeParam, 4L) + 1));
				break;
			case 27:
				PushLocalVariable(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 28:
				PushSharedVariable(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 29:
				PushInstanceVariableName(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 30:
				PushThis();
				break;
			case 31:
				PushParent();
				break;
			case 33:
				OperateStack("and");
				break;
			case 34:
				OperateStack("or");
				break;
			case 35:
				OperateStackSingle("not");
				break;
			case 36:
				PushInstanceVariable(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 41:
				CallFunction(BufferHelper.GetUInt(codeLine.PCodeParam, 0L), BufferHelper.GetUShort(codeLine.PCodeParam, 4L), BufferHelper.GetUShort(codeLine.PCodeParam, 6L));
				break;
			case 42:
				CreateObject(BufferHelper.GetUInt(codeLine.PCodeParam, 0L));
				break;
			case 44:
				PushGlobalVariable(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 45:
				PushLocalVariable(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 46:
				PushGlobalSharedVariable(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 47:
				PushConstant(((short)BufferHelper.GetUShort(codeLine.PCodeParam, 0L)).ToString());
				break;
			case 48:
				PushConstant(BufferHelper.GetUShort(codeLine.PCodeParam, 0L).ToString());
				break;
			case 49:
				PushConstant(((int)BufferHelper.GetUInt(codeLine.PCodeParam, 0L)).ToString());
				break;
			case 50:
				PushConstant(BufferHelper.GetUInt(codeLine.PCodeParam, 0L).ToString());
				break;
			case 51:
				PushConstant(BufferHelper.GetDecimal(PbFunction.Buffer, BufferHelper.GetUInt(codeLine.PCodeParam, 0L)));
				break;
			case 52:
				PushConstant(BufferHelper.GetReal(BufferHelper.GetUInt(codeLine.PCodeParam, 0L)));
				break;
			case 53:
				PushConstant(BufferHelper.GetDouble(PbFunction.Buffer, BufferHelper.GetUInt(codeLine.PCodeParam, 0L)));
				break;
			case 54:
				PushConstant(BufferHelper.GetTime(PbFunction.Buffer, BufferHelper.GetUInt(codeLine.PCodeParam, 0L)));
				break;
			case 55:
				PushConstant(BufferHelper.GetDate(PbFunction.Buffer, BufferHelper.GetUInt(codeLine.PCodeParam, 0L)));
				break;
			case 56:
				PushConstant(BufferHelper.GetEscapeString(PbFunction.Project.IsUnicode, PbFunction.Buffer, BufferHelper.GetUInt(codeLine.PCodeParam, 0L)));
				break;
			case 57:
				PushConstant((BufferHelper.GetUShort(codeLine.PCodeParam, 0L) == 1).ToString().ToLower());
				break;
			case 58:
				PushEnum(BufferHelper.GetUShort(codeLine.PCodeParam, 2L), BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 59:
			case 60:
			case 61:
			case 62:
			case 63:
			case 64:
			case 65:
			case 66:
			case 67:
			case 68:
			case 69:
			case 70:
			case 71:
			case 72:
			case 73:
			case 74:
			case 75:
			case 76:
			case 77:
			case 78:
			case 79:
				Cast(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 80:
			case 81:
			case 82:
			case 83:
			case 84:
			case 85:
			case 86:
				OperateStack("+");
				break;
			case 87:
			case 88:
			case 89:
			case 90:
			case 91:
			case 92:
			case 93:
				OperateStack("-");
				break;
			case 94:
			case 95:
			case 96:
			case 97:
			case 98:
			case 99:
			case 100:
				OperateStack("*");
				break;
			case 101:
			case 102:
			case 103:
			case 104:
			case 105:
			case 106:
			case 107:
				OperateStack("/");
				break;
			case 108:
			case 109:
			case 110:
			case 111:
			case 112:
			case 113:
			case 114:
				OperateStack("^");
				break;
			case 115:
			case 116:
			case 117:
			case 118:
			case 119:
			case 120:
			case 121:
				OperateStackSingle("-");
				break;
			case 122:
			case 123:
				OperateStack("+");
				break;
			case 124:
				EndAssign(true);
				break;
			case 125:
			case 126:
			case 127:
			case 128:
			case 129:
			case 130:
			case 131:
			case 132:
			case 133:
			case 134:
			case 135:
			case 136:
			case 137:
				EndAssign();
				break;
			case 138:
			case 139:
			case 140:
			case 141:
			case 142:
			case 143:
			case 144:
			case 145:
			case 146:
			case 147:
			case 148:
			case 149:
			case 150:
			case 151:
			case 152:
			case 153:
			case 154:
			case 155:
			case 156:
			case 157:
			case 158:
			case 159:
			case 160:
			case 161:
			case 162:
				Cast(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 163:
			case 164:
			case 165:
			case 166:
			case 167:
			case 168:
			case 169:
			case 170:
			case 171:
			case 172:
			case 173:
			case 174:
			case 175:
			case 176:
			case 177:
			case 178:
				OperateStack("=");
				break;
			case 179:
			case 180:
			case 181:
			case 182:
			case 183:
			case 184:
			case 185:
			case 186:
			case 187:
			case 188:
			case 189:
			case 190:
			case 191:
			case 192:
			case 193:
			case 194:
				OperateStack("<>");
				break;
			case 195:
			case 196:
			case 197:
			case 198:
			case 199:
			case 200:
			case 201:
			case 202:
			case 203:
			case 204:
			case 205:
			case 206:
				OperateStack(">");
				break;
			case 207:
			case 208:
			case 209:
			case 210:
			case 211:
			case 212:
			case 213:
			case 214:
			case 215:
			case 216:
			case 217:
			case 218:
				OperateStack("<");
				break;
			case 219:
			case 220:
			case 221:
			case 222:
			case 223:
			case 224:
			case 225:
			case 226:
			case 227:
			case 228:
			case 229:
			case 230:
				OperateStack(">=");
				break;
			case 231:
			case 232:
			case 233:
			case 234:
			case 235:
			case 236:
			case 237:
			case 238:
			case 239:
			case 240:
			case 241:
			case 242:
				OperateStack("<=");
				break;
			case 243:
			case 244:
			case 245:
			case 246:
			case 247:
			case 248:
			case 249:
				EndAssign2("++");
				break;
			case 250:
			case 251:
			case 252:
			case 253:
			case 254:
			case 255:
			case 256:
				EndAssign2("--");
				break;
			case 257:
			case 258:
			case 259:
			case 260:
			case 261:
			case 262:
			case 263:
				EndAssign("+");
				break;
			case 264:
			case 265:
			case 266:
			case 267:
			case 268:
			case 269:
			case 270:
				EndAssign("-");
				break;
			case 271:
			case 272:
			case 273:
			case 274:
			case 275:
			case 276:
			case 277:
				EndAssign("*");
				break;
			case 278:
				ResetAssign(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 282:
				BeginAssignLocalVariable(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 283:
				BeginAssignSharedVariable(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 284:
				BeginAssignGlobalVariable(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 285:
				BeginAssignLocalVariable(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 287:
				BeginAssignInstanceVariable();
				break;
			case 288:
			case 289:
			case 290:
			case 291:
			case 292:
			case 293:
			case 294:
			case 295:
			case 296:
				Cast(0);
				break;
			case 298:
			case 299:
			case 300:
			case 301:
			case 302:
			case 303:
			case 304:
			case 305:
			case 306:
			case 307:
			case 308:
			case 309:
			case 310:
			case 311:
			case 312:
			case 313:
			case 314:
			case 315:
				Cast(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 318:
			case 319:
				PushInstanceVariable(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 320:
			case 321:
			case 322:
			case 323:
				Cast(0);
				break;
			case 330:
			case 331:
				CallFunction(BufferHelper.GetUInt(codeLine.PCodeParam, 0L), BufferHelper.GetUShort(codeLine.PCodeParam, 4L), BufferHelper.GetUShort(codeLine.PCodeParam, 6L));
				break;
			case 332:
			case 333:
				PushLocalVariable(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 334:
			case 335:
				PushSharedVariable(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 336:
			case 337:
				PushGlobalVariable(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 342:
				EndAssign();
				break;
			case 343:
			case 344:
			case 345:
			case 346:
			case 347:
			case 348:
			case 349:
			case 350:
			case 351:
			case 352:
			case 353:
			case 354:
			case 355:
			case 356:
			case 357:
			case 358:
			case 359:
			case 360:
			case 361:
				Cast(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 362:
				CreateObject(BufferHelper.GetUInt(codeLine.PCodeParam, 0L));
				break;
			case 366:
				CallFunction(BufferHelper.GetUInt(codeLine.PCodeParam, 0L), BufferHelper.GetUShort(codeLine.PCodeParam, 4L), BufferHelper.GetUShort(codeLine.PCodeParam, 6L));
				break;
			case 367:
				PushLocalVariable(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 368:
				PushSharedVariable(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 369:
				PushGlobalVariable(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 372:
				OperateStack("+");
				break;
			case 373:
				OperateStack("-");
				break;
			case 374:
				OperateStack("*");
				break;
			case 375:
				OperateStack("/");
				break;
			case 376:
				OperateStack("^");
				break;
			case 377:
				OperateStackSingle("-");
				break;
			case 378:
				OperateStack("=");
				break;
			case 379:
				OperateStack("<>");
				break;
			case 380:
				OperateStack(">");
				break;
			case 381:
				OperateStack("<");
				break;
			case 382:
				OperateStack(">=");
				break;
			case 383:
				OperateStack("<=");
				break;
			case 384:
				OperateStack("and");
				break;
			case 385:
				OperateStack("or");
				break;
			case 386:
				OperateStackSingle("not");
				break;
			case 387:
				PushInstanceVariable(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 388:
			case 389:
				Cast(0);
				break;
			case 390:
				CallBuiltinFunction("int");
				break;
			case 391:
				CallBuiltinFunction("abs");
				break;
			case 392:
				CallBuiltinFunction("abs");
				break;
			case 393:
				CallBuiltinFunction("asc");
				break;
			case 394:
				CallBuiltinFunction("blob");
				break;
			case 395:
				CallBuiltinFunction("ceiling");
				break;
			case 396:
				CallBuiltinFunction("cos");
				break;
			case 397:
				CallBuiltinFunction("exp");
				break;
			case 398:
				CallBuiltinFunction("fact");
				break;
			case 399:
				CallBuiltinFunction("inthigh");
				break;
			case 400:
				CallBuiltinFunction("intlow");
				break;
			case 401:
				CallBuiltinFunction("isdate");
				break;
			case 402:
				CallBuiltinFunction("isnull");
				break;
			case 403:
				CallBuiltinFunction("isnumber");
				break;
			case 404:
				CallBuiltinFunction("istime");
				break;
			case 405:
				CallBuiltinFunction("isvalid");
				break;
			case 406:
				CallBuiltinFunction("lefttrim");
				break;
			case 407:
				CallBuiltinFunction("len");
				break;
			case 408:
				CallBuiltinFunction("len");
				break;
			case 409:
				CallBuiltinFunction("log");
				break;
			case 410:
				CallBuiltinFunction("logten");
				break;
			case 411:
				CallBuiltinFunction("lower");
				break;
			case 412:
				CallBuiltinFunction("pi");
				break;
			case 413:
				CallBuiltinFunction("rand");
				break;
			case 415:
				CallBuiltinFunction("righttrim");
				break;
			case 416:
				CallBuiltinFunction("sin");
				break;
			case 417:
				CallBuiltinFunction("sqrt");
				break;
			case 418:
				CallBuiltinFunction("tan");
				break;
			case 419:
				CallBuiltinFunction("trim");
				break;
			case 420:
				CallBuiltinFunction("upper");
				break;
			case 422:
				PushGlobalVariable(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 425:
				PushLocalVariable(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 426:
				PushSharedVariable(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 427:
				Cast(0);
				break;
			case 430:
				Cast(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 431:
				Index();
				break;
			case 432:
				Index2(BufferHelper.GetUShort(codeLine.PCodeParam, 0L), BufferHelper.GetUShort(codeLine.PCodeParam, 2L));
				break;
			case 433:
				Index3(BufferHelper.GetUShort(codeLine.PCodeParam, 0L), BufferHelper.GetUShort(codeLine.PCodeParam, 2L), BufferHelper.GetUShort(codeLine.PCodeParam, 4L));
				break;
			case 434:
				CreateArray(BufferHelper.GetUShort(codeLine.PCodeParam, 4L));
				break;
			case 435:
				CreateArray(BufferHelper.GetUShort(codeLine.PCodeParam, 4L));
				break;
			case 438:
				Cast(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 440:
				CallBuiltinFunction("lowerbound");
				break;
			case 441:
				CallBuiltinFunction("upperbound");
				break;
			case 442:
				EndAssign2("++");
				break;
			case 443:
				EndAssign2("--");
				break;
			case 444:
				PushGlobalFunctionName(BufferHelper.GetUShort(codeLine.PCodeParam, 2L), BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 445:
			case 446:
			case 447:
				CallGlobalFunction(BufferHelper.GetUShort(codeLine.PCodeParam, 2L), BufferHelper.GetUShort(codeLine.PCodeParam, 4L));
				break;
			case 448:
				CallGlobalFunction(BufferHelper.GetUShort(codeLine.PCodeParam, 2L), BufferHelper.GetUShort(codeLine.PCodeParam, 4L));
				break;
			case 451:
				SqlExecuteImmediate();
				break;
			case 452:
				SqlExecuteDynamicDescriptor(BufferHelper.GetUInt(codeLine.PCodeParam, 0L));
				break;
			case 453:
				SqlFetchDynamicDescriptor();
				break;
			case 454:
				SqlOpenDynamicDescriptor(BufferHelper.GetUInt(codeLine.PCodeParam, 0L));
				break;
			case 456:
				CreateObjectUsingName(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 457:
				Cast(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 459:
				Cast(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 461:
			case 462:
				Cast(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 463:
				Cast(0);
				break;
			case 464:
				PushInstanceVariable(0);
				break;
			case 466:
				PushInstanceVariableName(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 467:
			case 468:
			case 469:
			case 470:
			case 471:
				CallBuiltinFunction("mod", 2);
				break;
			case 472:
			case 473:
				CallBuiltinFunction("abs");
				break;
			case 474:
				CallBuiltinFunction("ceiling");
				break;
			case 475:
			case 476:
			case 477:
			case 478:
			case 479:
				CallBuiltinFunction("min", 2);
				break;
			case 480:
			case 481:
			case 482:
			case 483:
			case 484:
				CallBuiltinFunction("max", 2);
				break;
			case 485:
				Try(BufferHelper.GetUShort(codeLine.PCodeParam, 0L), BufferHelper.GetUShort(codeLine.PCodeParam, 2L));
				break;
			case 486:
				EndTry();
				break;
			case 487:
				Catch();
				break;
			case 488:
				Throw();
				break;
			case 489:
				EnterFinally(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 490:
				LeaveFinally();
				break;
			case 491:
			case 492:
			case 493:
			case 494:
			case 495:
			case 496:
			case 497:
			case 498:
			case 499:
			case 500:
			case 501:
			case 502:
			case 503:
			case 504:
				Cast(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 505:
				OperateStack("+");
				break;
			case 506:
				OperateStack("-");
				break;
			case 507:
				OperateStack("*");
				break;
			case 508:
				OperateStack("/");
				break;
			case 509:
				OperateStack("^");
				break;
			case 510:
				OperateStackSingle("-");
				break;
			case 511:
				PushConstant(BufferHelper.GetLongLong(PbFunction.Buffer, BufferHelper.GetUInt(codeLine.PCodeParam, 0L)));
				break;
			case 512:
				PushLocalVariable(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 513:
				PushGlobalVariable(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 515:
				PushSharedVariable(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 517:
				EndAssign();
				break;
			case 519:
				EndAssign("+");
				break;
			case 520:
				EndAssign("-");
				break;
			case 521:
				EndAssign("*");
				break;
			case 522:
				EndAssign2("++");
				break;
			case 523:
				EndAssign2("--");
				break;
			case 524:
				Cast(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 525:
				CallBuiltinFunction("abs");
				break;
			case 527:
				OperateStack("=");
				break;
			case 528:
				OperateStack("<>");
				break;
			case 529:
				OperateStack(">");
				break;
			case 530:
				OperateStack("<");
				break;
			case 531:
				OperateStack(">=");
				break;
			case 532:
				OperateStack("<=");
				break;
			case 533:
				CallBuiltinFunction("mod", 2);
				break;
			case 534:
				CallBuiltinFunction("min", 2);
				break;
			case 535:
				CallBuiltinFunction("max", 2);
				break;
			case 539:
				CallFunction(BufferHelper.GetUInt(codeLine.PCodeParam, 0L), BufferHelper.GetUShort(codeLine.PCodeParam, 4L), BufferHelper.GetUShort(codeLine.PCodeParam, 6L));
				break;
			case 540:
				CallGlobalFunction(BufferHelper.GetUShort(codeLine.PCodeParam, 2L), BufferHelper.GetUShort(codeLine.PCodeParam, 4L));
				break;
			case 542:
				PushInstanceVariable(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 543:
				Cast(0);
				break;
			case 544:
				Cast(BufferHelper.GetUShort(codeLine.PCodeParam, 0L));
				break;
			case 546:
				Cast(0);
				break;
			default:
				return false;
			case 37:
			case 317:
			case 449:
				break;
			}
			return true;
		}
	}
}
