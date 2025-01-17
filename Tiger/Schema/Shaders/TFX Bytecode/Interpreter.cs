using Arithmic;
using Tiger;
using Tiger.Schema;
using Tiger.Schema.Shaders;

public class TfxBytecodeInterpreter
{
    public List<TfxData> Opcodes { get; set; }
    public List<string> Stack { get; set; }
    public List<string> Temp { get; set; }

    public TfxBytecodeInterpreter(List<TfxData> opcodes)
    {
        Opcodes = opcodes ?? new List<TfxData>();
        Stack = new(capacity: 64);
        Temp = new(capacity: 16);
    }

    private List<string> StackPop(int pops)
    {
        if (Stack.Count < pops)
        {
            throw new Exception("Not enough elements in the stack to pop.");
        }

        List<string> v = Stack.Skip(Stack.Count - pops).ToList();
        Stack.RemoveRange(Stack.Count - pops, pops);
        return v;
    }

    private void StackPush(string value)
    {
        if (Stack.Count >= Stack.Capacity)
        {
            throw new Exception("Stack is at capacity.");
        }

        Stack.Add(value);
    }

    private string StackTop()
    {
        if (Stack.Count == 0)
        {
            throw new Exception("Stack is empty.");
        }
        string top = Stack[Stack.Count - 1];
        Stack.RemoveAt(Stack.Count - 1);
        return top;
    }

