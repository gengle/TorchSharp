// Copyright (c) .NET Foundation and Contributors.  All Rights Reserved.  See LICENSE in the project root for license information.
#nullable enable
using System;
using System.Runtime.InteropServices;

namespace TorchSharp.PInvoke
{
    internal static partial class LibTorchSharp
    {
        [DllImport("LibTorchSharp")]
        internal static extern void THSJIT_CompilationUnit_Invoke(IntPtr module, string name, IntPtr tensors, int length, AllocatePinnedArray allocator, out sbyte typeCode);

        [DllImport("LibTorchSharp")]
        internal static extern void THSJIT_CompilationUnit_dispose(IntPtr handle);

        [DllImport("LibTorchSharp")]
        internal static extern IntPtr THSJIT_compile(string script);

        [DllImport("LibTorchSharp")]
        internal static extern void THSJIT_Module_dispose(torch.nn.Module.HType handle);

        [DllImport("LibTorchSharp")]
        internal static extern void THSJIT_Module_named_parameters(torch.nn.Module.HType module, AllocatePinnedArray allocator1, AllocatePinnedArray allocator2);

        [DllImport("LibTorchSharp")]
        internal static extern void THSJIT_Module_named_buffers(torch.nn.Module.HType module, AllocatePinnedArray allocator1, AllocatePinnedArray allocator2);

        [DllImport("LibTorchSharp")]
        internal static extern void THSJIT_Module_named_modules(torch.nn.Module.HType module, AllocatePinnedArray allocator1, AllocatePinnedArray allocator2);

        [DllImport("LibTorchSharp")]
        internal static extern void THSJIT_Module_named_children(torch.nn.Module.HType module, AllocatePinnedArray allocator1, AllocatePinnedArray allocator2);

        [DllImport("LibTorchSharp")]
        internal static extern long THSJIT_getNumModules(torch.nn.Module.HType module);

        [DllImport("LibTorchSharp")]
        internal static extern int THSJIT_Module_num_inputs(torch.nn.Module.HType module);

        [DllImport("LibTorchSharp")]
        internal static extern int THSJIT_Module_num_outputs(torch.nn.Module.HType module);

        [DllImport("LibTorchSharp")]
        internal static extern void THSJIT_Module_train(torch.nn.Module.HType module, bool on);

        [DllImport("LibTorchSharp")]
        internal static extern void THSJIT_Module_eval(torch.nn.Module.HType module);

        [DllImport("LibTorchSharp")]
        internal static extern bool THSJIT_Module_is_training(torch.nn.Module.HType module);

        [DllImport("LibTorchSharp")]
        internal static extern void THSJIT_Module_to_device(torch.nn.Module.HType module, long deviceType, long deviceIndex);

        [DllImport("LibTorchSharp")]
        internal static extern void THSJIT_Module_to_device_dtype(torch.nn.Module.HType module, sbyte dtype, long deviceType, long deviceIndex);

        [DllImport("LibTorchSharp")]
        internal static extern void THSJIT_Module_to_dtype(torch.nn.Module.HType module, sbyte dtype);

        [DllImport("LibTorchSharp")]
        internal static extern IntPtr THSJIT_Module_getInputType(torch.nn.Module.HType module, int index);

        [DllImport("LibTorchSharp")]
        internal static extern IntPtr THSJIT_getOutputType(torch.jit.Type.HType module, int index);

        [DllImport("LibTorchSharp")]
        internal static extern void THSJIT_Module_forward(torch.nn.Module.HType module, IntPtr tensors, int length, AllocatePinnedArray allocator, out sbyte typeCode);

        [DllImport("LibTorchSharp")]
        internal static extern void THSJIT_Module_invoke(torch.nn.Module.HType module, string name, IntPtr tensors, int length, AllocatePinnedArray allocator, out sbyte typeCode);

        [DllImport("LibTorchSharp")]
        internal static extern IntPtr THSJIT_load(string filename, long deviceType, long deviceIndex);

        [DllImport("LibTorchSharp")]
        internal static extern void THSJIT_save(torch.nn.Module.HType handle, string filename);

        [DllImport("LibTorchSharp")]
        internal static extern long THSJIT_TensorType_sizes(torch.jit.Type.HType handle, AllocatePinnedArray allocator);

        [DllImport("LibTorchSharp")]
        internal static extern int THSJIT_getDimensionedTensorTypeDimensions(torch.jit.Type.HType handle);

        [DllImport("LibTorchSharp")]
        internal static extern string THSJIT_getDimensionedTensorDevice(torch.jit.Type.HType handle);

        [DllImport("LibTorchSharp")]
        internal static extern sbyte THSJIT_TensorType_dtype(torch.jit.Type.HType handle);

        [DllImport("LibTorchSharp")]
        internal static extern void THSJIT_Type_dispose(torch.jit.Type.HType handle);

        [DllImport("LibTorchSharp")]
        internal static extern void THSJIT_TensorType_dispose(torch.jit.Type.HType handle);

        [DllImport("LibTorchSharp")]
        internal static extern sbyte THSJIT_Type_kind(torch.jit.Type.HType handle);

        [DllImport("LibTorchSharp")]
        internal static extern IntPtr THSJIT_Type_cast(torch.jit.Type.HType module);
    }
}
