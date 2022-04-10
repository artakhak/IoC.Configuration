using IoC.Configuration.ConfigurationFile;
using NUnit.Framework;
using OROptimizer.Diagnostics.Log;
using System;
using System.Text;
using TestsSharedLibrary;
using TestsSharedLibrary.Diagnostics.Log;

namespace IoC.Configuration.Tests
{
    [TestFixture]
    public class TypeParserTests
    {
        private TypeParser _typeParser = new TypeParser();

        [SetUp]
        public void TestInitialize()
        {
            TestsHelper.SetupLogger();
            Log4Tests.LogLevel = LogLevel.Info;
        }

        [TearDown]
        public void TestCleanup()
        {
            LogHelper.RemoveContext();
        }

        // errorMessage.AppendLine("Examples of valid type names are: IoC.Configuration._Class1, IoC.Configuration.IMyType[namespace1.IInterface1, namespace2.IInterface2, namespace2.Type2[namespace2.IInterface3]].");


        [Test]
        public void NonGenericClassTest1()
        {
            var expectedTypeData = new TypeData("_Class1");
            ValidateSuccessfulParseResult("_Class1", expectedTypeData);
        }

        [Test]
        public void NonGenericClassTest2()
        {
            var expectedTypeData = new TypeData("Class1");
            ValidateSuccessfulParseResult("Class1", expectedTypeData);
        }

        [Test]
        public void NonGenericClassTest3()
        {
            var expectedTypeData = new TypeData("Namespace1._Class1");
            ValidateSuccessfulParseResult("Namespace1._Class1", expectedTypeData);
        }

        [Test]
        public void NonGenericClassTest4()
        {
            var expectedTypeData = new TypeData("Namespace1.Class1");
            ValidateSuccessfulParseResult("Namespace1.Class1", expectedTypeData);
        }

        [Test]
        public void NonGenericClassTest5()
        {
            var expectedTypeData = new TypeData("Namespace1.Class1");
            ValidateSuccessfulParseResult(" Namespace1.Class1 ", expectedTypeData);
        }

        [Test]
        public void NonGenericClassTest6()
        {
            var expectedTypeData = new TypeData("_Namespace1._Class1_");
            ValidateSuccessfulParseResult(" _Namespace1._Class1_ ", expectedTypeData);
        }

        [Test]
        public void NonGenericClassTest7()
        {
            var expectedTypeData = new TypeData("_Namespace1_._Class1_");
            ValidateSuccessfulParseResult(" _Namespace1_._Class1_ ", expectedTypeData);
        }

        [Test]
        public void GenericClassTest1()
        {
            var expectedTypeData = new TypeData("Namespace1.Class1");

            var typeData1 = new TypeData("Namespace2_.Class1");
            expectedTypeData.AddGenericTypeParameter(typeData1);

            ValidateSuccessfulParseResult($"Namespace1.Class1[Namespace2_.Class1]", expectedTypeData);
        }

        [Test]
        public void GenericClassTest2()
        {
            var expectedTypeData = new TypeData("Namespace1.Class1");

            var typeData1 = new TypeData("Namespace2_.Class1");
            expectedTypeData.AddGenericTypeParameter(typeData1);

            ValidateSuccessfulParseResult($" Namespace1.Class1 [ Namespace2_.Class1 ] ", expectedTypeData);
        }

        [Test]
        public void GenericClassTest3()
        {
            var expectedTypeData = new TypeData("_Namespace1._Class1_");

            var typeData1 = new TypeData("Namespace2_._Class1");
            expectedTypeData.AddGenericTypeParameter(typeData1);

            var typeData2 = new TypeData("Namespace3_._Class1_");
            typeData1.AddGenericTypeParameter(typeData2);

            ValidateSuccessfulParseResult($"_Namespace1._Class1_[Namespace2_._Class1[Namespace3_._Class1_]]", expectedTypeData);
        }

        [Test]
        public void GenericClassTest4()
        {
            var expectedTypeData = new TypeData("_Namespace1._Class1_");

            var typeData1 = new TypeData("Namespace2_._Class1");
            expectedTypeData.AddGenericTypeParameter(typeData1);

            var typeData2 = new TypeData("Namespace3_._Class1_");
            typeData1.AddGenericTypeParameter(typeData2);

            ValidateSuccessfulParseResult($" _Namespace1._Class1_ [ Namespace2_._Class1 [ Namespace3_._Class1_ ]  ] ", expectedTypeData);
        }

