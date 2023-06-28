using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MeetUpBot
{
    enum CreationStages
    {
        ThemeCreation,
        DescriptionCreation,
        TimeCoordination,
        Finished
    }
    class ReplyMenu
    {
        public static ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
        {
            new KeyboardButton[] { "Create Meeting" },
        })
        {
            ResizeKeyboard = true
        };

        public static ReplyKeyboardMarkup Create = new(new[]
        {
            new KeyboardButton[] { "Change Theme" },
            new KeyboardButton[] { "Change Description" },
            new KeyboardButton[] { "Add Users" },
			new KeyboardButton[] { "Finish" },
		})
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = true
        };
        public static InlineKeyboardMarkup DateChoose = new(new[]
        {
            new[] {InlineKeyboardButton.WithCallbackData (text:$"{DateTime.Now.Date}",callbackData: $"{DateTime.Now.Date}") }
            //new[] {InlineKeyboardButton.WithCallbackData (text:$"{DateTime.Now.AddDays(1).Date}",callbackData:$"{DateTime.Now.AddDays(1).Date}" ) },
            //new[] {InlineKeyboardButton.WithCallbackData (text: $"{DateTime.Now.AddDays(1).Date}", callbackData:$"{DateTime.Now.AddDays(1).Date}") }
        });
        public static string[] TimePoll =
        {
            "9:00 - 10:00" ,
            "10:00 - 11:00",
            "11:00 - 12:00",
            "12:00 - 13:00",
            "13:00 - 14:00",
            "14:00 - 15:00",
            "15:00 - 16:00",
            "16:00 - 17:00",
            "17:00 - 18:00",

        };


    }
}
