using MeetUpBot;
using MeetUpBot.Models;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;



const string TELEGRAM_TOKEN = "6019699889:AAFYlasAaCw2W8YwB69yGlNjtmVZ_grLSRU";



var botClient = new TelegramBotClient(TELEGRAM_TOKEN);


using CancellationTokenSource cts = new();


// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
    );

async Task SendMessage(ChatId chatId,string text,IReplyMarkup? replyKeyboardMarkup = null,CancellationToken cancellationToken = default)
{
    Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: text,
						parseMode: ParseMode.MarkdownV2,
						replyMarkup: replyKeyboardMarkup,
                        cancellationToken: cancellationToken);
    
}
async Task SendStatus(ChatId chatId, MeetUp Meeting) 
{
    using (var db = new MyDbContext())
    {
        //var meeting = db.MeetUps.ToList().Where(s => s.Id == Meeting.Id).First();
        string Time = "Уточняется";
        string stage;
        if (Meeting.Stage == CreationStages.Finished.ToString()) 
        {
            stage = "\u2705 Потверждено";
            Time = Meeting.Time.TimeOfDay.ToString();
        }
		else if (Meeting.Stage == CreationStages.TimeCoordination.ToString())
		{
			stage = "Ожидаем ответа участников";
		}
		else
		{
			stage = "Редактируется";
		}
        string status = $"Тема : {((Meeting.Theme != null)? Meeting.Theme:"Уточняется")}\n" +
                        $"Описание : {((Meeting.Description != null) ? Meeting.Theme : "Уточняется")}\n" +
                        $"Статус : {stage}\n"+
                        $"Время : {Time}";
		Message sentMessage = await botClient.SendTextMessageAsync(
						chatId: chatId,
                        text: status,
						parseMode: ParseMode.MarkdownV2);
	}

}