        [Test]
        public void GenericClassTest5()
        {
            var expectedTypeData = new TypeData("_Namespace1._Class1_");

            var typeData1 = new TypeData("Namespace2_._Class1");
            expectedTypeData.AddGenericTypeParameter(typeData1);

            var typeData2 = new TypeData("Namespace2_._Class2");
            expectedTypeData.AddGenericTypeParameter(typeData2);

            var typeData3 = new TypeData("Namespace3_._Class1_");
            typeData2.AddGenericTypeParameter(typeData3);

            ValidateSuccessfulParseResult($"_Namespace1._Class1_[Namespace2_._Class1,Namespace2_._Class2[Namespace3_._Class1_]]", expectedTypeData);
        }

        [Test]
        public void GenericClassTest6()
        {
            var expectedTypeData = new TypeData("_Namespace1._Class1_");

            var typeData1 = new TypeData("Namespace2_._Class1");
            expectedTypeData.AddGenericTypeParameter(typeData1);

            var typeData2 = new TypeData("Namespace2_._Class2");
            expectedTypeData.AddGenericTypeParameter(typeData2);

            var typeData3 = new TypeData("Namespace3_._Class1_");
            typeData2.AddGenericTypeParameter(typeData3);

            ValidateSuccessfulParseResult($" _Namespace1._Class1_ [ Namespace2_._Class1 , Namespace2_._Class2 [ Namespace3_._Class1_ ] ] ", expectedTypeData);
        }

        [Test]
        public void GenericClassTest7()
        {
            var expectedTypeData = new TypeData("_Namespace1._Class1_");

            var typeData1 = new TypeData("Namespace2_._Class1");
            expectedTypeData.AddGenericTypeParameter(typeData1);

            var typeData2 = new TypeData("Namespace3_._Class1_");
            typeData1.AddGenericTypeParameter(typeData2);

            var typeData3 = new TypeData("Namespace2_._Class2");
            expectedTypeData.AddGenericTypeParameter(typeData3);

            ValidateSuccessfulParseResult($"_Namespace1._Class1_[Namespace2_._Class1[Namespace3_._Class1_],Namespace2_._Class2]", expectedTypeData);
        }

        [Test]
        public void GenericClassTest8()
        {
            var expectedTypeData = new TypeData("_Namespace1._Class1_");

            var typeData1 = new TypeData("Namespace2_._Class1");
            expectedTypeData.AddGenericTypeParameter(typeData1);

            var typeData2 = new TypeData("Namespace3_._Class1_");
            typeData1.AddGenericTypeParameter(typeData2);

            var typeData3 = new TypeData("Namespace2_._Class2");
            expectedTypeData.AddGenericTypeParameter(typeData3);

            ValidateSuccessfulParseResult($" _Namespace1._Class1_ [ Namespace2_._Class1 [Namespace3_._Class1_] , Namespace2_._Class2 ] ", expectedTypeData);
        }

        [Test]
        public void GenericClassTest9()
        {
            var expectedTypeData = new TypeData("_Namespace1._Class1_");

            var typeData1 = new TypeData("Namespace2_._Class1");
            expectedTypeData.AddGenericTypeParameter(typeData1);

            var typeData2 = new TypeData("Namespace3_._Class1_");
            typeData1.AddGenericTypeParameter(typeData2);

            var typeData3 = new TypeData("Namespace2_._Class2");
            expectedTypeData.AddGenericTypeParameter(typeData3);

            var typeData4 = new TypeData("Namespace3_._Class4_");
            typeData3.AddGenericTypeParameter(typeData4);

            ValidateSuccessfulParseResult($"_Namespace1._Class1_[Namespace2_._Class1[Namespace3_._Class1_],Namespace2_._Class2[Namespace3_._Class4_]]", expectedTypeData);
        }

        [Test]
        public void GenericClassTest10()
        {
            var expectedTypeData = new TypeData("_Namespace1._Class1_");

            var typeData1 = new TypeData("Namespace2_._Class1");
            expectedTypeData.AddGenericTypeParameter(typeData1);

            var typeData2 = new TypeData("Namespace3_._Class1_");
            typeData1.AddGenericTypeParameter(typeData2);

            var typeData3 = new TypeData("Namespace2_._Class2");
            expectedTypeData.AddGenericTypeParameter(typeData3);

            var typeData4 = new TypeData("Namespace3_._Class4_");
            typeData3.AddGenericTypeParameter(typeData4);

            ValidateSuccessfulParseResult($" _Namespace1._Class1_ [ Namespace2_._Class1 [ Namespace3_._Class1_ ] , Namespace2_._Class2 [ Namespace3_._Class4_ ] ] ", expectedTypeData);
        }

        [Test]
        public void ArrayClassTest()
        {
            var expectedTypeData = new TypeData("_Namespace1._Class1_");
            expectedTypeData.IsArray = true;
            ValidateSuccessfulParseResult(" _Namespace1._Class1_#", expectedTypeData);
        }

