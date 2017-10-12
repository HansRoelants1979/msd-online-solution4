using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakeXrmEasy;
using Tc.Crm.Plugins;
using Attributes = Tc.Crm.Plugins.Attributes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace Tc.Crm.UnitTests.Plugins
{
    [TestClass]
    public class CreditCardPatternValidationOfAppointmentTests
    {
        XrmFakedContext context = null;
        Guid appointmentId = Guid.NewGuid();

        [TestInitialize]
        public void Initialise()
        {
            var configuration = CreditCardData.GetCreditCardPattern();

            var appointment = new Entity(Entities.Appointment, this.appointmentId);
            appointment.Attributes.Add(Attributes.Appointment.Subject, "subject");
            appointment.Attributes.Add(Attributes.Appointment.Description, "description");

            context = new XrmFakedContext();
            context.Initialize(new List<Entity>() { configuration, appointment });
        }

        private XrmFakedPluginExecutionContext GetPluginContext(int stage, int mode, string messageName, string subject, string description, string entityName = Entities.Appointment)
        {
            var appointment = new Entity(entityName, (messageName == Messages.Update) ? this.appointmentId : Guid.NewGuid());
            appointment.Attributes.Add(Attributes.Appointment.Subject, subject);
            appointment.Attributes.Add(Attributes.Appointment.Description, description);

            var cntxt = new XrmFakedPluginExecutionContext();
            cntxt.InputParameters = new ParameterCollection();
            cntxt.InputParameters.Add(InputParameters.Target, appointment);

            cntxt.MessageName = messageName;
            cntxt.Stage = stage;
            cntxt.Mode = mode;
            cntxt.PrimaryEntityName = entityName;

            return cntxt;
        }

        [TestMethod]
        public void CheckForInvalidMessage()
        {
            var cntxt = GetPluginContext((int)PluginStage.Preoperation, (int)PluginMode.Synchronous, Messages.Associate, CreditCardData.SingleLineCreditCardText, CreditCardData.MultiLineCreditCardText);
            context.ExecutePluginWith<Tc.Crm.Plugins.Appointment.CreditCardPatternValidationOfAppointment>(cntxt);
            Assert.IsTrue(context.Data.Count == 2);
        }

        [TestMethod]
        public void CheckForInvalidOperation()
        {
            var cntxt = GetPluginContext((int)PluginStage.Postoperation, (int)PluginMode.Synchronous, Messages.Create, CreditCardData.SingleLineCreditCardText, CreditCardData.MultiLineCreditCardText);
            context.ExecutePluginWith<Tc.Crm.Plugins.Appointment.CreditCardPatternValidationOfAppointment>(cntxt);
            Assert.IsTrue(context.Data.Count == 2);
        }

        [TestMethod]
        public void CheckForInvalidMode()
        {
            var cntxt = GetPluginContext((int)PluginStage.Preoperation, (int)PluginMode.Asynchronous, Messages.Create, CreditCardData.SingleLineCreditCardText, CreditCardData.MultiLineCreditCardText);
            context.ExecutePluginWith<Tc.Crm.Plugins.Appointment.CreditCardPatternValidationOfAppointment>(cntxt);
            Assert.IsTrue(context.Data.Count == 2);
        }

        [TestMethod]
        public void CheckForInvalidEntity()
        {
            var cntxt = GetPluginContext((int)PluginStage.Preoperation, (int)PluginMode.Synchronous, Messages.Create, CreditCardData.SingleLineCreditCardText, CreditCardData.MultiLineCreditCardText, Entities.Booking);
            context.ExecutePluginWith<Tc.Crm.Plugins.Appointment.CreditCardPatternValidationOfAppointment>(cntxt);
            Assert.IsTrue(context.Data.Count == 2);
        }

        [TestMethod]
        public void CheckWithoutCreditCardDataOnCreateOperation()
        {
            var cntxt = GetPluginContext((int)PluginStage.Preoperation, (int)PluginMode.Synchronous, Messages.Create, "12345678901234", "  \r\n \r\n 123456789012345");
            context.ExecutePluginWith<Tc.Crm.Plugins.Appointment.CreditCardPatternValidationOfAppointment>(cntxt);
            Assert.IsTrue(context.Data.Count == 2);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException))]
        public void CheckWithCreditCardDataOnCreateOperation()
        {
            var cntxt = GetPluginContext((int)PluginStage.Preoperation, (int)PluginMode.Synchronous, Messages.Create, CreditCardData.SingleLineCreditCardText, CreditCardData.MultiLineCreditCardText);
            context.ExecutePluginWith<Tc.Crm.Plugins.Appointment.CreditCardPatternValidationOfAppointment>(cntxt);
        }

        [TestMethod]
        public void CheckWithoutCreditCardDataOnUpdateOperation()
        {
            var cntxt = GetPluginContext((int)PluginStage.Preoperation, (int)PluginMode.Synchronous, Messages.Update, "12345678901234", "  \r\n \r\n 123456789012345");
            context.ExecutePluginWith<Tc.Crm.Plugins.Appointment.CreditCardPatternValidationOfAppointment>(cntxt);
            Assert.IsTrue(context.Data.Count == 2);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException))]
        public void CheckWithCreditCardDataOnUpdateOperation()
        {
            var cntxt = GetPluginContext((int)PluginStage.Preoperation, (int)PluginMode.Synchronous, Messages.Update, CreditCardData.SingleLineCreditCardText, CreditCardData.MultiLineCreditCardText);
            context.ExecutePluginWith<Tc.Crm.Plugins.Appointment.CreditCardPatternValidationOfAppointment>(cntxt);
        }

    }
}
