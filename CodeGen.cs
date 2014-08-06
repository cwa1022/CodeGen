using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Reflection;
using Microsoft.CSharp;
//using MsgPack;
using UnityEditor;
using UnityEngine;
 
//[InitializeOnLoad]
public class CodeGen 
{

	readonly static string path = "Assets/Plugins";

	readonly static string enumName = "AUDIOCLIP";
	
	CodeCompileUnit targetUnit;
	CodeNamespace cnamespace;
	
	
	static  CodeGen() 
	{
		//new CodeGen().Generate();
	}
	
	[MenuItem("Assets/CodeGen")]
	static void generate()
	{
		new CodeGen().Generate();
	}
	void Generate()
	{
		targetUnit = new CodeCompileUnit();
		cnamespace = new CodeNamespace("");
		createClass();	
		//createEnum();
		targetUnit.Namespaces.Add(cnamespace);
		
		string folderName="" ;
		
		UnityEngine.Object[] SelectedFolder = Selection.GetFiltered (typeof(UnityEngine.Object), SelectionMode.Assets);
		foreach (UnityEngine.Object obj in SelectedFolder) 
		{
			folderName = obj.name;
		}
		
		string filePath = path + "/" + folderName + ".cs";
		Directory.CreateDirectory(path);
		GenerateCSharpCode(filePath);
		
		string source = File.ReadAllText(filePath);
		source = source.Replace("public class", "public static class");
		File.WriteAllText(filePath, source);
		
		AssetDatabase.Refresh(ImportAssetOptions.Default);
		//moveball(GameObject);
	}
	
	void createClass()
	{
		string folderName="" ;
		
		UnityEngine.Object[] SelectedFolder = Selection.GetFiltered (typeof(UnityEngine.Object), SelectionMode.Assets);
		foreach (UnityEngine.Object obj in SelectedFolder) 
		{
			folderName = obj.name;
		}
		
		CodeTypeDeclaration targetClass = new CodeTypeDeclaration(folderName);
		targetClass.IsClass = true;
		targetClass.TypeAttributes = TypeAttributes.Public;
		
		
		
		UnityEngine.Object[] SelectedName = Selection.GetFiltered (typeof(UnityEngine.Object), SelectionMode.DeepAssets);
		
		foreach (UnityEngine.Object obj in SelectedName) 
		{
			if(obj.name == folderName)continue;
			//Debug.Log("obj.name = "+obj.name);
			GenerateMemberFiled(targetClass,obj.name);
		}
		
       	
		
		
		cnamespace.Types.Add(targetClass);
	}
	
	void GenerateMemberFiled(CodeTypeDeclaration targetClass,string att)
	{
		CodeMemberField widthValueField = new CodeMemberField();   
		widthValueField.Attributes = MemberAttributes.Public|MemberAttributes.Static;
        widthValueField.Name = att;
        widthValueField.Type = new CodeTypeReference(typeof(System.String));
		widthValueField.InitExpression  = new CodePrimitiveExpression(widthValueField.Name);
        widthValueField.Comments.Add(new CodeCommentStatement("The "+att+" of the object."));
        targetClass.Members.Add(widthValueField);
	}
	
	void createEnum()
	{
		string folderName="" ;
		
		UnityEngine.Object[] SelectedFolder = Selection.GetFiltered (typeof(UnityEngine.Object), SelectionMode.Assets);
		foreach (UnityEngine.Object obj in SelectedFolder) 
		{
			folderName = obj.name;
		}
		
		CodeTypeDeclaration type = new CodeTypeDeclaration(folderName);
		type.IsEnum = true;
		
		
		UnityEngine.Object[] SelectedName = Selection.GetFiltered (typeof(UnityEngine.Object), SelectionMode.DeepAssets);
		
		foreach (UnityEngine.Object obj in SelectedName) 
		{
			if(obj.name == folderName)continue;			
			GenerateEnumList(type,enumName,"res_"+obj.name);
		}
		
		
		cnamespace.Types.Add(type);
	}
	void GenerateEnumList(CodeTypeDeclaration type,string typeName,string member)
	{
		CodeMemberField f = new CodeMemberField(typeName, member);
  		type.Members.Add(f);
	}
	
	public void GenerateCSharpCode(string fileName)
	{
		CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
		CodeGeneratorOptions options = new CodeGeneratorOptions();
		options.BracingStyle = "C";
		using (StreamWriter sourceWriter = new StreamWriter(fileName))
		{
			provider.GenerateCodeFromCompileUnit(targetUnit, sourceWriter, options);
		}	
	}		
}
