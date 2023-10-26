using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Authorizer.Extentions.Common;

namespace Authorizer.ClassesGeneration
{
	public class RawClassDeclarationInfo
	{
		public string ClassName { get; set; }

		public string Properties { get; set; }

		public int HierarchyLevel { get; set; }

		public string ResultPathProperty { get; set; }

		public bool EndOfTheResultPath { get; set; }

		public bool IsTopClassInHierarchy => HierarchyLevel == 1;
	}

	public class RawPropertyDeclarationInfo
	{
		private const int PropertyNameIndex = 0;
		private const int PropertyTypeIndex = 1;
		private const int PropertyClassOrNotIndex = 2;

		public static RawPropertyDeclarationInfo[] From(string propertiesRepresentation)
		{
			if (string.IsNullOrWhiteSpace(propertiesRepresentation)) throw new ArgumentNullException(nameof(propertiesRepresentation));

			var results = propertiesRepresentation
				.Split(";", StringSplitOptions.RemoveEmptyEntries)
				.Select(p =>
				{
					var splittedPropertyInfo = p.Split("|", StringSplitOptions.RemoveEmptyEntries);

					if (splittedPropertyInfo.Length < 2)
						throw new InvalidOperationException(
							"Property info can't contain less than 2 parameters (name and type).");
					
					var isNestedClass = splittedPropertyInfo.Length >= 3 && (splittedPropertyInfo[PropertyClassOrNotIndex].TryCastToBool(out var nestedClass) && nestedClass);
					return new RawPropertyDeclarationInfo
					{
						PropertyName = splittedPropertyInfo[PropertyNameIndex],
						PropertyTypeAsString = splittedPropertyInfo[PropertyTypeIndex],
						TypeIsOtherNestedClass = isNestedClass,
						PropertyType = Type.GetType(splittedPropertyInfo[PropertyTypeIndex]) ?? Type.GetType("System.Object")
					};
				}).ToArray();

			return results;
		}

		public string PropertyName { get; set; }

		public string PropertyTypeAsString { get; set; }

		public Type PropertyType { get; set; }

		public bool TypeIsOtherNestedClass { get; set; }
	}

	public static class RawClassDeclarationInfoExtensions
	{
		public static Type BuildHierarchy(this RawClassDeclarationInfo[] rawClasses)
		{
			if (rawClasses.Any() is false) throw new ArgumentNullException(nameof(rawClasses));


			var assName = "GeneratedAssembly";
			string assemblyFileName = $"{AppDomain.CurrentDomain.BaseDirectory}{assName}.dll"; 
			//Assembly generatedAssembly = Assembly.LoadFile(assemblyFileName);
			//
			//var typeName = "AccessValue2";
			//var builder2 = CreateTypeBuilder(typeName);
			//foreach (var prop in new (string PropertyName, string PropertyTypeAsString)[]
			//{
			//	("Value", "System.String"),
			//	("ExpirationDate", "System.DateTime"),
			//})
			//{
			//	CreateProperty(builder2, prop.PropertyName, prop.PropertyTypeAsString);
			//}
			//
			//var type2 = builder2.CreateType();

			// Get the dynamically generated type from the loaded assembly
			//Type generatedClassType = generatedAssembly.GetType($"{typeName}");
		//Type generatedClassType2 = generatedAssembly.GetType($"DynamicModule.{typeName}");





























			var halfPreparedClasses = rawClasses.Select(cl => (cl: cl, props: RawPropertyDeclarationInfo.From(cl.Properties))).ToArray();

			Type result = null;

			foreach (var (cl, props) in halfPreparedClasses.OrderByDescending(x => x.cl.HierarchyLevel))
			{
				var builder = CreateTypeBuilder(cl.ClassName);
				foreach (var prop in props)
				{
					CreateProperty(builder, prop.PropertyName, prop.PropertyTypeAsString);
				}

				var type = builder.CreateType();

				if (cl.IsTopClassInHierarchy)
				{
					result = type;
				}
			}

			if (result is null) throw new InvalidOperationException("Top hierarchy class was not found");
			return result;
		}

		private static TypeBuilder CreateTypeBuilder(string className)
		{
			var assemblyName = new AssemblyName("DynamicAssembly");
			var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");
			TypeBuilder typeBuilder = moduleBuilder.DefineType(className, TypeAttributes.Public | TypeAttributes.Class);

			
			return typeBuilder;
		}

		private static void CreateProperty(TypeBuilder typeBuilder, string propertyName, string propertyType)
		{
			if (propertyName == "AccessValue")
			{
				var af4 = Type.GetType("DynamicModule.AccessValue");
				var sflj = Type.GetType($"DynamicAssembly.DynamicModule.{propertyName}");
				var gjkl = Type.GetType($"DynamicAssembly.DynamicModule.{propertyName}", (asName) =>
				{
					var test = asName;

					var ass = Assembly.Load("DynamicAssembly");
					var ass2 = ass.GetType("AccessValue");
					var ass3 = ass.GetType("DynamicModule.AccessValue");

					return ass;
				}, null);

				var hlj = Type.GetType(
					"DynamicAssembly",
					(name) =>
					{
						// Returns the assembly of the type by enumerating loaded assemblies
						// in the app domain            
						return AppDomain.CurrentDomain.GetAssemblies().Where(z => z.FullName == name.FullName).FirstOrDefault();
					},
					null,
					false);
			}

			Type type = Type.GetType(propertyType) ?? Type.GetType("System.Object");
			FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + propertyName, type, FieldAttributes.Private);

			PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, type, null);

			MethodBuilder getMethodBuilder = typeBuilder.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, type, Type.EmptyTypes);
			ILGenerator getIlGenerator = getMethodBuilder.GetILGenerator();
			getIlGenerator.Emit(OpCodes.Ldarg_0);
			getIlGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
			getIlGenerator.Emit(OpCodes.Ret);

			MethodBuilder setMethodBuilder = typeBuilder.DefineMethod("set_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new Type[] { type });
			ILGenerator setIlGenerator = setMethodBuilder.GetILGenerator();
			setIlGenerator.Emit(OpCodes.Ldarg_0);
			setIlGenerator.Emit(OpCodes.Ldarg_1);
			setIlGenerator.Emit(OpCodes.Stfld, fieldBuilder);
			setIlGenerator.Emit(OpCodes.Ret);

			propertyBuilder.SetGetMethod(getMethodBuilder);
			propertyBuilder.SetSetMethod(setMethodBuilder);
		}
    }
}
