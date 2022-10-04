﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using NUnit.Framework;

namespace OData2Poco.Tests
{
      [TestFixture]
    class AssemplyManagerTest
    {
          [Test]
          public void AddAsemplyTest()
          {
              var pocosetting = new PocoSetting();
              AssemplyManager am = new AssemplyManager(pocosetting, new List<ClassTemplate>());
              am.AddAssemply("xyz");
              Assert.IsTrue(am.AssemplyReference.Exists(m => m.Contains("xyz")));
          }
          [Test]
          public void AddAsemplyArrayTest()
          {
              var pocosetting = new PocoSetting();
              AssemplyManager am = new AssemplyManager(pocosetting, new List<ClassTemplate>());
              am.AddAssemply("xyz", "abc");
              Assert.IsTrue(am.AssemplyReference.Exists(m => m.Contains("xyz")));
              Assert.IsTrue(am.AssemplyReference.Exists(m => m.Contains("abc")));
          }
          [Test]
          //[TestCase("int", "?")]
              [TestCase("key","System.ComponentModel.DataAnnotations")]
            //{"required" ,"System.ComponentModel.DataAnnotations.Schema"},
            //{"table" ,"System.ComponentModel.DataAnnotations.Schema"},
            //{"json","Newtonsoft.Json"},  
            ////assemplies for datatype
            //{"geometry","Microsoft.Spatial"},  
            //{"geography", "Microsoft.Spatial"}  
          public void AddAsemplyByKey(string key ,string value)
          {
              var pocosetting =new PocoSetting
              {
                  Attributes = new List<string> { "key" },                 
              };

              AssemplyManager am = new AssemplyManager(pocosetting,new List<ClassTemplate>());
              Assert.IsTrue(am.AssemplyReference.Exists(m => m.Contains(value)));
          }
          [Test]
          public void AddAsemplyMultiAttributes()
          {
              var pocosetting = new PocoSetting
              {
                  Attributes= new List<string> { "tab","req"},
                  
              };

              AssemplyManager am = new AssemplyManager(pocosetting, new List<ClassTemplate>());
              Assert.IsTrue(am.AssemplyReference.Exists(m => m.Contains("System.ComponentModel.DataAnnotations.Schema")));
          }
          [Test]
          public void AddExternalAsemply()
          {
              var pocosetting = new PocoSetting
              {
                  Attributes = new List<string> { "tab", "req" },
              };

              AssemplyManager am = new AssemplyManager(pocosetting, new List<ClassTemplate>());
              am.AddAssemply("xyz");
              Assert.IsTrue(am.AssemplyReference.Exists(m => m.Contains("System.ComponentModel.DataAnnotations.Schema")));
              Assert.IsTrue(am.AssemplyReference.Exists(m => m.Contains("xyz")));
          }
    }
}