async Task HandleUpdateAsync(ITelegramBotClient botClient, Telegram.Bot.Types.Update update, CancellationToken cancellationToken)
{
    if (update.Type == UpdateType.Message  )
    {
        if (update.Message!.Type == MessageType.Text)
        {
            using (var db = new MyDbContext())
            {
                var message = update.Message;
                var chatId = message.Chat.Id;
                var messageText = message.Text;
                var repliedToMessage = message.ReplyToMessage;

                MeetUp Meeting = null;
                string Stage = null;
                var MeetUpEditing = false;
                if (db.MeetUps.ToList().Count != 0 && db.MeetUps.ToList().Where(p => p.Stage != CreationStages.Finished.ToString()).Count() != 0)
                {
                    MeetUpEditing = true;
                    Meeting = db.MeetUps.ToList().Where(p => p.Stage != CreationStages.Finished.ToString()).First();
                    Stage = Meeting.Stage;
                }


                Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");
                if (!messageText.Contains('/'))
                {
                    if (messageText == "Create Meeting")
                    {
                        await SendMessage(chatId, "Choose an option", ReplyMenu.Create, cancellationToken: cancellationToken);
                        db.MeetUps.Add(new MeetUp());
                        db.SaveChanges();

                    }
                    else if (MeetUpEditing == true)
                    {
                        if (messageText == "Change Theme")
                        {
                            Meeting.Stage = CreationStages.ThemeCreation.ToString();
                            db.SaveChanges();
                            await SendMessage(chatId, "Введите тему собрания", cancellationToken: cancellationToken);
                        }
                        else if (messageText == "Change Description")
                        {
                            Meeting.Stage = CreationStages.DescriptionCreation.ToString();
                            db.SaveChanges();
                            await SendMessage(chatId, "Введите описание собрания", cancellationToken: cancellationToken);
                        }
                        else if (messageText == "Add Users")
                        {
                            Meeting.Stage = CreationStages.TimeCoordination.ToString();
                            db.SaveChanges();
                            await SendMessage(chatId, "Введите TelegramId приглашаемых пользователей", cancellationToken: cancellationToken);
                        }
                        else if (messageText == "Finish")
                        {

                            bool TimeApproved = true;
                            foreach (var u in Meeting.Participants)
                            {
                                if (u.TimeChosen == false)
                                    TimeApproved = false;
                            }
                            /*var checker = db.Users.Where(
                                u => u.Id == (db.TimeSlots.GroupBy(t => t.User).Where(
							    t => (t.Count()) == (db.TimeSlots.GroupBy(ts => ts.User).Min(g => g.Count()))).Select(ts=>ts.Key).First()).Id
                                );*/
                            if (TimeApproved == true)
                            {
                                bool Approved = true;
                                var checker = db.Users.First();
                                DateTime ApprovedTime = db.TimeSlots.Where(ts => ts.User == checker).First().Start;
                                foreach (var tt in db.TimeSlots.GroupBy(ts => ts.User))
                                {
                                    Approved = true;
                                    if (tt.Key != checker)
                                    {
                                        foreach (var ts in tt)
                                        {
                                            if (db.TimeSlots.Where(t => t.User == checker).Contains(ts))
                                            {

                                                if (ApprovedTime == ts.Start)
                                                {
                                                    Approved = true;
                                                }
                                                else
                                                {
                                                    Approved = false;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                Meeting.Stage = CreationStages.Finished.ToString();
                                if (Approved == true)
                                {
                                    Meeting.Time = (DateTime)ApprovedTime;
                                    await SendStatus(chatId, Meeting);
                                    await SendMessage(chatId, "Choose an option", ReplyMenu.replyKeyboardMarkup, cancellationToken: cancellationToken);
                                }
                                else
                                {
                                    await SendMessage(chatId, "Не удалось согласовать время встречи", cancellationToken: cancellationToken);
                                }
                            }
                            else
                            {
                                await SendMessage(chatId, "Дождитесь согласования времени участников", cancellationToken: cancellationToken);
                            }
                            db.SaveChanges();
						}
                        else if (messageText.Contains('@'))
                        {
                            var id = messageText.Split('@')[1];
                            MeetUpBot.Models.User user;
                            if (db.Users.ToList().Where(p => p.TelegramID == id).Count() == 0)
                            {
								await SendMessage(chatId, "Неизвестный пользователь", cancellationToken: cancellationToken);
							}
                            else
                            {
                                
                                user = db.Users.ToList().Where(p => p.TelegramID == id).First();
								if (Meeting.Participants.Where(u => u.Id == user.Id).Count() == 0)
								{
									Meeting.Participants.Add(user);
                                    db.SaveChanges();
                                }
								if(user.TimeChosen == false)
								    await SendMessage(user.ChatID, $"{message.From.FirstName} {message.From.LastName} приглашает вас на встречу\n", ReplyMenu.DateChoose, cancellationToken: cancellationToken);
							}
                        }

                        else if (Stage == CreationStages.ThemeCreation.ToString())
                        {
                            Meeting.Theme = messageText;
                            db.SaveChanges();
                            await SendStatus(chatId, Meeting);
                        }
                        else if (Stage == CreationStages.DescriptionCreation.ToString())
                        {
                            Meeting.Description = messageText;
                            db.SaveChanges();
                            await SendStatus(chatId, Meeting);
                        }
                    }
                    else
                    {
                        await SendMessage(chatId, "Choose an option", ReplyMenu.replyKeyboardMarkup, cancellationToken: cancellationToken);

                    }
                }
                if (messageText.Contains('/'))
                {
                    if (messageText == "/start")
                    {
                        var user = new MeetUpBot.Models.User { Name = message.From.FirstName,Surname = message.From.LastName, TelegramID = message.From.Username, ChatID = chatId };
                        db.Users.Add(user);
                        db.SaveChanges();
                    }
                }
            }
        }
    }
    if (update.Type == UpdateType.PollAnswer)
    {
        using (var db = new MyDbContext())
        {
            var user = db.Users.Where(u=>u.ChatID == update.PollAnswer.User.Id).First();
            foreach (var option in update.PollAnswer.OptionIds)
            {
                var text = ReplyMenu.TimePoll[option];
                if (db.TimeSlots.ToList().Where(t=>t.Start == 
                                DateTime.Parse(string.Format(DateTime.Now.ToString("d"), text.Split(' ')[0]))).Count() == 0)
                {
                    db.TimeSlots.Add(new TimeSlot
                    {
                        User = db.Users.Where(u => u.ChatID == user.Id).First(),
                        Start = DateTime.Parse(text.Split(' ')[0])
                    });
                } 
            }
            user.TimeChosen = true;
            db.SaveChanges();
        }
	}
	if (update.Type == UpdateType.CallbackQuery)
	{
		if (update.CallbackQuery.Data != null)
		{
			var chatId = update.CallbackQuery.Message.Chat.Id;
			DateTime date;
			DateTime.TryParse(update.CallbackQuery.Data, out date);
			Message pollMessage = await botClient.SendPollAsync(
				chatId: chatId,
				question: "Choose comfort timeslots",
				options: ReplyMenu.TimePoll,
				allowsMultipleAnswers: true,
                isAnonymous: false,

				cancellationToken: cancellationToken);
		}
	}
	else
	{
		return;
	}
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}

var me = await botClient.GetMeAsync();
Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");
Console.ReadLine();
cts.Cancel();