    public Dictionary<int, string> Evaluate(DynamicArray<Vec4> constants, bool print = false, Material? material = null)
    {
        Dictionary<int, string> hlsl = new();
        try
        {
            if (print)
                Console.WriteLine($"--------Evaluating Bytecode:");
            foreach ((int _ip, var op) in Opcodes.Select((value, index) => (index, value)))
            {
                if (print)
                    Console.WriteLine($"0x{op.op:X} {op.op} : {TfxBytecodeOp.TfxToString(op, constants, material)}");
                switch (op.op)
                {
                    case TfxBytecode.Add:
                    case TfxBytecode.Add2:
                        var add = StackPop(2);
                        StackPush($"({add[0]} + {add[1]})");
                        break;
                    case TfxBytecode.Subtract:
                        var sub = StackPop(2);
                        StackPush($"({sub[0]} - {sub[1]})");
                        break;
                    case TfxBytecode.Multiply:
                    case TfxBytecode.Multiply2:
                        var mul = StackPop(2);
                        StackPush($"({mul[0]} * {mul[1]})");
                        break;
                    case TfxBytecode.Divide:
                        var div = StackPop(2);
                        StackPush($"({div[0]} / {div[1]})");
                        break;
                    case TfxBytecode.IsZero:
                        var isZero = StackTop();
                        StackPush($"(float4({isZero}.x == 0 ? 1 : 0, " +
                            $"{isZero}.y == 0 ? 1 : 0, " +
                            $"{isZero}.z == 0 ? 1 : 0, " +
                            $"{isZero}.w == 0 ? 1 : 0))");
                        break;
                    case TfxBytecode.Min:
                        var min = StackPop(2);
                        StackPush($"(min({min[0]}, {min[1]}))");
                        break;
                    case TfxBytecode.Max:
                        var max = StackPop(2);
                        StackPush($"(max({max[0]}, {max[1]}))");
                        break;
                    case TfxBytecode.LessThan: //I dont think I need to do < for each element?
                        var lessThan = StackPop(2);
                        StackPush(LessThan(lessThan[0], lessThan[1]));
                        break;
                    case TfxBytecode.Dot:
                        var dot = StackPop(2);
                        StackPush($"(dot({dot[0]}, {dot[1]}))");
                        break;
                    case TfxBytecode.Merge_1_3:
                        var merge = StackPop(2);
                        StackPush($"(float4({merge[0]}.x, {merge[1]}.x, {merge[1]}.y, {merge[1]}.z))");
                        break;
                    case TfxBytecode.Merge_2_2:
                        var merge2_2 = StackPop(2);
                        StackPush($"(float4({merge2_2[0]}.x, {merge2_2[0]}.y, {merge2_2[1]}.x, {merge2_2[1]}.y))");
                        break;
                    case TfxBytecode.Merge_3_1:
                        var merge3_1 = StackPop(2);
                        StackPush($"(float4({merge3_1[0]}.x, {merge3_1[0]}.y, {merge3_1[0]}.z, {merge3_1[1]}.x))");
                        break;
                    case TfxBytecode.Cubic:
                        var Unk0f = StackPop(2);
                        StackPush($"((({Unk0f[1]}.xxxx * {Unk0f[0]} + {Unk0f[1]}.yyyy) * ({Unk0f[0]} * {Unk0f[0]}) + ({Unk0f[1]}.zzzz * {Unk0f[0]} + {Unk0f[1]}.wwww)))");
                        break;
                    case TfxBytecode.Lerp:
                        var lerp = StackPop(3);
                        StackPush($"(lerp({lerp[1]}, {lerp[0]}, {lerp[2]}))");
                        break;
                    case TfxBytecode.LerpSaturated:
                        lerp = StackPop(3);
                        StackPush($"(saturate(lerp({lerp[1]}, {lerp[0]}, {lerp[2]})))");
                        break;
                    case TfxBytecode.MultiplyAdd:
                        var mulAdd = StackPop(3);
                        StackPush($"({mulAdd[0]} * {mulAdd[1]} + {mulAdd[2]})");
                        break;
                    case TfxBytecode.Clamp:
                        var clamp = StackPop(3);
                        StackPush($"(clamp({clamp[0]}, {clamp[1]}, {clamp[2]}))");
                        break;
                    case TfxBytecode.Abs:
                        StackPush($"(abs({StackTop()}))");
                        break;
                    case TfxBytecode.Sign:
                        StackPush($"(sign({StackTop()}))");
                        break;
                    case TfxBytecode.Floor:
                        StackPush($"(floor({StackTop()}))");
                        break;
                    case TfxBytecode.Ceil:
                        StackPush($"(ceil({StackTop()}))");
                        break;
                    case TfxBytecode.Round:
                        //S2 material expressions dont support round, for some reason...
                        StackPush($"(floor({StackTop()}+0.5))");
                        break;
                    case TfxBytecode.Frac:
                        StackPush($"(frac({StackTop()}))");
                        break;
                    case TfxBytecode.Negate:
                        StackPush($"(-{StackTop()})");
                        break;
                    case TfxBytecode.VecRotSin:
                        StackPush($"_trig_helper_vector_sin_rotations_estimate({StackTop()})");
                        break;
                    case TfxBytecode.VecRotCos:
                        StackPush($"_trig_helper_vector_cos_rotations_estimate({StackTop()})");
                        break;
                    case TfxBytecode.VecRotSinCos:
                        StackPush($"_trig_helper_vector_sin_cos_rotations_estimate({StackTop()})");
                        break;
                    case TfxBytecode.PermuteAllX:
                        StackPush($"({StackTop()}.xxxx)");
                        break;
                    case TfxBytecode.Permute:
                        var param = ((PermuteData)op.data).fields;
                        var permute = StackTop();
                        StackPush($"({permute}{TfxBytecodeOp.DecodePermuteParam(param)})");
                        break;
                    case TfxBytecode.Saturate:
                        StackPush($"(saturate({StackTop()}))");
                        break;
                    case TfxBytecode.Triangle:
                        StackPush($"bytecode_op_triangle({StackTop()})");
                        break;
                    case TfxBytecode.Jitter:
                        StackPush($"bytecode_op_jitter({StackTop()})");
                        break;
                    case TfxBytecode.Wander:
                        StackPush($"bytecode_op_wander({StackTop()})");
                        break;
                    case TfxBytecode.Rand:
                        StackPush($"bytecode_op_rand({StackTop()})");
                        break;
                    case TfxBytecode.RandSmooth:
                        StackPush($"bytecode_op_rand_smooth({StackTop()})");
                        break;
                    case TfxBytecode.TransformVec4:
                        var TransformVec4 = StackPop(5);
                        StackPush($"{mul_vec4(TransformVec4)}");
                        break;
                    case TfxBytecode.PushConstantVec4:
                        var vec = constants[((PushConstantVec4Data)op.data).constant_index].Vec;
                        StackPush($"float4{vec}");
                        break;
                    case TfxBytecode.LerpConstant:
                        var t = StackTop();
                        var a = constants[((LerpConstantData)op.data).constant_start].Vec;
                        var b = constants[((LerpConstantData)op.data).constant_start + 1].Vec;

                        StackPush($"(lerp(float4{a}, float4{b}, {t}))");
                        break;
                    case TfxBytecode.LerpConstantSaturated:
                        t = StackTop();
                        a = constants[((LerpConstantData)op.data).constant_start].Vec;
                        b = constants[((LerpConstantData)op.data).constant_start + 1].Vec;

                        StackPush($"(saturate(lerp(float4{a}, float4{b}, {t})))");
                        break;
                    case TfxBytecode.Spline4Const:
                        var X = StackTop();
                        var C3 = $"float4{constants[((Spline4ConstData)op.data).constant_index].Vec}";
                        var C2 = $"float4{constants[((Spline4ConstData)op.data).constant_index + 1].Vec}";
                        var C1 = $"float4{constants[((Spline4ConstData)op.data).constant_index + 2].Vec}";
                        var C0 = $"float4{constants[((Spline4ConstData)op.data).constant_index + 3].Vec}";
                        var threshold = $"float4{constants[((Spline4ConstData)op.data).constant_index + 4].Vec}";

                        StackPush($"bytecode_op_spline4_const({X}, {C3}, {C2}, {C1}, {C0}, {threshold})");
                        break;
                    case TfxBytecode.Spline8Const:
                        var s8c_X = StackTop();
                        var s8c_C3 = $"float4{constants[((Spline8ConstData)op.data).constant_index].Vec}";
                        var s8c_C2 = $"float4{constants[((Spline8ConstData)op.data).constant_index + 1].Vec}";
                        var s8c_C1 = $"float4{constants[((Spline8ConstData)op.data).constant_index + 2].Vec}";
                        var s8c_C0 = $"float4{constants[((Spline8ConstData)op.data).constant_index + 3].Vec}";
                        var s8c_D3 = $"float4{constants[((Spline8ConstData)op.data).constant_index + 4].Vec}";
                        var s8c_D2 = $"float4{constants[((Spline8ConstData)op.data).constant_index + 5].Vec}";
                        var s8c_D1 = $"float4{constants[((Spline8ConstData)op.data).constant_index + 6].Vec}";
                        var s8c_D0 = $"float4{constants[((Spline8ConstData)op.data).constant_index + 7].Vec}";
                        var s8c_CThresholds = $"float4{constants[((Spline8ConstData)op.data).constant_index + 8].Vec}";
                        var s8c_DThresholds = $"float4{constants[((Spline8ConstData)op.data).constant_index + 9].Vec}";

                        StackPush($"bytecode_op_spline8_const({s8c_X}, {s8c_C3}, {s8c_C2}, {s8c_C1}, {s8c_C0}, {s8c_D3}, {s8c_D2}, {s8c_D1}, {s8c_D0}, {s8c_CThresholds}, {s8c_DThresholds})");
                        break;
                    case TfxBytecode.Spline8ConstChain:
                        var s8cc_X = StackTop();
                        var s8cc_Recursion = $"float4{constants[((Spline8ConstData)op.data).constant_index].Vec}";
                        var s8cc_C3 = $"float4{constants[((Spline8ConstData)op.data).constant_index + 1].Vec}";
                        var s8cc_C2 = $"float4{constants[((Spline8ConstData)op.data).constant_index + 2].Vec}";
                        var s8cc_C1 = $"float4{constants[((Spline8ConstData)op.data).constant_index + 3].Vec}";
                        var s8cc_C0 = $"float4{constants[((Spline8ConstData)op.data).constant_index + 4].Vec}";
                        var s8cc_D3 = $"float4{constants[((Spline8ConstData)op.data).constant_index + 5].Vec}";
                        var s8cc_D2 = $"float4{constants[((Spline8ConstData)op.data).constant_index + 6].Vec}";
                        var s8cc_D1 = $"float4{constants[((Spline8ConstData)op.data).constant_index + 7].Vec}";
                        var s8cc_D0 = $"float4{constants[((Spline8ConstData)op.data).constant_index + 8].Vec}";
                        var s8cc_CThresholds = $"float4{constants[((Spline8ConstData)op.data).constant_index + 9].Vec}";
                        var s8cc_DThresholds = $"float4{constants[((Spline8ConstData)op.data).constant_index + 10].Vec}";

                        StackPush($"bytecode_op_spline8_chain_const({s8cc_X}, {s8cc_Recursion}, {s8cc_C3}, {s8cc_C2}, {s8cc_C1}, {s8cc_C0}, {s8cc_D3}, {s8cc_D2}, {s8cc_D1}, {s8cc_D0}, {s8cc_CThresholds}, {s8cc_DThresholds})");
                        break;
                    case TfxBytecode.Gradient4Const:
                        var g4c_X = StackTop();
                        var BaseColor = $"float4{constants[((Gradient4ConstData)op.data).constant_index].Vec}";
                        var Cred = $"float4{constants[((Gradient4ConstData)op.data).constant_index + 1].Vec}";
                        var Cgreen = $"float4{constants[((Gradient4ConstData)op.data).constant_index + 2].Vec}";
                        var Cblue = $"float4{constants[((Gradient4ConstData)op.data).constant_index + 3].Vec}";
                        var Calpha = $"float4{constants[((Gradient4ConstData)op.data).constant_index + 4].Vec}";
                        var Cthresholds = $"float4{constants[((Gradient4ConstData)op.data).constant_index + 5].Vec}";

                        StackPush($"bytecode_op_gradient4_const({g4c_X}, {BaseColor}, {Cred}, {Cgreen}, {Cblue}, {Calpha}, {Cthresholds})");
                        break;
                    case TfxBytecode.Gradient8Const: // A massive unknown function with a 12 inputs, maybe this is Gradient8Const? (idk if that exists)
                        var g8c_X1 = StackTop();
                        var g8c_BaseColor = $"float4{constants[((Gradient8ConstData)op.data).constant_index].Vec}";
                        var g8c_Cred = $"float4{constants[((Gradient8ConstData)op.data).constant_index + 1].Vec}";
                        var g8c_Cgreen = $"float4{constants[((Gradient8ConstData)op.data).constant_index + 2].Vec}";
                        var g8c_Cblue = $"float4{constants[((Gradient8ConstData)op.data).constant_index + 3].Vec}";
                        var g8c_Calpha = $"float4{constants[((Gradient8ConstData)op.data).constant_index + 4].Vec}";
                        var g8c_Dred = $"float4{constants[((Gradient8ConstData)op.data).constant_index + 5].Vec}";
                        var g8c_Dgreen = $"float4{constants[((Gradient8ConstData)op.data).constant_index + 6].Vec}";
                        var g8c_Dblue = $"float4{constants[((Gradient8ConstData)op.data).constant_index + 7].Vec}";
                        var g8c_Dalpha = $"float4{constants[((Gradient8ConstData)op.data).constant_index + 8].Vec}";
                        var g8c_Cthresholds = $"float4{constants[((Gradient8ConstData)op.data).constant_index + 9].Vec}";
                        var g8c_Dthresholds = $"float4{constants[((Gradient8ConstData)op.data).constant_index + 10].Vec}";

                        StackPush($"bytecode_op_gradient8_const({g8c_X1}, {g8c_BaseColor}, {g8c_Cred}, {g8c_Cgreen}, {g8c_Cblue}, {g8c_Calpha}, {g8c_Dred}, {g8c_Dgreen}, {g8c_Dblue}, {g8c_Dalpha}, {g8c_Cthresholds}, {g8c_Dthresholds})");
                        break;

                    case TfxBytecode.PushExternInputFloat:
                        var v = Externs.GetExternFloat(((PushExternInputFloatData)op.data).extern_, ((PushExternInputFloatData)op.data).element * 4);
                        StackPush(v);
                        break;
                    case TfxBytecode.PushExternInputVec4:
                        var PushExternInputVec4 = Externs.GetExternVec4(((PushExternInputVec4Data)op.data).extern_, ((PushExternInputVec4Data)op.data).element * 16);
                        StackPush(PushExternInputVec4);
                        break;
                    case TfxBytecode.PushExternInputMat4:
                        //var Mat4 = Matrix4x4.Identity;
                        StackPush($"float4(1,0,0,0)");
                        StackPush($"float4(0,1,0,0)");
                        StackPush($"float4(0,0,1,0)");
                        StackPush($"float4(0,0,0,1)");
                        break;

                    // Texture stuff
                    case TfxBytecode.PushTexDimensions:
                        var ptd = ((PushTexDimensionsData)op.data);
                        Texture tex = FileResourcer.Get().GetFile<Texture>(material.PSSamplers[ptd.index].Hash);
                        StackPush($"float4({tex.TagData.Width}, {tex.TagData.Height}, {tex.TagData.Depth}, {tex.TagData.ArraySize}){TfxBytecodeOp.DecodePermuteParam(ptd.fields)}");
                        break;
                    case TfxBytecode.PushTexTileParams:
                        var ptt = ((PushTexTileParamsData)op.data);
                        tex = FileResourcer.Get().GetFile<Texture>(material.PSSamplers[ptt.index].Hash);
                        StackPush($"float4{tex.TagData.TilingScaleOffset}{TfxBytecodeOp.DecodePermuteParam(ptt.fields)}");
                        break;
                    case TfxBytecode.PushTexTileCount:
                        var pttc = ((PushTexTileCountData)op.data);
                        tex = FileResourcer.Get().GetFile<Texture>(material.PSSamplers[pttc.index].Hash);
                        StackPush($"float4({tex.TagData.TileCount}, {tex.TagData.ArraySize}, 0, 0){TfxBytecodeOp.DecodePermuteParam(pttc.fields)}");
                        break;
                    /////


                    case TfxBytecode.PushExternInputTextureView:
                    case TfxBytecode.PushExternInputUav:
                    case TfxBytecode.SetShaderTexture:
                    case TfxBytecode.SetShaderSampler:
                    case TfxBytecode.PushSampler:
                        break;

                    case TfxBytecode.PushFromOutput:
                        StackPush($"{hlsl[((PushFromOutputData)op.data).element]}");
                        break;

                    case TfxBytecode.PopOutputMat4:
                        var PopOutputMat4 = StackPop(4);
                        var Mat4_1 = PopOutputMat4[0];
                        var Mat4_2 = PopOutputMat4[1];
                        var Mat4_3 = PopOutputMat4[2];
                        var Mat4_4 = PopOutputMat4[3];

                        hlsl.TryAdd(((PopOutputMat4Data)op.data).slot, Mat4_1);
                        hlsl.TryAdd(((PopOutputMat4Data)op.data).slot + 1, Mat4_2);
                        hlsl.TryAdd(((PopOutputMat4Data)op.data).slot + 2, Mat4_3);
                        hlsl.TryAdd(((PopOutputMat4Data)op.data).slot + 3, Mat4_4);
                        Stack.Clear();
                        break;
                    case TfxBytecode.PushTemp:
                        var PushTemp = ((PushTempData)op.data).slot;
                        StackPush(Temp[PushTemp]);
                        break;
                    case TfxBytecode.PopTemp:
                        var PopTemp = ((PopTempData)op.data).slot;
                        var PopTemp_v = StackTop();
                        Temp.Insert(PopTemp, PopTemp_v);
                        break;


                    // Unknown or Useless
                    case TfxBytecode.Unk42:
                    case TfxBytecode.Unk4c:
                        StackPush($"float4(1,1,1,1)");
                        break;
                    case TfxBytecode.Unk50:
                        StackPush($"float4(0,0,0,0)");
                        break;
                    case TfxBytecode.Unk2c:
                    case TfxBytecode.Unk49:
                    case TfxBytecode.Unk51:
                        _ = StackPop(1);
                        break;
                    case TfxBytecode.Unk2d:
                        _ = StackPop(4);
                        break;
                    case TfxBytecode.Unk14:
                        _ = StackPop(2);
                        break;

                    case TfxBytecode.PushGlobalChannelVector:
                        var global_channel = GlobalChannels.Get(((PushGlobalChannelVectorData)op.data).unk1);
                        StackPush($"float4{global_channel}");
                        break;
                    case TfxBytecode.PushObjectChannelVector:
                        StackPush($"float4(1, 1, 1, 1)");
                        break;

                    case TfxBytecode.PopOutput:
                        if (print)
                            Console.WriteLine($"----Output Stack Count: {Stack.Count}\n");

                        if (Stack.Count == 0) // Shouldnt happen
                            hlsl.TryAdd(((PopOutputData)op.data).slot, "float4(0, 0, 0, 0)");
                        else
                            hlsl.TryAdd(((PopOutputData)op.data).slot, StackTop());

                        Stack.Clear(); // Does this matter?
                        break;
                    default:
                        if (print)
                            Console.WriteLine($"Not Implemented: {op.op}");
                        break;

                }
            }
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }

        return hlsl;
    }

    private string mul_vec4(List<string> TransformVec4) //probably wrong
    {
        var x_axis = TransformVec4[0];
        var y_axis = TransformVec4[1];
        var z_axis = TransformVec4[2];
        var w_axis = TransformVec4[3];
        var value = TransformVec4[4];

        string res = $"({x_axis}*{value}.xxxx)";  //x_axis.mul(rhs.xxxx());
        res = $"({res}+({y_axis}*{value}.yyyy))"; //res = res.add(self.y_axis.mul(rhs.yyyy()));
        res = $"({res}+({z_axis}*{value}.zzzz))"; //res = res.add(self.z_axis.mul(rhs.zzzz()));
        res = $"({res}+({w_axis}*{value}.wwww))"; //res = res.add(self.w_axis.mul(rhs.wwww()));

        return res;
    }

    private string GreaterThan(string a, string b)
    {
        return $"({a} > {b})";
    }

    private string LessThan(string a, string b)
    {
        return $"({a} < {b})";
    }
}
