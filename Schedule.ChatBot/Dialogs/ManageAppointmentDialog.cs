using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Configuration;
using Schedule.ChatBot.Utility;
using Schedule.ChatBot.Models;

namespace Schedule.ChatBot.Dialogs
{
    [LuisModel("b115709a-61fb-458e-bbf0-23b04386c273", "3de2208fc7604f3ba8bf598744f66f57")]
    [Serializable]
    public class ManageAppointmentDialog : LuisDialog<object>
    {
        private static string UserPhoneNumber = ConfigurationManager.AppSettings["UserPhone"];
        private Appointment _appointment;
        private static string[] CancellationOptions = new[] { "Transportation Problem", "Not Feeling Well", "Conflicting Appointment", "Yolo" };

        [LuisIntent("")] //none
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Sorry, I don't understand what you mean.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("schedulechatbot.intent.cancel")]
        public async Task CancelAppointment(IDialogContext context, LuisResult result)
        {
            _appointment = await RestApiClient.GetAppointmentByPhoneNumberAsync(UserPhoneNumber);

            if (_appointment != null)
                PromptDialog.Confirm(context, CancelAppointmentConfirmed, $"Are you sure you want to cancel your appointment with Dr. {_appointment.Therapist.LastName} on {_appointment.AppointmentDate.ToString("MM/dd/yyyy")} at {_appointment.AppointmentDate.ToString("hh:mm")}{_appointment.AppointmentDate.ToString("tt")}?", promptStyle: PromptStyle.None);
            else
            {
                await context.PostAsync($"Sorry, I wasn't able to find an appointment for phone number {UserPhoneNumber}.");
                context.Wait(MessageReceived);
            }
        }

        private async Task CancelAppointmentConfirmed(IDialogContext context, IAwaitable<bool> confirmation)
        {
            if (await confirmation)
            {
                PromptDialog.Choice(context, CancelAppointmentComplete, CancellationOptions, "Please provide a reason for cancelling.", promptStyle: PromptStyle.Keyboard);
            }
            else
            {
                await context.PostAsync("No problem, I haven't changed anything.");
                context.Wait(MessageReceived);
            }
        }

        private async Task CancelAppointmentComplete(IDialogContext context, IAwaitable<string> selection)
        {
            var choice = await selection;
            await RestApiClient.CancelAppointmentAsync(_appointment.Id, choice);
            await context.PostAsync("Thank you. Your appointment has been cancelled. Have a nice day!");

            context.Wait(MessageReceived);
        }

        [LuisIntent("schedulechatbot.intent.checkappointment")]
        public async Task CheckAppointment(IDialogContext context, LuisResult result)
        {
            var appointment = await RestApiClient.GetAppointmentByPhoneNumberAsync(UserPhoneNumber);

            if (appointment != null)
                await context.PostAsync($"Your appointment with Dr. {appointment.Therapist.LastName} is on {appointment.AppointmentDate.ToString("MM/dd/yyyy")} at {appointment.AppointmentDate.ToString("hh:mm")}{appointment.AppointmentDate.ToString("tt")}.");
            else
                await context.PostAsync($"Appointment not found for phone {UserPhoneNumber}.");

            context.Wait(MessageReceived);
        }

        [LuisIntent("schedulechatbot.intent.reschedule")]
        public async Task RescheduleAppointment(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Reschedule appointment");

            context.Wait(MessageReceived);
        }
    }
}