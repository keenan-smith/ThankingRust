using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace ThankingRust
{
    /// <summary>
    /// Used to detour a specific function
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class OverrideAttribute : Attribute
    {
        #region Properties
        /// <summary>
        /// The class that contains the method
        /// </summary>
        public Type Class { get; private set; }
        /// <summary>
        /// The name of the method
        /// </summary>
        public string MethodName { get; private set; }
        /// <summary>
        /// The methodinfo of the method
        /// </summary>
        public MethodInfo Method { get; private set; }
        /// <summary>
        /// The flags to find the method
        /// </summary>
        public BindingFlags Flags { get; private set; }
        /// <summary>
        /// Was the method found
        /// </summary>
        public bool MethodFound { get; private set; }
        #endregion

        /// <inheritdoc />
        /// <summary>
        /// Used to detour a specific function
        /// </summary>
        /// <param name="tClass">The class containing the method</param>
        /// <param name="method">The name of the method to replace</param>
        /// <param name="flags">The flags of the method to replace</param>
        public OverrideAttribute(Type tClass, string method, BindingFlags flags, int index = 0)
        {
            // Set the variables
            Class = tClass;
            MethodName = method;
            Flags = flags;

            try
            {
                Method = Class.GetMethods(flags).Where(a => a.Name == method).ToArray()[index];

                MethodFound = true;
            }
            catch (Exception)
            {
                MethodFound = false;
            }
        }
    }

    public static class OverrideUtilities
    {
        #region Public Functions
        /// <summary>
        /// Calls the original method that was Overrideed
        /// </summary>
        /// <param name="method">The original method</param>
        /// <param name="instance">The instance for the method(null if static)</param>
        /// <param name="args">The arguments for the method</param>
        /// <returns>The value that the original function returns</returns>
        public static object CallOriginalFunc(MethodInfo method, object instance = null, params object[] args)
        {
            // Do the checks
            if (OverrideManager.Overrides.All(o => o.Value.Original != method))
                throw new Exception("The Override specified was not found!");

            // Set the variables
            OverrideWrapper wrapper = OverrideManager.Overrides.First(a => a.Value.Original == method).Value;

            return wrapper.CallOriginal(args, instance);
        }

        /// <summary>
        /// Calls the original method that was Overrideed
        /// </summary>
        /// <param name="instance">The instance for the method(null if static)</param>
        /// <param name="args">The arguments for the method</param>
        /// <returns>The value tahat the original function returns</returns>
        public static object CallOriginal(object instance = null, params object[] args)
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(false);

            if (trace.FrameCount < 1)
                throw new Exception("Invalid trace back to the original method! Please provide the methodinfo instead!");

            MethodBase modded = trace.GetFrame(1).GetMethod();
            MethodInfo original = null;

            if (!Attribute.IsDefined(modded, typeof(OverrideAttribute)))
                modded = trace.GetFrame(2).GetMethod();
            OverrideAttribute att = (OverrideAttribute)Attribute.GetCustomAttribute(modded, typeof(OverrideAttribute));

            if (att == null)
                throw new Exception("This method can only be called from an overwritten method!");
            if (!att.MethodFound)
                throw new Exception("The original method was never found!");
            original = att.Method;

            if (OverrideManager.Overrides.All(o => o.Value.Original != original))
                throw new Exception("The Override specified was not found!");

            OverrideWrapper wrapper = OverrideManager.Overrides.First(a => a.Value.Original == original).Value;

            return wrapper.CallOriginal(args, instance);
        }

        /// <summary>
        /// Enables the override of a method(WARNING: The method needs to have been overridden atleast once!)
        /// </summary>
        /// <param name="method">The original method that was overridden</param>
        /// <returns>If the override was enabled successfully</returns>
        public static bool EnableOverride(MethodInfo method)
        {
            // Set the variables
            OverrideWrapper wrapper = OverrideManager.Overrides.First(a => a.Value.Original == method).Value;

            // Do the checks
            return wrapper != null && wrapper.Override();
        }

        /// <summary>
        /// Disables the override of a method(WARNING: The method needs to have been overridden atleast once!)
        /// </summary>
        /// <param name="method">The original method that was Overrideed</param>
        /// <returns>If the Override was disabled successfully</returns>
        public static bool DisableOverride(MethodInfo method)
        {
            // Set the variables
            OverrideWrapper wrapper = OverrideManager.Overrides.First(a => a.Value.Original == method).Value;

            // Do the checks
            return wrapper != null && wrapper.Revert();
        }

        public static bool OverrideFunction(IntPtr ptrOriginal, IntPtr ptrModified)
        {
            try
            {
                switch (IntPtr.Size)
                {
                    case sizeof(Int32):
                        unsafe
                        {
                            byte* ptrFrom = (byte*)ptrOriginal.ToPointer();

                            *ptrFrom = 0x68; // PUSH
                            *((uint*)(ptrFrom + 1)) = (uint)ptrModified.ToInt32(); // Pointer
                            *(ptrFrom + 5) = 0xC3; // RETN

                            /* push, offset
                             * retn
                             * 
                             * 
                             */
                        }
                        break;
                    case sizeof(Int64):
                        unsafe
                        {
                            byte* ptrFrom = (byte*)ptrOriginal.ToPointer();

                            *ptrFrom = 0x48;
                            *(ptrFrom + 1) = 0xB8;
                            *((ulong*)(ptrFrom + 2)) = (ulong)ptrModified.ToInt64(); // Pointer
                            *(ptrFrom + 10) = 0xFF;
                            *(ptrFrom + 11) = 0xE0;

                            /* mov rax, offset
                             * jmp rax
                             * 
                             */
                        }
                        break;
                    default:
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return false;
            }
        }

        public static bool RevertOverride(OffsetBackup backup)
        {
            try
            {
                unsafe
                {
                    byte* ptrOriginal = (byte*)backup.Method.ToPointer();

                    *ptrOriginal = backup.A;
                    *(ptrOriginal + 1) = backup.B;
                    *(ptrOriginal + 10) = backup.C;
                    *(ptrOriginal + 11) = backup.D;
                    *(ptrOriginal + 12) = backup.E;
                    if (IntPtr.Size == sizeof(Int32))
                    {
                        *((uint*)(ptrOriginal + 1)) = backup.F32;
                        *(ptrOriginal + 5) = backup.G;
                    }
                    else
                    {
                        *((ulong*)(ptrOriginal + 2)) = backup.F64;
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region SubClasses
        public class OffsetBackup
        {
            #region Variables
            public IntPtr Method;

            public byte A, B, C, D, E, G;
            public ulong F64;
            public uint F32;
            #endregion

            public OffsetBackup(IntPtr method)
            {
                Method = method;

                unsafe
                {
                    byte* ptrMethod = (byte*)method.ToPointer();

                    A = *ptrMethod;
                    B = *(ptrMethod + 1);
                    C = *(ptrMethod + 10);
                    D = *(ptrMethod + 11);
                    E = *(ptrMethod + 12);
                    if (IntPtr.Size == sizeof(Int32))
                    {
                        F32 = *((uint*)(ptrMethod + 1));
                        G = *(ptrMethod + 5);
                    }
                    else
                    {
                        F64 = *((ulong*)(ptrMethod + 2));
                    }
                }
            }
        }
        #endregion
        #endregion
    }

    public class OverrideWrapper
    {
        #region Properties

        public MethodInfo Original { get; private set; }
        public MethodInfo Modified { get; private set; }

        public IntPtr PtrOriginal { get; private set; }
        public IntPtr PtrModified { get; private set; }

        public OverrideUtilities.OffsetBackup OffsetBackup { get; private set; }
        public OverrideAttribute Attribute { get; private set; }

        public bool Detoured { get; private set; }
        public object Instance { get; private set; }
        public bool Local { get; private set; }

        #endregion

        public OverrideWrapper(MethodInfo original, MethodInfo modified, OverrideAttribute attribute, object instance = null)
        {
            // Set the variables
            Original = original;
            Modified = modified;
            Instance = instance;
            Attribute = attribute;
            Local = Modified.DeclaringType.Assembly == Assembly.GetExecutingAssembly();

            RuntimeHelpers.PrepareMethod(original.MethodHandle);
            RuntimeHelpers.PrepareMethod(modified.MethodHandle);
            PtrOriginal = Original.MethodHandle.GetFunctionPointer();
            PtrModified = Modified.MethodHandle.GetFunctionPointer();

            OffsetBackup = new OverrideUtilities.OffsetBackup(PtrOriginal);
            Detoured = false;
        }

        #region Public Functions
        public bool Override()
        {
            if (Detoured)
                return true;
            bool result = OverrideUtilities.OverrideFunction(PtrOriginal, PtrModified);

            if (result)
                Detoured = true;

            return result;
        }

        public bool Revert()
        {
            if (!Detoured)
                return false;
            bool result = OverrideUtilities.RevertOverride(OffsetBackup);

            if (result)
                Detoured = false;

            return result;
        }

        public object CallOriginal(object[] args, object instance = null)
        {
            Revert();
            object result = null;
            try
            {
                result = Original.Invoke(instance ?? Instance, args);
            }
            catch (Exception)
            {

            }

            Override();
            return result;
        }
        #endregion
    }

    public static class OverrideManager
    {
        //TODO:Don't make regions for one line, or for one method

        // Dictionary of detours
        private static Dictionary<OverrideAttribute, OverrideWrapper> _overrides =
            new Dictionary<OverrideAttribute, OverrideWrapper>();

        // The public detours
        public static Dictionary<OverrideAttribute, OverrideWrapper> Overrides => _overrides;

        /// <summary>
        /// Loads override information for method
        /// </summary>
        /// <param name="method">Method to override another</param>
        public static void LoadOverride(MethodInfo method)
        {
            // Get attribute related variables
            OverrideAttribute attribute =
                (OverrideAttribute)Attribute.GetCustomAttribute(method, typeof(OverrideAttribute));

            // Check if method has been overrided before
            if (Overrides.Count(a => a.Key.Method == attribute.Method) > 0)
                return;

            // Create wrapper for override
            OverrideWrapper wrapper = new OverrideWrapper(attribute.Method, method, attribute);

            // Override
            wrapper.Override();

            // Add override to the list
            //Overrides.Add(attribute, wrapper);
        }
    }
}