        [Test]
        public void GenericArrayClassTest()
        {
            var typeData1 = new TypeData("_Namespace1._Class1_");
            typeData1.IsArray = true;

            var typeData1_1 = new TypeData("class4");
            typeData1.AddGenericTypeParameter(typeData1_1);

            var typeData1_2 = new TypeData("class5");
            typeData1_2.IsArray = true;
            typeData1.AddGenericTypeParameter(typeData1_2);

            var typeData1_3 = new TypeData("_Namespace2._Class2_");
            typeData1_3.IsArray = true;
            typeData1.AddGenericTypeParameter(typeData1_3);

            var typeData1_3_1 = new TypeData("Class6");
            typeData1_3.IsArray = true;
            typeData1_3.AddGenericTypeParameter(typeData1_3_1);

            var typeData1_4 = new TypeData("_Namespace3._Class3_");
            typeData1_4.IsArray = true;
            typeData1.AddGenericTypeParameter(typeData1_4);

            var typeData1_4_1 = new TypeData("_Namespace4._Class4_");
            typeData1_4_1.IsArray = true;
            typeData1_4.AddGenericTypeParameter(typeData1_4_1);

            ValidateSuccessfulParseResult(" _Namespace1._Class1_[class4, class5#, _Namespace2._Class2_ [Class6 #] # ,  _Namespace3._Class3_ [ _Namespace4._Class4_ # ] # ] #", typeData1);
        }

        
        [TestCase(" _Namespace1._Class1_[class1", 28)]
        [TestCase("3Namespace1_._Class1", 0)]
        [TestCase(" Namespace1_._Class1.", 21)]
        [TestCase(" Namespace1_.._Class1", 13)]
        [TestCase(" 3Namespace1_._Class1", 1)]
        [TestCase(".Namespace1_._Class1", 0)]
        [TestCase(" .Namespace1_._Class1", 1)]
        [TestCase(" Namespace1_.3_Class1", 13)]
        [TestCase(" Namespace1_.._Class1", 13)]
        [TestCase(" Namespace1_. _Class1", 14)]
        [TestCase(" Namespace1_._@Class1", 14)]
        [TestCase("_Namespace1._Class1_]", 20)]
        [TestCase("_Namespace1._Class1_,class2", 20)]
        [TestCase("_Namespace1._Class1_[", 21)]
        [TestCase("_Namespace1._Class1_[]", 21)]
        [TestCase("_Namespace1._Class1_[class1, ]", 29)]
        [TestCase("_Namespace1._Class1_[class1]]", 28)]
        [TestCase(" _Namespace1._Class1_[class1", 28)]
        [TestCase(" _Namespace1._Class1_[class1[class2[class3]]", 44)]
        [TestCase("_Namespace1._Class1_[namespace1.class1[namespace1.class2],]", 58)]
        [TestCase("_Namespace1._Class1_[namespace1.class1[namespace1.class2],namespace1.class3]]]", 76)]
        [TestCase("_Namespace1._Class1_[namespace1.class1[namespace1.class2],namespace1.class3", 75)]
        [TestCase(" _Namespace1._Class1_ [ namespace1.class1 [ namespace1.class2 ], namespace1.class3 [ class4 ]  ] ]", 97)]
        [TestCase("class1#[class2]", 7)]
        [TestCase("class1##", 7)]
        [TestCase("class1[class2[]#", 14)]
        [TestCase("class1[class2##]#", 14)]
        public void InvalidTypeFullNameTests(string typeFullName, int errorIndex)
        {
            LogHelper.Context.Log.Info($"typeFullName='{typeFullName}'");
            ValidateFailedParseResult(typeFullName, errorIndex);
        }

        private void ValidateFailedParseResult(string typeFullName, int errorIndex)
        {
            try
            {
                var parsedTypeData = _typeParser.Parse(typeFullName);
                Assert.Fail();
            }
            catch (ParseTypeException e)
            {
                Assert.AreEqual(errorIndex, e.ErrorIndex);

                LogHelper.Context.Log.Error(e.Message);

                var errorLocationText = "Error location: \"";
                LogHelper.Context.Log.Error($"{errorLocationText}{typeFullName}\"");

                var errorLocationLine = new StringBuilder();
                errorLocationLine.Append(new String('-', errorLocationText.Length + e.ErrorIndex));
                // 2191=up arrow.
                errorLocationLine.Append('\u2191');
                LogHelper.Context.Log.Error(errorLocationLine.ToString());
            }
        }


        private void ValidateSuccessfulParseResult(string typeFullName, ITypeData expectedTypeData)
        {
            var parsedTypeData = _typeParser.Parse(typeFullName);

            Assert.IsNotNull(parsedTypeData);

            Assert.AreEqual(expectedTypeData, parsedTypeData);
        }

    }
}
