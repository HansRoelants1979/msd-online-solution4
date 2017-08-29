using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeXrmEasy;

using Tc.Crm.Plugins;
using Tc.Crm.Plugins.Note;
using Attributes = Tc.Crm.Plugins.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace Tc.Crm.UnitTests.Plugins
{
    [TestClass]
    public class PostNoteUpdateCreditCardPatternValidationTest
    {
        private Entity Configuration;
        private Entity Annotation;
        XrmFakedContext context = null;

        [TestInitialize]
        public void Initialise()
        {

            Configuration = new Entity(Entities.Configuration, Guid.NewGuid());
            Configuration.Attributes.Add(Attributes.Configuration.Name, Configurationkeys.CreditCardPattern);
            Configuration.Attributes.Add(Attributes.Configuration.Value, @"^\D*\d{16}\D*$");
            Annotation = new Entity(Entities.Annotation, Guid.NewGuid());
            Annotation.Attributes.Add(Attributes.Annotation.NoteText, "1234567812345678");
            context = new XrmFakedContext();
            context.Initialize(new List<Entity>() { Configuration, Annotation });

        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException))]
        public void TestCreateNote()
        {
            var target = Annotation;
            var pluginContext = new XrmFakedPluginExecutionContext();
            pluginContext.MessageName = Messages.Create;
            pluginContext.PrimaryEntityName = Entities.Annotation;
            pluginContext.InputParameters = new ParameterCollection();
            pluginContext.InputParameters.Add(InputParameters.Target, target);
            Assert.ThrowsException<InvalidPluginExecutionException>(() => context.ExecutePluginWith<Crm.Plugins.Note.PostNoteUpdateCreditCardPatternValidation>(pluginContext));

        }



        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException))]
        public void TestUpdateNote()
        {
            var target = Annotation;
            var pluginContext = new XrmFakedPluginExecutionContext();
            pluginContext.MessageName = Messages.Update;
            pluginContext.PrimaryEntityName = Entities.Annotation;
            pluginContext.InputParameters = new ParameterCollection();
            pluginContext.InputParameters.Add(InputParameters.Target, target);
            Assert.ThrowsException<InvalidPluginExecutionException>(() => context.ExecutePluginWith<Crm.Plugins.Note.PostNoteUpdateCreditCardPatternValidation>(pluginContext));

        }
    }
}
